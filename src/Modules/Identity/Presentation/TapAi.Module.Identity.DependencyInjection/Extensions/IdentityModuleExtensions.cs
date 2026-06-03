using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using TapAi.Module.Identity.Application.Common.Interfaces;
using TapAi.Module.Identity.Application.Common.Settings;
using TapAi.Module.Identity.Infrastructure.EmailWorker;
using TapAi.Module.Identity.Infrastructure.Messaging;
using TapAi.Module.Identity.Infrastructure.Options;
using TapAi.Module.Identity.Infrastructure.Services;
using TapAi.Module.Identity.Persistence.Contexts;
using TapAi.Module.Identity.Persistence.Features.Auth.Commands.BlockUser;
using TapAi.Module.Identity.Persistence.Features.Auth.Commands.ChangePassword;
using TapAi.Module.Identity.Persistence.Features.Auth.Commands.Login;
using TapAi.Module.Identity.Persistence.Features.Auth.Commands.RefreshToken;
using TapAi.Module.Identity.Persistence.Features.Auth.Commands.RegisterStart;
using TapAi.Module.Identity.Persistence.Features.Auth.Commands.RegisterVerify;
using TapAi.Module.Identity.Persistence.Features.Auth.Commands.UnblockUser;
using TapAi.Shared.Application.Abstraction;
using AppConc = TapAi.Shared.Application.ResponseObject.Concreate;

namespace TapAi.Module.Identity.DependencyInjection.Extensions;

public static class IdentityModuleExtensions
{
    public static IServiceCollection AddIdentityModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ── Options ───────────────────────────────────────────────────────────
        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<RegistrationJwtOptions>()
            .Bind(configuration.GetSection(RegistrationJwtOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<OtpSettings>()
            .Bind(configuration.GetSection(OtpSettings.SectionName));

        services.AddOptions<BruteForceSettings>()
            .Bind(configuration.GetSection(BruteForceSettings.SectionName));

        services.AddOptions<RabbitMqOptions>()
            .Bind(configuration.GetSection(RabbitMqOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<EmailOptions>()
            .Bind(configuration.GetSection(EmailOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        // ── DbContexts ────────────────────────────────────────────────────────
        services.AddDbContext<CommandDbContext>(opt =>
            opt.UseNpgsql(configuration.GetConnectionString("CommandDb")));

        services.AddDbContext<QueryDbContext>(opt =>
            opt.UseNpgsql(configuration.GetConnectionString("QueryDbApp")));

        // ── DbContext interface aliases ────────────────────────────────────────
        services.AddScoped<IIdentityWriteDbContext>(sp => sp.GetRequiredService<CommandDbContext>());
        services.AddScoped<IIdentityReadDbContext>(sp => sp.GetRequiredService<QueryDbContext>());

        // ── Redis ─────────────────────────────────────────────────────────────
        var redisConnStr = configuration.GetConnectionString("Redis")
            ?? throw new InvalidOperationException("ConnectionStrings:Redis tapılmadı.");
        services.AddSingleton<IConnectionMultiplexer>(_ =>
            ConnectionMultiplexer.Connect(redisConnStr));

        // ── Services ──────────────────────────────────────────────────────────
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
        services.AddScoped<IRegistrationJwtService, RegistrationJwtService>();
        services.AddSingleton<IOtpService, RedisOtpService>();
        services.AddSingleton<ILoginRateLimitService, RedisLoginRateLimitService>();
        services.AddSingleton<IEventPublisher, RabbitMqEventPublisher>();

        // ── Handlers ──────────────────────────────────────────────────────────
        services.AddScoped<
            ICommandHandler<RegisterStartRequest, AppConc.Response<RegisterStartResponse>>,
            RegisterStartHandler>();
        services.AddScoped<IValidator<RegisterStartRequest>, RegisterStartValidator>();

        services.AddScoped<
            ICommandHandler<RegisterVerifyRequest, AppConc.Response>,
            RegisterVerifyHandler>();

        services.AddScoped<
            ICommandHandler<LoginRequest, AppConc.Response<LoginResponse>>,
            LoginHandler>();
        services.AddScoped<IValidator<LoginRequest>, LoginValidator>();

        services.AddScoped<
            ICommandHandler<RefreshTokenRequest, AppConc.Response<RefreshTokenResponse>>,
            RefreshTokenHandler>();

        services.AddScoped<
            ICommandHandler<ChangePasswordRequest, AppConc.Response>,
            ChangePasswordHandler>();
        services.AddScoped<IValidator<ChangePasswordRequest>, ChangePasswordValidator>();

        services.AddScoped<
            ICommandHandler<BlockUserRequest, AppConc.Response>,
            BlockUserHandler>();
        services.AddScoped<IValidator<BlockUserRequest>, BlockUserValidator>();

        services.AddScoped<
            ICommandHandler<UnblockUserRequest, AppConc.Response>,
            UnblockUserHandler>();

        // ── Background Workers ────────────────────────────────────────────────
        services.AddHostedService<EmailConsumerWorker>();

        return services;
    }

    /// <summary>
    /// Identity modulu üçün pending migration-ları hər iki DB-yə tətbiq edir.
    /// QueryDb (admin) istifadə edilir — QueryDbApp (read-only) yox.
    /// </summary>
    public static async Task MigrateIdentityAsync(this IServiceProvider services)
    {
        var config = services.GetRequiredService<IConfiguration>();
        var logger = services.GetRequiredService<ILogger<CommandDbContext>>();

        var commandConnStr = config.GetConnectionString("CommandDb")
            ?? throw new InvalidOperationException("ConnectionStrings:CommandDb tapılmadı.");
        var queryConnStr = config.GetConnectionString("QueryDb")
            ?? throw new InvalidOperationException("ConnectionStrings:QueryDb tapılmadı.");

        await using var commandCtx = new CommandDbContext(
            new DbContextOptionsBuilder<CommandDbContext>()
                .UseNpgsql(commandConnStr)
                .Options);
        logger.LogInformation("[Identity] CommandDb migration tətbiq edilir...");
        await commandCtx.Database.MigrateAsync();

        await using var queryCtx = new QueryDbContext(
            new DbContextOptionsBuilder<QueryDbContext>()
                .UseNpgsql(queryConnStr)
                .Options);
        logger.LogInformation("[Identity] QueryDb migration tətbiq edilir...");
        await queryCtx.Database.MigrateAsync();

        logger.LogInformation("[Identity] Migration tamamlandı.");
    }
}
