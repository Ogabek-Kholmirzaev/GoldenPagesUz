using GoldenPagesUz.Exceptions;
using GoldenPagesUz.Models;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace GoldenPagesUz.Services;

public class CompanyService : ICompanyService
{
    private readonly HttpClient _httpClient;

    public CompanyService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ExcelFileModel> GetExcelByCategoryIdAsync(int categoryId)
    {
        var companies = await GetCompaniesByCategoryIdAsync(categoryId);

        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        using (var package = new ExcelPackage())
        {
            var worksheet = package.Workbook.Worksheets.Add("Companies");

            worksheet.Rows.Height = 20;

            worksheet.Column(2).Width = 40;
            worksheet.Column(3).Width = 40;
            worksheet.Column(4).Width = 40;
            worksheet.Column(5).Width = 40;
            worksheet.Column(6).Width = 40;

            worksheet.Cells[1, 1, 1, 6].Merge = true;
            worksheet.Row(1).Style.Font.Bold = true;
            worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[1, 1].Value = $"Список компаний. https://www.goldenpages.uz/rubrics/?Id={categoryId}";

            worksheet.Cells[2, 1].Value = "№";
            worksheet.Cells[2, 2].Value = "Название";
            worksheet.Cells[2, 3].Value = "Адрес";
            worksheet.Cells[2, 4].Value = "Телефон";
            worksheet.Cells[2, 5].Value = "Адрес в яндекс картах";
            worksheet.Cells[2, 6].Value = "Ссылка в goldpages.uz";

            for (var i = 0; i < companies.Count; i++)
            {
                var company = companies[i];

                worksheet.Cells[i + 3, 1].Value = i + 1;
                worksheet.Cells[i + 3, 2].Value = company.Name;
                worksheet.Cells[i + 3, 3].Value = company.Address;
                worksheet.Cells[i + 3, 4].Value = company.Telephone;
                worksheet.Cells[i + 3, 5].Value = company.YandexMapUrl;
                worksheet.Cells[i + 3, 6].Value = company.Url;
            }

            return new ExcelFileModel()
            {
                FileBytes = await package.GetAsByteArrayAsync(),
                FileName = $"gp-category-{categoryId} {DateTime.UtcNow.AddHours(5):yyyy-MM-dd hh-mm-ss}.xlsx"
            };
        }
    }

    public async Task<List<CompanyModel>> GetCompaniesByCategoryIdAsync(int categoryId)
    {
        var companies = new List<CompanyModel>();
        var page = 1;

        while (true)
        {
            var url = $"https://www.goldenpages.uz/rubrics/?Id={categoryId}&Page={page}";
            var response = await _httpClient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            if (content.Contains("type=\"application/ld+json\"") == false)
            {
                if (page == 1)
                    throw new CategoryNotFoundException(categoryId);

                break;
            }

            var listOfItemListElementJson = SeparateItemListElementFromHtml(content);
            if (string.IsNullOrWhiteSpace(listOfItemListElementJson))
            {
                if (page == 1)
                    throw new CategoryNotFoundException(categoryId);

                break;
            }

            var itemListElements = JsonConvert.DeserializeObject<List<ItemListElement>>(listOfItemListElementJson);
            if (itemListElements?.Count > 0)
                companies.AddRange(itemListElements.Select(itemListElement => new CompanyModel(itemListElement)));

            page++;
        }


        return companies;
    }

    private string SeparateItemListElementFromHtml(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            return string.Empty;

        var startIndex = content.IndexOf("[[");
        var endIndex = content.IndexOf("]]");

        return startIndex == -1 || endIndex == -1
            ? string.Empty
            : content.Substring(startIndex + 1, endIndex - startIndex);
    }
}