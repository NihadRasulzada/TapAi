using TapAi.Module.Catalog.Domain.Entity;
using TapAi.Module.Catalog.Persistence.Contexts;

using TapAi.Shared.Application.Abstraction;
using AppConc = TapAi.Shared.Application.ResponseObject.Concreate;

namespace TapAi.Module.Catalog.Persistence.Features.Cars.Commands.CreateDraft;

public sealed class CreateDraftHandler(
    ICatalogWriteDbContext writeDb,
    IUserAccessGuard userAccessGuard)
    : ICommandHandler<CreateDraftRequest, AppConc.Response<CreateDraftResponse>>
{
    public async Task<AppConc.Response<CreateDraftResponse>> HandleAsync(
        CreateDraftRequest command,
        CancellationToken ct = default)
    {
        if (!await userAccessGuard.IsActiveAsync(command.SellerId, ct))
            return AppConc.Response<CreateDraftResponse>.Forbidden(
                "Your account is not allowed to create listings.");

        var draft = CarDraft.Create(command.SellerId);
        writeDb.Add(draft);
        await writeDb.SaveChangesAsync(ct);
        return AppConc.Response<CreateDraftResponse>.Created(new CreateDraftResponse(draft.Id));
    }
}