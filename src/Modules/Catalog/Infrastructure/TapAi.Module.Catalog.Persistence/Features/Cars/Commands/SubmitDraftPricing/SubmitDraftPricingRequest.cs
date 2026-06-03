using TapAi.Shared.Application.Abstraction;
using AppConc = TapAi.Shared.Application.ResponseObject.Concreate;

namespace TapAi.Module.Catalog.Persistence.Features.Cars.Commands.SubmitDraftPricing;

public sealed record SubmitDraftPricingRequest(
    Guid DraftId,
    int Price,
    string Description,
    Guid RequesterId
) : ICommand<AppConc.Response<SubmitDraftPricingResponse>>;
