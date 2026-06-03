using TapAi.Shared.Application.Abstraction;
using AppConc = TapAi.Shared.Application.ResponseObject.Concreate;

namespace TapAi.Module.Catalog.Persistence.Features.Brand.Commands.DeleteBrand;

public sealed record DeleteBrandRequest(Guid Id) : ICommand<AppConc.Response>;