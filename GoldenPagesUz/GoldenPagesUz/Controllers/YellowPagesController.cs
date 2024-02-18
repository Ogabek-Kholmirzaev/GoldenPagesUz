using GoldenPagesUz.Services;
using Microsoft.AspNetCore.Mvc;

namespace GoldenPagesUz.Controllers;

[Route("api/[controller]")]
[ApiController]
public class YellowPagesController : ControllerBase
{
    private readonly IYpService _ypService;

    public YellowPagesController(IYpService ypService)
    {
        _ypService = ypService;
    }

    [HttpGet("category/companies/excel")]
    public async Task<ActionResult> GetCompaniesByCategoryUrlAsync([FromQuery] string categoryUrl)
    {
        Console.WriteLine("\n" + DateTime.Now.ToString("O") + "\n");

        var excelFile = await _ypService.GetCompaniesByCategoryUrlToExcelAsync(categoryUrl);

        Console.WriteLine("\n" + DateTime.Now.ToString("O") + "\n");

        return File(
            excelFile.FileBytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            excelFile.FileName);
    }

    [HttpGet("categories")]
    public async Task<ActionResult> GetCategoriesAsync()
    {
        var categories = await _ypService.GetCategoriesAsync();
        return Ok(categories);
    }
}