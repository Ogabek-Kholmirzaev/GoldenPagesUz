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
        var excelFile = await _ypService.GetCompaniesByCategoryUrlToExcelAsync(categoryUrl);

        return File(
            excelFile.FileBytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            excelFile.FileName);
    }
}