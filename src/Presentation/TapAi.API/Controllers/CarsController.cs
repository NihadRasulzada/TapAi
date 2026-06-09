using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TapAi.API.Controllers.Requests;
using TapAi.API.Extensions;
using TapAi.Module.Catalog.Persistence.Features.Cars.Commands.CreateDraft;
using TapAi.Module.Catalog.Persistence.Features.Cars.Commands.SubmitDraftDetails;
using TapAi.Module.Catalog.Persistence.Features.Cars.Commands.SubmitDraftImages;
using TapAi.Module.Catalog.Persistence.Features.Cars.Commands.SubmitDraftPricing;
using TapAi.Module.Catalog.Persistence.Features.Cars.Queries.GetDraft;
using TapAi.Module.Catalog.Persistence.Features.Cars.Queries.GetCarConfig;
using TapAi.Shared.Application.Abstraction;
using TapAi.Shared.Web.Controllers;
using AppConc = TapAi.Shared.Application.ResponseObject.Concreate;

namespace TapAi.API.Controllers;

/// <summary>
/// Backend tərəfindən idarə olunan çoxaddımlı avtomobil elanı onboarding-i.
/// </summary>
[ApiController]
[Route("api/cars")]
[Produces("application/json")]
[Authorize]
public sealed class CarsController(
    ICommandDispatcher commandDispatcher,
    IQueryDispatcher queryDispatcher,
    ICurrentUser currentUser) : ControllerBase
{
    private Guid CallerId => currentUser.Id;

    /// <summary>
    /// Onboarding axını üçün addım təriflərini qaytarır (açıq/public).
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(SuccessResponse<GetCarConfigResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetConfig(CancellationToken ct)
    {
        var result = await queryDispatcher
            .DispatchAsync<GetCarConfigRequest, AppConc.Response<GetCarConfigResponse>>(
                new GetCarConfigRequest(), ct);
        return this.HandleServiceResponse(result);
    }

    /// <summary>
    /// Yeni draft yaradır və onun ID-sini qaytarır.
    /// </summary>
    [HttpPost("drafts")]
    [ProducesResponseType(typeof(CreatedResponse<CreateDraftResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ServerErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateDraft(CancellationToken ct)
    {
        var result = await commandDispatcher
            .DispatchAsync<CreateDraftRequest, AppConc.Response<CreateDraftResponse>>(
                new CreateDraftRequest(CallerId), ct);
        return this.HandleServiceResponse(result);
    }

    /// <summary>
    /// Draft-ın cari vəziyyətini qaytarır (davam etdirmə dəstəyi).
    /// </summary>
    [HttpGet("drafts/{draftId:guid}")]
    [ProducesResponseType(typeof(SuccessResponse<GetDraftResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDraft([FromRoute] Guid draftId, CancellationToken ct)
    {
        var result = await queryDispatcher
            .DispatchAsync<GetDraftRequest, AppConc.Response<GetDraftResponse>>(
                new GetDraftRequest(draftId, CallerId), ct);
        return this.HandleServiceResponse(result);
    }

    /// <summary>
    /// Addım 1 — avtomobil şəkillərini yüklə.
    /// </summary>
    [HttpPost("drafts/{draftId:guid}/images")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(SuccessResponse<SubmitDraftImagesResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SubmitImages(
        [FromRoute] Guid draftId,
        [FromForm] SubmitDraftImagesHttpRequest request,
        CancellationToken ct)
    {
        var command = new SubmitDraftImagesRequest(draftId, await request.Images.ToImageDataAsync(ct), CallerId);
        var result = await commandDispatcher
            .DispatchAsync<SubmitDraftImagesRequest, AppConc.Response<SubmitDraftImagesResponse>>(command, ct);
        return this.HandleServiceResponse(result);
    }

    /// <summary>
    /// Addım 2 — avtomobil detallarını göndər (marka, model, il, yürüş, yanacaq növü, transmissiya).
    /// </summary>
    [HttpPost("drafts/{draftId:guid}/details")]
    [ProducesResponseType(typeof(SuccessResponse<SubmitDraftDetailsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SubmitDetails(
        [FromRoute] Guid draftId,
        [FromBody] SubmitDraftDetailsHttpRequest request,
        CancellationToken ct)
    {
        var command = new SubmitDraftDetailsRequest(
            draftId, request.BrandId, request.ModelId, request.Year,
            request.FuelType, request.TransmissionType, request.Mileage,
            CallerId);
        var result = await commandDispatcher
            .DispatchAsync<SubmitDraftDetailsRequest, AppConc.Response<SubmitDraftDetailsResponse>>(command, ct);
        return this.HandleServiceResponse(result);
    }

    /// <summary>
    /// Addım 3 — qiymət və təsviri göndər, avtomobil elanını yayımlayır.
    /// </summary>
    [HttpPost("drafts/{draftId:guid}/pricing")]
    [ProducesResponseType(typeof(CreatedResponse<SubmitDraftPricingResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SubmitPricing(
        [FromRoute] Guid draftId,
        [FromBody] SubmitDraftPricingHttpRequest request,
        CancellationToken ct)
    {
        var command = new SubmitDraftPricingRequest(draftId, request.Price, request.Description, CallerId);
        var result = await commandDispatcher
            .DispatchAsync<SubmitDraftPricingRequest, AppConc.Response<SubmitDraftPricingResponse>>(command, ct);
        return this.HandleServiceResponse(result);
    }
}
