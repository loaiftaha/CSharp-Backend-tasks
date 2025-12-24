using Microsoft.AspNetCore.Mvc;

namespace Task45.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BirthDate : Controller
    {
        [HttpGet]
        public string Get([FromQuery] string? name, [FromQuery] string? years, [FromQuery] string? months, [FromQuery] string? days)
        {
            var nameRes = string.IsNullOrWhiteSpace(name) ? "Anonymous" : name;

            if (!string.IsNullOrWhiteSpace(years) && !string.IsNullOrWhiteSpace(months) && !string.IsNullOrWhiteSpace(days))
            {
                var yearsRes = int.Parse(string.IsNullOrWhiteSpace(years) ? "0" : years);
                var monthsRes = int.Parse(string.IsNullOrWhiteSpace(months) ? "0" : months);
                int daysRes = int.Parse(string.IsNullOrWhiteSpace(days) ? "0" : days);

                int ageY = (System.DateTime.Now.Year - yearsRes);
                
                if(System.DateTime.Now.Month < monthsRes || System.DateTime.Now.Month == monthsRes && System.DateTime.Now.Day < daysRes)
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


