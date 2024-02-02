using GoldenPagesUz.Models;
using GoldenPagesUz.Services;
using Microsoft.AspNetCore.Mvc;

namespace GoldenPagesUz.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CompaniesController : ControllerBase
{
    private readonly ICompanyService _companyService;

    public CompaniesController(ICompanyService companyService)
    {
        _companyService = companyService;
    }

    [HttpGet("category/{categoryId:int}/excel")]
    public async Task<ActionResult> Get(int categoryId)
    {
        Console.WriteLine(DateTime.Now.ToString("O"));

        var excelFile = await _companyService.GetExcelByCategoryIdAsync(categoryId);

        Console.WriteLine(DateTime.Now.ToString("O") + "\n");

        return File(
            excelFile.FileBytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            excelFile.FileName);
    }
}