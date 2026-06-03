using TapAi.Shared.Application.Abstraction;
using AppConc = TapAi.Shared.Application.ResponseObject.Concreate;

namespace TapAi.Module.Identity.Persistence.Features.Auth.Commands.Register;

public sealed record RegisterUserRequest(
    string Email,
    string Password,
    string FirstName,
    string LastName
) : ICommand<AppConc.Response<RegisterUserResponse>>;
