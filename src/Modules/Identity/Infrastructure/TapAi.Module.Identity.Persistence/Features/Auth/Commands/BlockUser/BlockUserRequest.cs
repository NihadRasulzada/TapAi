using TapAi.Shared.Application.Abstraction;
using AppConc = TapAi.Shared.Application.ResponseObject.Concreate;

namespace TapAi.Module.Identity.Persistence.Features.Auth.Commands.BlockUser;

public sealed record BlockUserRequest(
    Guid UserId,
    int DurationSeconds
) : ICommand<AppConc.Response>;
