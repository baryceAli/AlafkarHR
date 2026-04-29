namespace Catalog.Products.Models;

public class Variant : Aggregate<Guid>
{
    public string Name { get; private set; } = default!;
    public string NameEng { get; private set; } = default!;
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
    public static Variant Create(Guid id, string name, string nameEng, Guid companyId, string createdBy)
    {
        return new Variant()
        {
            Id = id,
            Name = name,
            NameEng = nameEng,
            CompanyId = companyId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy,

        };
    }

    public void Update(VariantDto variantDto, string modifiedBy)
    {
        Name = variantDto.Name;
        NameEng = variantDto.NameEng;
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


            //if (!existingIds.Contains(v.Id))
            //    throw new Exception($"Invalid VariantValue Id: {v.Id}");

            //var existingValue = _values.First(ev => ev.Id == v.Id && !ev.IsDeleted);
            //existingValue.Update(v.Value, v.ValueEng, modifiedBy);

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
}
