using System.ComponentModel.DataAnnotations.Schema;

namespace GoldenPagesUz.Data.Entities;

public class Company
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string Telephone { get; set; }
    public string YandexMapUrl { get; set; }
    public string Url { get; set; }
    
    public long CategoryId { get; set; }
    [ForeignKey(nameof(CategoryId))]
    public Category Category { get; set; }
}