using TapAi.Shared.Application.Abstraction;
using AppConc = TapAi.Shared.Application.ResponseObject.Concreate;

namespace TapAi.Module.Catalog.Persistence.Features.Brand.Queries.GetBrandById;

public sealed record GetBrandByIdRequest(Guid Id) : IQuery<AppConc.Response<GetBrandByIdResponse>>;