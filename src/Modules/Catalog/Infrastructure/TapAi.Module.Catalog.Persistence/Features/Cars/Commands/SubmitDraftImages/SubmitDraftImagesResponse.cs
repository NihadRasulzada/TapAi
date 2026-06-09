namespace TapAi.Module.Catalog.Persistence.Features.Cars.Commands.SubmitDraftImages;

/// <summary>Şəkillər yükləndikdən sonra qaytarılır; klienti detallar addımına yönəldir.</summary>
public sealed record SubmitDraftImagesResponse(
    Guid DraftId,
    int CompletedStep,
    int TotalSteps,
    string NextStepKey
);