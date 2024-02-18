using System.Text.RegularExpressions;
using GoldenPagesUz.Data.Entities;
using GoldenPagesUz.Models;
using GoldenPagesUz.Models.YellowPages;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;

namespace GoldenPagesUz.Services;

public class YpService : IYpService
{
    const string RubricsCategoriesClassName = "rubricsCategories";
    const string ParentCategoriesClassName = "col-md-4";
    const string BaseUrl = "https://yellowpages.uz";

    int IdNumber = 1;

    private readonly IWebHostEnvironment _hostingEnvironment;

    public YpService(IWebHostEnvironment hostingEnvironment)
    {
        _hostingEnvironment = hostingEnvironment;
    }

    public Task<List<Category>> GetSubCategoriesByCategoryUrlAsync(IWebDriver driver, string categoryUrl, long? parentCategoryId = null)
    {
        var subCategories = new List<Category>();

        driver.Navigate().GoToUrl(categoryUrl);

        try
        {
            var divRubricsCategories = driver.FindElement(By.ClassName(RubricsCategoriesClassName));
            var aElements = divRubricsCategories.FindElements(By.TagName("a"));

            foreach (var aElement in aElements)
            {
                try
                {
                    var href = aElement.GetAttribute("href");
                    if (!string.IsNullOrWhiteSpace(href))
                    {
                        subCategories.Add(new Category
                        {
                            Id = IdNumber++,
                            Name = aElement.Text,
                            Url = href,
                            ParentCategoryId = parentCategoryId
                        });
                    }

                }
                catch (StaleElementReferenceException e)
                { }
            }
        }
        catch (NoSuchElementException)
        {
            return Task.FromResult(new List<Category>());
        }

        return Task.FromResult(subCategories);
    }

    public async Task<List<YpCategoryCompany>> GetCompaniesByCategoryUrlAsync(string categoryUrl)
    {
        var driver = new EdgeDriver();

        var subCategories = await GetSubCategoriesByCategoryUrlAsync(driver, categoryUrl);
        if (subCategories.Count == 0)
        {
            subCategories.Add(new Category
            {
                Name = string.Empty,
                Url = categoryUrl
            });
        }
        
        var ypCategoryCompanies = new List<YpCategoryCompany>();
        foreach (var subCategory in subCategories)
        {
            ypCategoryCompanies.Add(new YpCategoryCompany
            {
                CategoryUrl = subCategory.Url,
                Companies = await GetCompaniesByCategoryUrlAsync(driver, subCategory.Url)
            });
        }

        driver.Quit();
        return ypCategoryCompanies;
    }

    public async Task<ExcelFileModel> GetCompaniesByCategoryUrlToExcelAsync(string categoryUrl)
    {
        var categoryCompanies = await GetCompaniesByCategoryUrlAsync(categoryUrl);

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

            var row = 1;

            foreach (var ypCategoryCompany in categoryCompanies)
            {
                worksheet.Cells[row, 1, row, 6].Merge = true;
                worksheet.Row(row).Style.Font.Bold = true;
                worksheet.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[row, 1].Value = ypCategoryCompany.CategoryUrl;

                row++;

                worksheet.Cells[row, 1].Value = "№";
                worksheet.Cells[row, 2].Value = "Название";
                worksheet.Cells[row, 3].Value = "Адрес";
                worksheet.Cells[row, 4].Value = "Телефон";
                worksheet.Cells[row, 5].Value = "Адрес в яндекс картах";
                worksheet.Cells[row, 6].Value = "Ссылка в yellowpages.uz";

                row++;

                for (var i = 0; i < ypCategoryCompany.Companies.Count; i++)
                {
                    var company = ypCategoryCompany.Companies[i];

                    worksheet.Cells[row, 1].Value = i + 1;
                    worksheet.Cells[row, 2].Value = company.Name;
                    worksheet.Cells[row, 3].Value = company.Address;
                    worksheet.Cells[row, 4].Value = company.Telephone;
                    worksheet.Cells[row, 5].Value = company.YandexMapUrl;
                    worksheet.Cells[row, 6].Value = company.Url;

                    row++;
                }

                row++;
            }

            return new ExcelFileModel
            {
                FileBytes = await package.GetAsByteArrayAsync(),
                FileName = $"yp {DateTime.UtcNow.AddHours(5):yyyy-MM-dd hh-mm-ss}.xlsx"
            };
        }
    }

    public async Task<List<Category>> GetCategoriesAsync()
    {
        var driver = new EdgeDriver();
        driver.Navigate().GoToUrl(BaseUrl);

        var categories = new List<Category>();
        var result = new List<Category>();
        var parentCategoriesElements = driver.FindElements(By.ClassName(ParentCategoriesClassName));

        foreach (var parentCategoriesElement in parentCategoriesElements)
        {
            var aTags = parentCategoriesElement.FindElements(By.TagName("a"));
            var tag = aTags[1];

            var href = GetAttribute(tag, "href");
            var name = tag.Text;

            categories.Add(new Category
            {
                Id = IdNumber++,
                Name = name,
                Url = href,
            });

            Console.WriteLine($"{href} {name}");
        }

        result.AddRange(categories);

        foreach (var category in categories)
        {
            var subCategories = await GetSubCategoriesByCategoryUrlAsync(driver, category.Url, category.Id);
            result.AddRange(subCategories);
        }

        driver.Quit();

        var path = Path.Combine(_hostingEnvironment.ContentRootPath, "wwwroot", "categories.json");
        var json = JsonConvert.SerializeObject(result, Formatting.Indented);

        await File.WriteAllTextAsync(path, json);

        return result;
    }

    private async Task<List<CompanyModel>> GetCompaniesByCategoryUrlAsync(IWebDriver driver, string categoryUrl)
    {
        int pageNumber = 1, pageSize = 50;
        var companies = new List<CompanyModel>();

        while (true)
        {
            var url = $"{categoryUrl}?pagenumber={pageNumber}&pagesize={pageSize}";

            driver.Navigate().GoToUrl(url);

            var scriptElements = driver.FindElements(By.CssSelector("script[type=\"application/ld+json\"]"));
            var scriptElement = scriptElements.LastOrDefault();

            var content = scriptElement != null ? GetAttribute(scriptElement, "innerHTML")?.Trim() : string.Empty;

            //number of times "position" word is found in the json content string
            var count = 0;
            var match = Regex.Match(content ?? string.Empty, "position");
            while (match.Success)
            {
                count++;
                if (count > 1)
                    break;

                match = match.NextMatch();
            }
            
            if (count == 0)
            {
                var companiesByTags = await GetCompaniesByTagsAsync(driver, url);
                companies.AddRange(companiesByTags);

                if (companiesByTags.Count < pageSize)
                    break;
            }
            else if (count == 1)
            {
                //TODO: parser boshqa bo'ladi
                var ypJsonParser = JsonConvert.DeserializeObject<YpJsonParser<MainEntityV2>>(content);

                companies.Add(new CompanyModel(ypJsonParser.MainEntity.ItemListElement.Item));
                break;
            }
            else
            {
                var ypJsonParser = JsonConvert.DeserializeObject<YpJsonParser<MainEntityV1>>(content);

                companies.AddRange(ypJsonParser.MainEntity.ItemListElement.Select(x => new CompanyModel(x.Item)).ToList());

                if (ypJsonParser.MainEntity.ItemListElement.Count < pageSize)
                    break;
            }

            pageNumber++;
        }

        return companies;
    }

    private Task<List<CompanyModel>> GetCompaniesByTagsAsync(IWebDriver driver, string url)
    {
        var companies = new List<CompanyModel>();
        var companiesElements = driver.FindElements(By.ClassName("organizationBlock"));

        foreach (var companiesElement in companiesElements)
        {
            //name, address, telephone, yandexMapUrl, Url

            //name, Url
            var aLinkOrgName = FindElementByWebElement(companiesElement, By.ClassName("organizationName"));
            
            //address
            var inputAddress = FindElementByWebElement(companiesElement, By.ClassName("fullAddress"));

            //telephone
            string? aOnClick = null;
            var pPhoneElement = FindElementByWebElement(companiesElement, By.ClassName("text16"));

            if (pPhoneElement != null)
            {
                var aPPhoneElement = FindElementByWebElement(pPhoneElement, By.TagName("a"));
                if (aPPhoneElement != null)
                    aOnClick = GetAttribute(aPPhoneElement, "onclick");
            }

            var ypItem = new YpItem
            {
                Name = aLinkOrgName?.Text,
                SameAs = aLinkOrgName?.GetAttribute("href"),
                Address = new YpAddress
                {
                    StreetAddress = inputAddress != null ? GetAttribute(inputAddress, "value") : null
                },
                Telephone = aOnClick != null ? SeparatePhoneNumber(aOnClick) : null
            };

            companies.Add(new CompanyModel(ypItem));
        }

        return Task.FromResult(companies);
    }

    private string SeparatePhoneNumber(string aOnClick)
    {
        var start = aOnClick.IndexOf(',');
        var end = aOnClick.LastIndexOf(',');

        aOnClick = aOnClick.Substring(start + 1, end - start - 1);

        aOnClick = aOnClick.Replace("'", "").Trim();

        return aOnClick;
    }

    private IWebElement? FindElementByWebElement(IWebElement element, By by)
    {
        try
        {
            return element.FindElement(by);
        }
        catch (NoSuchElementException e)
        {
            return null;
        }
    }

    private string? GetAttribute(IWebElement element, string attributeName)
    {
        try
        {
            return element.GetAttribute(attributeName);
        }
        catch (StaleElementReferenceException e)
        {
            return null;
        }
    }
}