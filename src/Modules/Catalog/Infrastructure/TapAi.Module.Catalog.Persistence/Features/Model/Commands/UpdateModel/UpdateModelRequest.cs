using TapAi.Shared.Application.Abstraction;
using AppConc = TapAi.Shared.Application.ResponseObject.Concreate;

namespace TapAi.Module.Catalog.Persistence.Features.Model.Commands.UpdateModel;

public sealed record UpdateModelRequest(Guid Id, string Name, Guid BrandId)
    : ICommand<AppConc.Response<UpdateModelResponse>>;