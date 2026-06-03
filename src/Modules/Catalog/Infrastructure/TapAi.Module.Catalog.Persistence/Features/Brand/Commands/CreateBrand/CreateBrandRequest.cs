using TapAi.Shared.Application.Abstraction;
using AppConc = TapAi.Shared.Application.ResponseObject.Concreate;

namespace TapAi.Module.Catalog.Persistence.Features.Brand.Commands.CreateBrand;

public sealed record CreateBrandRequest(string Name) : ICommand<AppConc.Response<CreateBrandResponse>>;