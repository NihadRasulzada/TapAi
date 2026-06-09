using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TapAi.API.Extensions;
using TapAi.Module.Catalog.Persistence.Features.Brand.Commands.CreateBrand;
using TapAi.Module.Catalog.Persistence.Features.Brand.Commands.DeleteBrand;
using TapAi.Module.Catalog.Persistence.Features.Brand.Commands.UpdateBrand;
using TapAi.Module.Catalog.Persistence.Features.Brand.Queries.GetAllBrands;
using TapAi.Module.Catalog.Persistence.Features.Brand.Queries.GetBrandById;
using TapAi.Shared.Application.Abstraction;
using TapAi.Shared.Web.Controllers;
using AppConc = TapAi.Shared.Application.ResponseObject.Concreate;

namespace TapAi.API.Controllers;

/// <summary>
/// Avtomobil markaları üçün CRUD əməliyyatları.
/// </summary>
[ApiController]
[Route("api/brands")]
[Produces("application/json")]
public sealed class BrandController(
    ICommandDispatcher commandDispatcher,
    IQueryDispatcher queryDispatcher) : ControllerBase
{
    /// <summary>Bütün markaları ada görə sıralanmış şəkildə qaytarır.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(SuccessResponse<IReadOnlyList<GetAllBrandsResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await queryDispatcher
            .DispatchAsync<GetAllBrandsRequest, AppConc.Response<IReadOnlyList<GetAllBrandsResponse>>>(
                new GetAllBrandsRequest(), ct);
        return this.HandleServiceResponse(result);
    }

    /// <summary>ID-yə görə tək marka qaytarır.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(SuccessResponse<GetBrandByIdResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct)
    {
        var result = await queryDispatcher
            .DispatchAsync<GetBrandByIdRequest, AppConc.Response<GetBrandByIdResponse>>(
                new GetBrandByIdRequest(id), ct);
        return this.HandleServiceResponse(result);
    }

    /// <summary>Yeni marka yaradır.</summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(CreatedResponse<CreateBrandResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create([FromBody] CreateBrandRequest request, CancellationToken ct)
    {
        var result = await commandDispatcher
            .DispatchAsync<CreateBrandRequest, AppConc.Response<CreateBrandResponse>>(request, ct);
        return this.HandleServiceResponse(result);
    }

    /// <summary>Mövcud markanı yeniləyir.</summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(SuccessResponse<UpdateBrandResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateBrandHttpRequest request,
        CancellationToken ct)
    {
        var result = await commandDispatcher
            .DispatchAsync<UpdateBrandRequest, AppConc.Response<UpdateBrandResponse>>(
                new UpdateBrandRequest(id, request.Name), ct);
        return this.HandleServiceResponse(result);
    }

    /// <summary>Markanı silir. Hər hansı model bu markaya istinad edirsə uğursuz olur.</summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ConflictResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken ct)
    {
        var result = await commandDispatcher
            .DispatchAsync<DeleteBrandRequest, AppConc.Response>(
                new DeleteBrandRequest(id), ct);
        return this.HandleServiceResponse(result);
    }
}

/// <summary>Marka adının yenilənməsi üçün yük.</summary>
public sealed class UpdateBrandHttpRequest
{
    /// <example>Toyota</example>
    public string Name { get; set; } = string.Empty;
}