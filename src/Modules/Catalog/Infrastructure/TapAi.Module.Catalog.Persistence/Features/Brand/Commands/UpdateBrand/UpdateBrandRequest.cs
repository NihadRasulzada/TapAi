using TapAi.Shared.Application.Abstraction;
using AppConc = TapAi.Shared.Application.ResponseObject.Concreate;

namespace TapAi.Module.Catalog.Persistence.Features.Brand.Commands.UpdateBrand;

public sealed record UpdateBrandRequest(Guid Id, string Name) : ICommand<AppConc.Response<UpdateBrandResponse>>;