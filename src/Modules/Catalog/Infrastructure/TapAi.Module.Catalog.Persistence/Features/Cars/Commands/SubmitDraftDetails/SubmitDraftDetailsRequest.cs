using TapAi.Module.Catalog.Domain.Enum;
using TapAi.Shared.Application.Abstraction;
using AppConc = TapAi.Shared.Application.ResponseObject.Concreate;

namespace TapAi.Module.Catalog.Persistence.Features.Cars.Commands.SubmitDraftDetails;

public sealed record SubmitDraftDetailsRequest(
    Guid DraftId,
    Guid BrandId,
    Guid ModelId,
    short Year,
    FuelType FuelType,
    TransmissionType TransmissionType,
    int Mileage,
    Guid RequesterId
) : ICommand<AppConc.Response<SubmitDraftDetailsResponse>>;
