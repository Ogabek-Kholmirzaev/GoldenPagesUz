using GoldenPagesUz.Models;
using GoldenPagesUz.Models.YellowPages;
using OpenQA.Selenium;

namespace GoldenPagesUz.Services;

public interface IYpService
{

    Task<List<string>> GetSubCategoriesByCategoryUrlAsync(IWebDriver driver, string categoryUrl);
    Task<List<YpCategoryCompany>> GetCompaniesByCategoryUrlAsync(string categoryUrl);
    Task<ExcelFileModel> GetCompaniesByCategoryUrlToExcelAsync(string categoryUrl);
}