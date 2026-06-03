using Microsoft.EntityFrameworkCore;
using TapAi.Module.Catalog.Persistence.Contexts;

using TapAi.Shared.Application.Abstraction;
using AppConc = TapAi.Shared.Application.ResponseObject.Concreate;
using DomainModel = TapAi.Module.Catalog.Domain.Entity.Model;

namespace TapAi.Module.Catalog.Persistence.Features.Model.Commands.UpdateModel;

public sealed class UpdateModelHandler(
    ICatalogWriteDbContext writeDb,
    ICatalogReadDbContext readDb)
    : ICommandHandler<UpdateModelRequest, AppConc.Response<UpdateModelResponse>>
{
    public async Task<AppConc.Response<UpdateModelResponse>> HandleAsync(
        UpdateModelRequest command, CancellationToken ct = default)
    {
        var model = await readDb.Models
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == command.Id, ct);
        if (model is null)
            return AppConc.Response<UpdateModelResponse>.NotFound("Model not found.");

        var brandExists = await readDb.Brands.AnyAsync(b => b.Id == command.BrandId, ct);
        if (!brandExists)
            return AppConc.Response<UpdateModelResponse>.NotFound("Brand not found.");

        writeDb.Attach(model);
        model.Update(command.Name, command.BrandId);
        await writeDb.SaveChangesAsync(ct);

        return AppConc.Response<UpdateModelResponse>.Success(
            new UpdateModelResponse(model.Id, model.Name, model.BrandId));
    }
}