namespace Catalog.Products.Models;

public class Variant : Aggregate<Guid>
{
    public string Name { get; private set; } = default!;
    public string NameEng { get; private set; } = default!;
    public VariantDisplayType DisplayType { get; private set; } = VariantDisplayType.Pills;
    public VariantCreationMode CreationMode { get; private set; } = VariantCreationMode.Instant;
    public Guid CompanyId { get; private set; }

    private readonly List<VariantValue> _values = new();
    public IReadOnlyCollection<VariantValue> Values => _values;
    private Variant() { }

    //internal Variant(Guid id, string name, string nameEng,Guid companyId)
    //{
    //    Id = id;
    //    Name = name;
    //    NameEng = nameEng;
    //    CompanyId = companyId;
    //    CreatedAt = DateTime.UtcNow;
    //    //CreatedBy = createdBy;
    //}

    [JsonConstructor]
    public Variant(Guid id, string name, string nameEng, Guid companyId)
    {
        Id = id;
        Name = name;
        NameEng = nameEng;
        CompanyId = companyId;
    }
    public static Variant Create(Guid id, string name, string nameEng, VariantDisplayType displayType, VariantCreationMode creationMode, Guid companyId, string createdBy)
    {
        ValidateVariantSettings(displayType, creationMode);
        return new Variant()
        {
            Id = id,
            Name = name,
            NameEng = nameEng,
            DisplayType = NormalizeDisplayType(displayType),
            CreationMode = NormalizeCreationMode(creationMode),
            CompanyId = companyId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy,

        };
    }

    public void Update(VariantDto variantDto, string modifiedBy)
    {
        Name = variantDto.Name;
        NameEng = variantDto.NameEng;
        ValidateVariantSettings(variantDto.DisplayType, variantDto.CreationMode);
        DisplayType = NormalizeDisplayType(variantDto.DisplayType);
        CreationMode = NormalizeCreationMode(variantDto.CreationMode);
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;

        var activeValues = _values.Where(v => !v.IsDeleted).ToList();
        var activeIds = activeValues.Select(v => v.Id).ToHashSet();

        // Add + Update
        foreach (var v in variantDto.Values)
        {
            if (v.Id == Guid.Empty)
            {
                AddVariantValue(v.Value, v.ValueEng, modifiedBy);
                continue;
            }

            // 🚨 ONLY validate against ACTIVE values
            if (!activeIds.Contains(v.Id))
                throw new Exception($"Invalid or deleted VariantValue Id: {v.Id}");


            var existingValue = activeValues.First(ev => ev.Id == v.Id);
            existingValue.Update(v.Value, v.ValueEng, modifiedBy);
        }

        // Remove
        var dtoIds = variantDto.Values
            .Where(v => v.Id != Guid.Empty)
            .Select(v => v.Id)
            .ToHashSet();

        var valuesToRemove = activeValues
            .Where(ev => !dtoIds.Contains(ev.Id))
            .ToList();

        foreach (var value in valuesToRemove)
        {
            value.Remove(modifiedBy);
        }
    }
    public void Remove(string deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }

    public void AddVariantValue(string value, string valueEng, string createdBy)
    {
        var exists = _values.FirstOrDefault(v => v.Value == value && !v.IsDeleted);
        if (exists != null)
            throw new Exception($"Variant value is already added to this variant: {value}");
        exists = _values.FirstOrDefault(v => v.ValueEng == valueEng && !v.IsDeleted);
        if (exists != null)
            throw new Exception($"Variant value is already added to this variant: {valueEng}");

        var newVariantValue = new VariantValue(Id, value, valueEng, createdBy);//(Guid.NewGuid(), Id, value, valueEng, createdBy);

            //newVariantValue  =VariantValue.Create(Guid.NewGuid(), Id, value, valueEng, createdBy);
        _values.Add(newVariantValue);

    }

    private static void ValidateVariantSettings(VariantDisplayType displayType, VariantCreationMode creationMode)
    {
        if (NormalizeDisplayType(displayType) == VariantDisplayType.MultiCheckbox &&
            NormalizeCreationMode(creationMode) != VariantCreationMode.Never)
            throw new Exception("Multi-checkbox variants must use Never creation mode.");
    }

    private static VariantDisplayType NormalizeDisplayType(VariantDisplayType displayType)
        => displayType == default ? VariantDisplayType.Pills : displayType;

    private static VariantCreationMode NormalizeCreationMode(VariantCreationMode creationMode)
        => creationMode == default ? VariantCreationMode.Instant : creationMode;
}
