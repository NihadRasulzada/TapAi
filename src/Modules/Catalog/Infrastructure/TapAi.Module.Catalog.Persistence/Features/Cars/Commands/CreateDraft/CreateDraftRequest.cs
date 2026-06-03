using TapAi.Shared.Application.Abstraction;
using AppConc = TapAi.Shared.Application.ResponseObject.Concreate;

namespace TapAi.Module.Catalog.Persistence.Features.Cars.Commands.CreateDraft;

public sealed record CreateDraftRequest(Guid SellerId) : ICommand<AppConc.Response<CreateDraftResponse>>;
