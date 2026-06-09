using System.Text;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Npgsql;
using Scalar.AspNetCore;
using TapAi.API.Identity;
using TapAi.API.Middleware;
using TapAi.Module.Catalog.DependencyInjection.Extensions;
using TapAi.Module.Identity.DependencyInjection.Extensions;
using TapAi.Module.Media.DependencyInjection.Extensions;
using TapAi.Shared.Application.Abstraction;
using TapAi.Shared.Application.Pipeline;
using TapAi.Shared.Infrastructure.Implementations;
using TapAi.Shared.Infrastructure.Pipeline;
using TapAi.Shared.Infrastructure.Settings;

var builder = WebApplication.CreateBuilder(args);

// BackgroundService xətası bütün host-u çökürməsin;
// worker critical log yazır və dayanır, API işləməyə davam edir.
builder.Services.Configure<HostOptions>(opts =>
    opts.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore);

builder.Services.AddControllers();

// ── Current user (HTTP konteksti üzərindən JWT claim-ləri) ─────────────────────
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();

// ── OpenAPI / Scalar ──────────────────────────────────────────────────────────
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, ct) =>
    {
        document.Info = new OpenApiInfo
        {
            Title = "TapAi API",
            Version = "v1",
            Description = "REST API for the TapAi car marketplace platform."
        };
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "JWT access token"
        };
        return Task.CompletedTask;
    });
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ── Authentication / JWT ──────────────────────────────────────────────────────
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opts =>
    {
        opts.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]!))
        };
    });
builder.Services.AddAuthorization();

// ── Options ──────────────────────────────────────────────────────────────────
builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMq"));

// ── Dispatchers ───────────────────────────────────────────────────────────────
builder.Services.AddScoped<ICommandDispatcher, CommandDispatcher>();
builder.Services.AddScoped<IQueryDispatcher, QueryDispatcher>();

// ── Pipeline behaviors ────────────────────────────────────────────────────────
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));

// ── Modules ───────────────────────────────────────────────────────────────────
builder.Services.AddCatalogModule(builder.Configuration);
builder.Services.AddMediaModule(builder.Configuration);
builder.Services.AddIdentityModule(builder.Configuration);

// ── MassTransit / RabbitMQ ────────────────────────────────────────────────────
var rabbit = builder.Configuration.GetSection("RabbitMq");

builder.Services.AddMassTransit(x =>
{
    x.AddMediaConsumers();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(rabbit["Host"], rabbit["VirtualHost"], h =>
        {
            h.Username(rabbit["Username"] ?? string.Empty);
            h.Password(rabbit["Password"] ?? string.Empty);
        });

        cfg.ConfigureEndpoints(context);
    });
});

// ─────────────────────────────────────────────────────────────────────────────
var app = builder.Build();

// ── Database migrations ───────────────────────────────────────────────────────
// Əvvəlcə hər iki DB-yə migration tətbiq et, SONRA replication qur.
// Bu sıra vacibdir: subscription yarananda cədvəllər hər iki tərəfdə artıq mövcud olur.
await app.Services.MigrateCatalogAsync();
await app.Services.MigrateMediaAsync();
await app.Services.MigrateIdentityAsync();
await app.Services.SeedIdentityAsync();

// ── Logical replication subscription ─────────────────────────────────────────
await SetupReplicationAsync(app.Configuration, app.Logger);

// ── Middleware ────────────────────────────────────────────────────────────────
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "TapAi API";
        options.Theme = ScalarTheme.Default;
        options.Authentication = new ScalarAuthenticationOptions
        {
            PreferredSecurityScheme = "Bearer"
        };
    });
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
    app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

// ── Replication setup ─────────────────────────────────────────────────────────
// Migration-lardan SONRA çağırılır ki, subscription yarananda hər iki DB-də
// cədvəllər mövcud olsun. Beləliklə worker heç conflict görməz.
//
// copy_data = false: schema hər iki tərəfdə eynidir (EF migration tətbiq edilib),
// artıq mövcud sətirləri (məs. __EFMigrationsHistory) conflict olmadan
// yenidən kopyalamağa ehtiyac yoxdur. Yalnız yeni dəyişikliklər stream olunur.
static async Task SetupReplicationAsync(IConfiguration config, ILogger logger)
{
    var queryConnStr  = config.GetConnectionString("QueryDb");
    var commandConnStr = config.GetConnectionString("CommandDb");

    if (string.IsNullOrEmpty(queryConnStr) || string.IsNullOrEmpty(commandConnStr))
    {
        logger.LogWarning("[Replication] ConnectionStrings tapılmadı, keçilir.");
        return;
    }

    var pubName  = config["Replication:PublicationName"]  ?? "tapai_publication";
    var subName  = config["Replication:SubscriptionName"] ?? "tapai_subscription";
    var replUser = config["Replication:ReplicatorUser"]   ?? "replicator";
    var replPass = config["Replication:ReplicatorPassword"] ?? string.Empty;

    // command-db-nin query-db konteynerindən görünən adresi.
    // App localhost:5433 ilə bağlanır, amma subscription connection string-i
    // query-db prosesi tərəfindən oxunur — Docker internal hostname lazımdır.
    var cmdHost = config["Replication:CommandDbInternalHost"] ?? "command-db";
    var cmdPort = int.TryParse(config["Replication:CommandDbInternalPort"], out var p) ? p : 5432;
    var cmdDb   = config["Replication:CommandDbName"] ?? "tapai_command";

    try
    {
        // Subscription — query-db üzərindən idarə olunur (admin connection)
        await using var conn = new NpgsqlConnection(queryConnStr);
        await conn.OpenAsync();

        // Artıq mövcuddursa yaratma
        await using var checkCmd = conn.CreateCommand();
        checkCmd.CommandText =
            "SELECT COUNT(*) FROM pg_subscription WHERE subname = @name";
        checkCmd.Parameters.AddWithValue("name", subName);
        var exists = (long)(await checkCmd.ExecuteScalarAsync() ?? 0L) > 0;

        if (exists)
        {
            logger.LogInformation("[Replication] Subscription '{Sub}' artıq mövcuddur, keçilir.", subName);
            return;
        }

        // CREATE SUBSCRIPTION transaction xaricində işləməlidir.
        // NpgsqlConnection default olaraq auto-commit rejimindədir — əla.
        await using var createCmd = conn.CreateCommand();
        createCmd.CommandText = $"""
            CREATE SUBSCRIPTION {subName}
              CONNECTION 'host={cmdHost} port={cmdPort} dbname={cmdDb} user={replUser} password={replPass}'
              PUBLICATION {pubName}
              WITH (copy_data = false);
            """;
        await createCmd.ExecuteNonQueryAsync();

        logger.LogInformation(
            "[Replication] Subscription '{Sub}' yaradıldı. Publisher: {Host}/{Db}",
            subName, cmdHost, cmdDb);
    }
    catch (Exception ex)
    {
        // Replication olmazsa app işləməyə davam edir — query-db sadəcə geridə qalar.
        logger.LogError(ex,
            "[Replication] Subscription yaradılarkən xəta baş verdi. " +
            "Query-db replication olmadan işləyəcək.");
    }
}
