namespace GoldenPagesUz.Models.YellowPages;

public class YpCategoryCompany
{
    public string CategoryUrl { get; set; }
    public long Count { get; set; }
    public List<CompanyModel> Companies { get; set; }
}