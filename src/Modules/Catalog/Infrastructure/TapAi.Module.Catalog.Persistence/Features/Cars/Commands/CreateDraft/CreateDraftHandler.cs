using TapAi.Module.Catalog.Domain.Entity;
using TapAi.Module.Catalog.Persistence.Contexts;

using TapAi.Shared.Application.Abstraction;
using AppConc = TapAi.Shared.Application.ResponseObject.Concreate;

namespace TapAi.Module.Catalog.Persistence.Features.Cars.Commands.CreateDraft;

public sealed class CreateDraftHandler(ICatalogWriteDbContext writeDb)
    : ICommandHandler<CreateDraftRequest, AppConc.Response<CreateDraftResponse>>
{
    public async Task<AppConc.Response<CreateDraftResponse>> HandleAsync(
        CreateDraftRequest command,
        CancellationToken ct = default)
    {
        var draft = CarDraft.Create(command.SellerId);
        writeDb.Add(draft);
        await writeDb.SaveChangesAsync(ct);
        return AppConc.Response<CreateDraftResponse>.Created(new CreateDraftResponse(draft.Id));
    }
}