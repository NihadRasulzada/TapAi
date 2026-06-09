namespace TapAi.API.Controllers.Requests;

/// <summary>Addım 3 üçün qiymət və təsvir.</summary>
public sealed class SubmitDraftPricingHttpRequest
{
    /// <example>25000</example>
    public int Price { get; set; }
    /// <example>Yaxşı saxlanılıb, tək sahib, tam servis tarixçəsi.</example>
    public string Description { get; set; } = string.Empty;
}