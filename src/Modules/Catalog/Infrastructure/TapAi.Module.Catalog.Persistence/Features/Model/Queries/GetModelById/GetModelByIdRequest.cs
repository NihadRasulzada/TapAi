using TapAi.Shared.Application.Abstraction;
using AppConc = TapAi.Shared.Application.ResponseObject.Concreate;

namespace TapAi.Module.Catalog.Persistence.Features.Model.Queries.GetModelById;

public sealed record GetModelByIdRequest(Guid Id) : IQuery<AppConc.Response<GetModelByIdResponse>>;