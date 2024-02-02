using Microsoft.AspNetCore.Mvc;

namespace GoldenPagesUz.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        [HttpGet("{categoryId:int}")]
        public IActionResult Get(int categoryId) => Ok("Ok ...");
    }
}