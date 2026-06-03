using TapAi.Shared.Application.Abstraction;
using AppConc = TapAi.Shared.Application.ResponseObject.Concreate;

namespace TapAi.Module.Catalog.Persistence.Features.Brand.Queries.GetAllBrands;

public sealed record GetAllBrandsRequest : IQuery<AppConc.Response<IReadOnlyList<GetAllBrandsResponse>>>;