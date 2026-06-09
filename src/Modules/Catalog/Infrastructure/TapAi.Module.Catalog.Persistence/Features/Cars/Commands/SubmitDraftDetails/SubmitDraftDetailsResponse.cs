namespace TapAi.Module.Catalog.Persistence.Features.Cars.Commands.SubmitDraftDetails;

/// <summary>Detallar saxlanıldıqdan sonra qaytarılır; klienti qiymət addımına yönəldir.</summary>
public sealed record SubmitDraftDetailsResponse(
    Guid DraftId,
    int CompletedStep,
    int TotalSteps,
    string NextStepKey
);