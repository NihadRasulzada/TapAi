namespace TapAi.Module.Catalog.Persistence.Features.Cars.Commands.SubmitDraftPricing;

/// <summary>Draft yayımlandıqda qaytarılır; yeni avtomobilin ID-sini ehtiva edir.</summary>
public sealed record SubmitDraftPricingResponse(
    Guid DraftId,
    int CompletedStep,
    int TotalSteps,
    Guid CarId
);