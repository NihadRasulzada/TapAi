using Microsoft.AspNetCore.Http;

namespace TapAi.API.Controllers.Requests;

/// <summary>Multipart form body for the images step.</summary>
public sealed class SubmitDraftImagesHttpRequest
{
    /// <summary>One or more car images (JPEG, PNG, WebP or GIF).</summary>
    public IFormFileCollection? Images { get; set; }
}