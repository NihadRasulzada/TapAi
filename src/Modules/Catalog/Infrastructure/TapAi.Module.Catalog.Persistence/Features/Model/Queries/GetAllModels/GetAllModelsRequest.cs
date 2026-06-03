using TapAi.Shared.Application.Abstraction;
using AppConc = TapAi.Shared.Application.ResponseObject.Concreate;

namespace TapAi.Module.Catalog.Persistence.Features.Model.Queries.GetAllModels;

public sealed record GetAllModelsRequest(Guid? BrandId = null)
    : IQuery<AppConc.Response<IReadOnlyList<GetAllModelsResponse>>>;