using MassTransit;
using Microsoft.EntityFrameworkCore;
using TapAi.Module.Catalog.Domain.Entity;
using TapAi.Module.Catalog.Domain.Enum;
using TapAi.Module.Catalog.Persistence.Contexts;

using TapAi.Shared.Application.Abstraction;
using TapAi.Shared.Contracts.IntegrationEvents;
using AppConc = TapAi.Shared.Application.ResponseObject.Concreate;

namespace TapAi.Module.Catalog.Persistence.Features.Cars.Commands.SubmitDraftPricing;

public sealed class SubmitDraftPricingHandler(
    ICatalogWriteDbContext writeDb,
    ICatalogReadDbContext readDb,
    IPublishEndpoint publishEndpoint,
    IUserAccessGuard userAccessGuard)
    : ICommandHandler<SubmitDraftPricingRequest, AppConc.Response<SubmitDraftPricingResponse>>
{
    public async Task<AppConc.Response<SubmitDraftPricingResponse>> HandleAsync(
        SubmitDraftPricingRequest command,
        CancellationToken ct = default)
    {
        var draft = await readDb.CarDrafts
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == command.DraftId, ct);
        if (draft is null)
            return AppConc.Response<SubmitDraftPricingResponse>.NotFound("Draft not found.");
        if (!draft.IsOwnedBy(command.RequesterId))
            return AppConc.Response<SubmitDraftPricingResponse>.Forbidden(
                "You do not have access to this draft.");
        if (!await userAccessGuard.IsActiveAsync(command.RequesterId, ct))
            return AppConc.Response<SubmitDraftPricingResponse>.Forbidden(
                "Your account is not allowed to publish listings.");
        if (draft.Status == CarDraftStatus.Completed)
            return AppConc.Response<SubmitDraftPricingResponse>.BadRequest("Draft is already completed.");
        if (draft.CurrentStep != 3)
            return AppConc.Response<SubmitDraftPricingResponse>.BadRequest(
                "Complete the previous steps before submitting pricing.");
        if (draft.BrandId is null || draft.ModelId is null || draft.Year is null)
            return AppConc.Response<SubmitDraftPricingResponse>.BadRequest(
                "Car details are incomplete. Resubmit the details step.");

        var car = new Car(
            draft.SellerId,
            draft.BrandId.Value,
            draft.ModelId.Value,
            draft.Year.Value,
            draft.FuelType!.Value,
            draft.TransmissionType!.Value,
            draft.Mileage!.Value,
            command.Price,
            command.Description);

        writeDb.Attach(draft);
        draft.SetPricing(command.Price, command.Description);
        draft.Complete();
        writeDb.Add(car);
        await writeDb.SaveChangesAsync(ct);

        await publishEndpoint.Publish(
            new CarListingPublishedIntegrationEvent(car.Id, command.DraftId, draft.SellerId), ct);

        return AppConc.Response<SubmitDraftPricingResponse>.Created(
            new SubmitDraftPricingResponse(command.DraftId, 3, 3, car.Id));
    }
}