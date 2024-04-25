using API.Data;
using Microsoft.AspNetCore.Mvc;


namespace API.Controllers
{
    [Route("TestController")]
    [ApiController]
    public class TestController : Controller
    {
        private readonly ApplicationDbContext applicationDbContext;
        public TestController(ApplicationDbContext context)
        {
            applicationDbContext = context;
        }
        [HttpGet]
        public IActionResult Index()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var accounts = applicationDbContext.admins.ToList();         
            return Ok(accounts);
        }
    }
}
