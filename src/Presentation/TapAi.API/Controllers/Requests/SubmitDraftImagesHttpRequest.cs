using Microsoft.AspNetCore.Http;

namespace TapAi.API.Controllers.Requests;

/// <summary>Şəkillər addımı üçün multipart form gövdəsi.</summary>
public sealed class SubmitDraftImagesHttpRequest
{
    /// <summary>Bir və ya bir neçə avtomobil şəkli (JPEG, PNG, WebP və ya GIF).</summary>
    public IFormFileCollection? Images { get; set; }
}