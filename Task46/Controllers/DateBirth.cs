using Microsoft.AspNetCore.Mvc;

namespace Task46.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BirthDate : Controller
    {
        [HttpPost]
        public string Get([FromForm] string? name, [FromForm] int? years, [FromForm] int? months, [FromForm] int? days)
        {
            var nameRes = string.IsNullOrWhiteSpace(name) ? "Anonymous" : name;

            if (years is not null && months is not null && days is not null)
            {
                

                int ageY = (System.DateTime.Now.Year - (int)years);

                if (System.DateTime.Now.Month < months || System.DateTime.Now.Month == months && System.DateTime.Now.Day < days)
                {
                    ageY--;
                }

                return $"Hello {nameRes}. Your age is {ageY} Years!";


            }
            else
            {
                return $"Hello {nameRes}, I can’t calculate your age without knowing your birthdate!";
            }


        }
    }
}


