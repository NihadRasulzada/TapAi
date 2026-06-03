using Microsoft.EntityFrameworkCore;
using TapAi.Module.Identity.Application.Common.Interfaces;
using TapAi.Module.Identity.Domain.Entity;
using TapAi.Module.Identity.Persistence.Contexts;
using TapAi.Shared.Application.Abstraction;
using AppConc = TapAi.Shared.Application.ResponseObject.Concreate;

namespace TapAi.Module.Identity.Persistence.Features.Auth.Commands.RegisterVerify;

public sealed class RegisterVerifyHandler(
    IIdentityWriteDbContext writeDb,
    IOtpService otpService,
    IRegistrationJwtService registrationJwtService
) : ICommandHandler<RegisterVerifyRequest, AppConc.Response>
{
    public async Task<AppConc.Response> HandleAsync(
        RegisterVerifyRequest command, CancellationToken ct = default)
    {
        var claims = registrationJwtService.ValidateToken(command.RegistrationToken);
        if (claims is null)
            return AppConc.Response.Unauthorized("Invalid or expired registration token. Please restart registration.");

        var (record, ttl) = await otpService.GetAsync(claims.PhoneNumber, ct);
        if (record is null)
            return AppConc.Response.Unauthorized("OTP has expired or was already used. Please restart registration.");

        if (command.Otp != record.Code)
        {
            var newAttempts = record.Attempts + 1;

            if (newAttempts >= otpService.MaxAttempts)
            {
                await otpService.DeleteAsync(claims.PhoneNumber, ct);
                return AppConc.Response.Unauthorized(
                    "Maximum OTP attempts exceeded. Please restart registration.");
            }

            await otpService.UpdateAsync(
                claims.PhoneNumber,
                record with { Attempts = newAttempts },
                ttl ?? TimeSpan.FromSeconds(60),
                ct);

            var remaining = otpService.MaxAttempts - newAttempts;
            return AppConc.Response.Unauthorized(
                $"Invalid OTP. {remaining} attempt(s) remaining.");
        }

        var user = User.CreateWithPhone(
            claims.PhoneNumber,
            claims.PasswordHash,
            claims.FirstName,
            claims.LastName);

        writeDb.Add(user);

        try
        {
            await writeDb.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex)
            when (ex.InnerException?.Message.Contains("23505", StringComparison.Ordinal) == true
               || ex.InnerException?.Message.Contains("unique", StringComparison.OrdinalIgnoreCase) == true)
        {
            await otpService.DeleteAsync(claims.PhoneNumber, ct);
            return AppConc.Response.Conflict("This phone number is already registered.");
        }

        await otpService.DeleteAsync(claims.PhoneNumber, ct);

        return AppConc.Response.Success("Registration completed successfully.");
    }
}
