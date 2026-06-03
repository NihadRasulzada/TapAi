using TapAi.Shared.Application.Abstraction;
using AppConc = TapAi.Shared.Application.ResponseObject.Concreate;

namespace TapAi.Module.Catalog.Persistence.Features.Model.Commands.CreateModel;

public sealed record CreateModelRequest(string Name, Guid BrandId) : ICommand<AppConc.Response<CreateModelResponse>>;