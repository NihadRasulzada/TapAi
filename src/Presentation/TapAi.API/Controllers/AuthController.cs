using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TapAi.API.Extensions;
using TapAi.Module.Identity.Persistence.Features.Auth.Commands.BlockUser;
using TapAi.Module.Identity.Persistence.Features.Auth.Commands.ChangePassword;
using TapAi.Module.Identity.Persistence.Features.Auth.Commands.Login;
using TapAi.Module.Identity.Persistence.Features.Auth.Commands.RefreshToken;
using TapAi.Module.Identity.Persistence.Features.Auth.Commands.RegisterStart;
using TapAi.Module.Identity.Persistence.Features.Auth.Commands.RegisterVerify;
using TapAi.Module.Identity.Persistence.Features.Auth.Commands.UnblockUser;
using TapAi.Shared.Application.Abstraction;
using TapAi.Shared.Web.Controllers;
using AppConc = TapAi.Shared.Application.ResponseObject.Concreate;

namespace TapAi.API.Controllers;

[ApiController]
[Route("api/auth")]
[Produces("application/json")]
public sealed class AuthController(
    ICommandDispatcher commandDispatcher,
    ICurrentUser currentUser) : ControllerBase
{
    /// <summary>
    /// Qeydiyyat prosesinin 1-ci addımı: ad, soyad, telefon, şifrə qəbul edir,
    /// OTP göndərir və registration token qaytarır.
    /// </summary>
    [HttpPost("register/start")]
    [ProducesResponseType(typeof(SuccessResponse<RegisterStartResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RegisterStart(
        [FromBody] RegisterStartRequest request, CancellationToken ct)
    {
        var result = await commandDispatcher
            .DispatchAsync<RegisterStartRequest, AppConc.Response<RegisterStartResponse>>(request, ct);
        return this.HandleServiceResponse(result);
    }

    /// <summary>
    /// Qeydiyyat prosesinin 2-ci addımı: OTP təsdiqləyir və istifadəçini yaradır.
    /// </summary>
    [HttpPost("register/verify")]
    [ProducesResponseType(typeof(SuccessResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RegisterVerify(
        [FromBody] RegisterVerifyRequest request, CancellationToken ct)
    {
        var result = await commandDispatcher
            .DispatchAsync<RegisterVerifyRequest, AppConc.Response>(request, ct);
        return this.HandleServiceResponse(result);
    }

    /// <summary>
    /// İstifadəçi girişi həyata keçirir və token qaytarır.
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(SuccessResponse<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request, CancellationToken ct)
    {
        var result = await commandDispatcher
            .DispatchAsync<LoginRequest, AppConc.Response<LoginResponse>>(request, ct);
        return this.HandleServiceResponse(result);
    }

    /// <summary>
    /// Köhnə refresh token ilə yeni access və refresh token alır.
    /// </summary>
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(SuccessResponse<RefreshTokenResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken(
        [FromBody] RefreshTokenRequest request, CancellationToken ct)
    {
        var result = await commandDispatcher
            .DispatchAsync<RefreshTokenRequest, AppConc.Response<RefreshTokenResponse>>(request, ct);
        return this.HandleServiceResponse(result);
    }

    /// <summary>
    /// Autentifikasiya olunmuş istifadəçinin şifrəsini dəyişir.
    /// </summary>
    [HttpPost("change-password")]
    [Authorize]
    [ProducesResponseType(typeof(SuccessResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordHttpBody body, CancellationToken ct)
    {
        var command = new ChangePasswordRequest(
            currentUser.Id,
            body.CurrentPassword,
            body.NewPassword,
            body.ConfirmPassword);
        var result = await commandDispatcher
            .DispatchAsync<ChangePasswordRequest, AppConc.Response>(command, ct);
        return this.HandleServiceResponse(result);
    }

    /// <summary>
    /// Müəyyən istifadəçini bloklayır.
    /// </summary>
    [HttpPost("users/{userId:guid}/block")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(SuccessResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> BlockUser(
        [FromRoute] Guid userId,
        [FromBody] BlockUserHttpBody body,
        CancellationToken ct)
    {
        var command = new BlockUserRequest(userId, body.DurationSeconds);
        var result = await commandDispatcher
            .DispatchAsync<BlockUserRequest, AppConc.Response>(command, ct);
        return this.HandleServiceResponse(result);
    }

    /// <summary>
    /// Bloklanmış istifadəçini blokdan çıxarır.
    /// </summary>
    [HttpPost("users/{userId:guid}/unblock")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(SuccessResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UnblockUser(
        [FromRoute] Guid userId, CancellationToken ct)
    {
        var result = await commandDispatcher
            .DispatchAsync<UnblockUserRequest, AppConc.Response>(
                new UnblockUserRequest(userId), ct);
        return this.HandleServiceResponse(result);
    }
}

public sealed record ChangePasswordHttpBody(
    string CurrentPassword,
    string NewPassword,
    string ConfirmPassword
);

public sealed record BlockUserHttpBody(int DurationSeconds);
