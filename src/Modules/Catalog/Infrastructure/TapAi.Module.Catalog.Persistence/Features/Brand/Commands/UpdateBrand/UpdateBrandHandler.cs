using Microsoft.EntityFrameworkCore;
using TapAi.Module.Catalog.Persistence.Contexts;

using TapAi.Shared.Application.Abstraction;
using AppConc = TapAi.Shared.Application.ResponseObject.Concreate;
using DomainBrand = TapAi.Module.Catalog.Domain.Entity.Brand;

namespace TapAi.Module.Catalog.Persistence.Features.Brand.Commands.UpdateBrand;

public sealed class UpdateBrandHandler(
    ICatalogWriteDbContext writeDb,
    ICatalogReadDbContext readDb)
    : ICommandHandler<UpdateBrandRequest, AppConc.Response<UpdateBrandResponse>>
{
    public async Task<AppConc.Response<UpdateBrandResponse>> HandleAsync(
        UpdateBrandRequest command, CancellationToken ct = default)
    {
        // Oxuma DB-dən track olunmadan yüklə; dəyişdirmədən əvvəl yazma DB-yə attach et
        // ki, change tracker yalnız fərqi (delta) tutsun.
        var brand = await readDb.Brands
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == command.Id, ct);
        if (brand is null)
            return AppConc.Response<UpdateBrandResponse>.NotFound("Brand not found.");

        writeDb.Attach(brand);          // Unchanged vəziyyəti — snapshot qeydə alındı
        brand.UpdateName(command.Name); // EF dəyişikliyi aşkarlayır
        await writeDb.SaveChangesAsync(ct);

        return AppConc.Response<UpdateBrandResponse>.Success(new UpdateBrandResponse(brand.Id, brand.Name));
    }
}