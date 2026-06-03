using TapAi.Shared.Application.Abstraction;
using AppConc = TapAi.Shared.Application.ResponseObject.Concreate;

namespace TapAi.Module.Catalog.Persistence.Features.Cars.Queries.GetCarConfig;

public sealed record GetCarConfigRequest : IQuery<AppConc.Response<GetCarConfigResponse>>;