using TapAi.Shared.Application.Abstraction;
using AppConc = TapAi.Shared.Application.ResponseObject.Concreate;

namespace TapAi.Module.Identity.Persistence.Features.Auth.Commands.Login;

public sealed record LoginRequest(
    string Email,
    string Password
) : ICommand<AppConc.Response<LoginResponse>>;
