using TapAi.Shared.Contracts.Dtos;

namespace TapAi.Shared.Contracts.IntegrationEvents;

public sealed record DraftImagesUploadedIntegrationEvent(
    Guid DraftId,
    IReadOnlyList<ImageData> Images
);