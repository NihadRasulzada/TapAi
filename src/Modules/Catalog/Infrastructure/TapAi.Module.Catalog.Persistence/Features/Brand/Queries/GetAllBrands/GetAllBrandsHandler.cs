using TapAi.Module.Catalog.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

using TapAi.Shared.Application.Abstraction;
using AppConc = TapAi.Shared.Application.ResponseObject.Concreate;

namespace TapAi.Module.Catalog.Persistence.Features.Brand.Queries.GetAllBrands;

public sealed class GetAllBrandsHandler(ICatalogReadDbContext db)
    : IQueryHandler<GetAllBrandsRequest, AppConc.Response<IReadOnlyList<GetAllBrandsResponse>>>
{
    public async Task<AppConc.Response<IReadOnlyList<GetAllBrandsResponse>>> HandleAsync(
        GetAllBrandsRequest query, CancellationToken ct = default)
    {
        var brands = await db.Brands
            .AsNoTracking()
            .OrderBy(b => b.Name)
            .Select(b => new GetAllBrandsResponse(b.Id, b.Name))
            .ToListAsync(ct);

        return AppConc.Response<IReadOnlyList<GetAllBrandsResponse>>.Success(brands);
    }
}