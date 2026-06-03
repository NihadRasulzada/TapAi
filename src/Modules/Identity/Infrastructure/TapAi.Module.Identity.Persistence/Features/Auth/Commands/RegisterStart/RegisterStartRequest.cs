using TapAi.Shared.Application.Abstraction;
using AppConc = TapAi.Shared.Application.ResponseObject.Concreate;

namespace TapAi.Module.Identity.Persistence.Features.Auth.Commands.RegisterStart;

public sealed record RegisterStartRequest(
    string FirstName,
    string LastName,
    string PhoneNumber,
    string Password
) : ICommand<AppConc.Response<RegisterStartResponse>>;
