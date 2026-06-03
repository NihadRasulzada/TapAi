using TapAi.Shared.Application.Abstraction;
using AppConc = TapAi.Shared.Application.ResponseObject.Concreate;

namespace TapAi.Module.Identity.Persistence.Features.Auth.Commands.RefreshToken;

public sealed record RefreshTokenRequest(
    string RefreshToken
) : ICommand<AppConc.Response<RefreshTokenResponse>>;
