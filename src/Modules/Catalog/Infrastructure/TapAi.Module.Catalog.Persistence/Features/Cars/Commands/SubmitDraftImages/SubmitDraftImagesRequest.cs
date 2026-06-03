using TapAi.Shared.Application.Abstraction;
using TapAi.Shared.Contracts.Dtos;
using AppConc = TapAi.Shared.Application.ResponseObject.Concreate;

namespace TapAi.Module.Catalog.Persistence.Features.Cars.Commands.SubmitDraftImages;

public sealed record SubmitDraftImagesRequest(
    Guid DraftId,
    IReadOnlyList<ImageData> Images,
    Guid RequesterId
) : ICommand<AppConc.Response<SubmitDraftImagesResponse>>;
