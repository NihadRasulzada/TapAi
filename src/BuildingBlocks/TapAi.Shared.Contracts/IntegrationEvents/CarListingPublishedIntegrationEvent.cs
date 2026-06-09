namespace TapAi.Shared.Contracts.IntegrationEvents;

public sealed record CarListingPublishedIntegrationEvent(Guid CarId, Guid DraftId, Guid SellerId);