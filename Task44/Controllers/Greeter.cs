using Microsoft.AspNetCore.Mvc;


namespace Task44.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class Greeter : Controller
    {
        [HttpGet]
        public string Get([FromQuery] string? name)
        {
            var nameRes = string.IsNullOrWhiteSpace(name) ? "Anonymous" : name;
            return $"Hello {nameRes}!" ;

        }
    }


}
