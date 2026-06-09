using MassTransit;
using Microsoft.EntityFrameworkCore;
using TapAi.Module.Media.Domain.Enums;
using TapAi.Module.Media.Persistence.Contexts;
using TapAi.Shared.Contracts.IntegrationEvents;
using MediaEntity = TapAi.Module.Media.Domain.Entity.Media;

namespace TapAi.Module.Media.Persistence.Consumers;

public sealed class CarListingPublishedConsumer(
    IMediaWriteDbContext writeDb,
    IMediaReadDbContext readDb)
    : IConsumer<CarListingPublishedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<CarListingPublishedIntegrationEvent> context)
    {
        var message = context.Message;
        var ct = context.CancellationToken;

        // Oxuma DB-dən track olunmadan yüklə; mutasiyalar saxlanılsın deyə yazma DB-yə attach et.
        var draftMediaItems = await readDb.Medias
            .AsNoTracking()
            .Where(m => m.OwnerId == message.DraftId && m.OwnerType == MediaOwnerType.CarDraft)
            .ToListAsync(ct);

        if (draftMediaItems.Count == 0)
            return;

        writeDb.AttachRange(draftMediaItems);

        foreach (var media in draftMediaItems)
            media.TransferOwnership(message.CarId, MediaOwnerType.Car);

        await writeDb.SaveChangesAsync(ct);
    }
}