using GoldenPagesUz.Data.Entities;
using GoldenPagesUz.Models;
using GoldenPagesUz.Models.YellowPages;
using OpenQA.Selenium;

namespace GoldenPagesUz.Services;

public interface IYpService
{

    Task<List<Category>> GetSubCategoriesByCategoryUrlAsync(IWebDriver driver, string categoryUrl, long? parentCategoryId = null);
    Task<List<YpCategoryCompany>> GetCompaniesByCategoryUrlAsync(string categoryUrl);
    Task<ExcelFileModel> GetCompaniesByCategoryUrlToExcelAsync(string categoryUrl);
    Task<List<Category>> GetCategoriesAsync();
}