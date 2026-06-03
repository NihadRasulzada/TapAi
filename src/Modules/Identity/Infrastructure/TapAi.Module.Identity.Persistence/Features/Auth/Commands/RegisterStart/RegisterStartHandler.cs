using Microsoft.EntityFrameworkCore;
using TapAi.Module.Identity.Application.Common.Interfaces;
using TapAi.Module.Identity.Application.Common.Settings;
using TapAi.Module.Identity.Persistence.Contexts;
using TapAi.Shared.Application.Abstraction;
using Microsoft.Extensions.Options;
using AppConc = TapAi.Shared.Application.ResponseObject.Concreate;

namespace TapAi.Module.Identity.Persistence.Features.Auth.Commands.RegisterStart;

public sealed class RegisterStartHandler(
    IIdentityReadDbContext readDb,
    IPasswordHasher passwordHasher,
    IOtpService otpService,
    IRegistrationJwtService registrationJwtService,
    IOptions<OtpSettings> otpOptions
) : ICommandHandler<RegisterStartRequest, AppConc.Response<RegisterStartResponse>>
{
    public async Task<AppConc.Response<RegisterStartResponse>> HandleAsync(
        RegisterStartRequest command, CancellationToken ct = default)
    {
        var normalized = command.PhoneNumber.ToUpperInvariant();

        var exists = await readDb.Users
            .AnyAsync(u => u.NormalizedPhoneNumber == normalized, ct);

        if (exists)
            return AppConc.Response<RegisterStartResponse>.Conflict(
                "This phone number is already registered.");

        var passwordHash = passwordHasher.Hash(command.Password);

        await otpService.StoreAsync(normalized, otpOptions.Value.DefaultCode, ct);

        var token = registrationJwtService.GenerateToken(
            normalized,
            passwordHash,
            command.FirstName,
            command.LastName);

        return AppConc.Response<RegisterStartResponse>.Success(
            new RegisterStartResponse(token));
    }
}
