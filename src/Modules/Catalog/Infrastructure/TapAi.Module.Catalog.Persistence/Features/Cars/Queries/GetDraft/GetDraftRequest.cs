using TapAi.Shared.Application.Abstraction;
using AppConc = TapAi.Shared.Application.ResponseObject.Concreate;

namespace TapAi.Module.Catalog.Persistence.Features.Cars.Queries.GetDraft;

public sealed record GetDraftRequest(Guid DraftId, Guid RequesterId) : IQuery<AppConc.Response<GetDraftResponse>>;
