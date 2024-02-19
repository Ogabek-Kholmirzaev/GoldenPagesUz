using System.ComponentModel.DataAnnotations.Schema;

namespace GoldenPagesUz.Data.Entities;

public class Category
{
    public long Id { get; set; }
    public required string Name { get; set; }
    public required string Url { get; set; }
    
    public long? ParentCategoryId { get; set; }
    [ForeignKey(nameof(ParentCategoryId))]
    public Category? ParentCategory { get; set; }

    public List<Category> SubCategories { get; set; }
    public List<Company> Compaies { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow.AddHours(5);
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow.AddHours(5);
}