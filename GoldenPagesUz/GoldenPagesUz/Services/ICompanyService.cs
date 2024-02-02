using GoldenPagesUz.Models;

namespace GoldenPagesUz.Services;

public interface ICompanyService
{
    Task<ExcelFileModel> GetExcelByCategoryIdAsync(int categoryId);
    Task<List<Company>> GetCompaniesByCategoryIdAsync(int categoryId);
}