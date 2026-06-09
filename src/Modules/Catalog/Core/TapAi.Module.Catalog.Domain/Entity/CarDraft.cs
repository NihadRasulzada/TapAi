using TapAi.Module.Catalog.Domain.Enum;
using TapAi.Module.Catalog.Domain.Exceptions;
using TapAi.Shared.Domain.Models;

namespace TapAi.Module.Catalog.Domain.Entity;

public class CarDraft : BaseEntity
{
    public Guid SellerId { get; private set; }
    public CarDraftStatus Status { get; private set; }
    public int CurrentStep { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Addım 2 — avtomobil detalları
    public Guid? BrandId { get; private set; }
    public Guid? ModelId { get; private set; }
    public short? Year { get; private set; }
    public FuelType? FuelType { get; private set; }
    public TransmissionType? TransmissionType { get; private set; }
    public int? Mileage { get; private set; }

    // Addım 3 — qiymət
    public int? Price { get; private set; }
    public string? Description { get; private set; }

    // ── Factory ──────────────────────────────────────────────────────────────
    /// <summary>
    /// Yeni draft yaradır. SellerId token-dən gəlir və boş ola bilməz.
    /// </summary>
    public static CarDraft Create(Guid sellerId)
    {
        if (sellerId == Guid.Empty)
            throw new DomainException("SellerId cannot be empty.");
        return new CarDraft(Guid.NewGuid(), sellerId);
    }

    // İki arqumentli konstruktor EF Core-un tək-Guid ctor-u ilə imza toqquşmasının qarşısını alır.
    private CarDraft(Guid id, Guid sellerId) : base(id)
    {
        SellerId = sellerId;
        Status = CarDraftStatus.InProgress;
        CurrentStep = 1;
        CreatedAt = DateTime.UtcNow;
    }

    protected CarDraft(Guid id) : base(id) { } // EF Core materializasiyası

    /// <summary>Draft-ın verilmiş seller-ə aid olub-olmadığını yoxlayır.</summary>
    public bool IsOwnedBy(Guid sellerId) => SellerId == sellerId;

    public void AdvanceStep() => CurrentStep++;

    public void SetDetails(
        Guid brandId,
        Guid modelId,
        short year,
        FuelType fuelType,
        TransmissionType transmissionType,
        int mileage)
    {
        BrandId = brandId;
        ModelId = modelId;
        Year = year;
        FuelType = fuelType;
        TransmissionType = transmissionType;
        Mileage = mileage;
    }

    public void SetPricing(int price, string description)
    {
        Price = price;
        Description = description;
    }

    public void Complete() => Status = CarDraftStatus.Completed;
}
