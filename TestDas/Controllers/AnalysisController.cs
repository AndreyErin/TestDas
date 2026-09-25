using Microsoft.AspNetCore.Mvc;

namespace TestDas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnalysisController : ControllerBase
    {
        [HttpGet]
        public string Index()
        {
            return "!!!";
        }
    }
}
