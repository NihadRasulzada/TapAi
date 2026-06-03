using TapAi.Module.Catalog.Domain.Enum;

namespace TapAi.Module.Catalog.Persistence.Features.Cars.Queries.GetDraft;

public sealed record GetDraftResponse(
    Guid DraftId,
    CarDraftStatus Status,
    int CurrentStep,
    int TotalSteps
);