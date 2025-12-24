using Microsoft.AspNetCore.Mvc;

namespace Task46.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Greeter : Controller
    {
        [HttpPost]
        public string Get([FromForm] string? name)
        {
            var nameRes = string.IsNullOrWhiteSpace(name) ? "Anonymous" : name;
            return $"Hello {nameRes}!";
        }
    }
}
