using TapAi.Shared.Application.Abstraction;
using AppConc = TapAi.Shared.Application.ResponseObject.Concreate;

namespace TapAi.Module.Identity.Persistence.Features.Auth.Commands.UnblockUser;

public sealed record UnblockUserRequest(Guid UserId) : ICommand<AppConc.Response>;
