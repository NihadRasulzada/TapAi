using TapAi.Shared.Application.Abstraction;
using AppConc = TapAi.Shared.Application.ResponseObject.Concreate;

namespace TapAi.Module.Identity.Persistence.Features.Auth.Commands.RegisterVerify;

public sealed record RegisterVerifyRequest(
    string RegistrationToken,
    string Otp
) : ICommand<AppConc.Response>;
