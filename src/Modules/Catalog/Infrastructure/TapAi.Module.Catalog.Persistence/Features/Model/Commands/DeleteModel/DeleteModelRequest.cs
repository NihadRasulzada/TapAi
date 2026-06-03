using TapAi.Shared.Application.Abstraction;
using AppConc = TapAi.Shared.Application.ResponseObject.Concreate;

namespace TapAi.Module.Catalog.Persistence.Features.Model.Commands.DeleteModel;

public sealed record DeleteModelRequest(Guid Id) : ICommand<AppConc.Response>;