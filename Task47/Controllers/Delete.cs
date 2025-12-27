using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Task47.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Delete : Controller
    {
        static readonly string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");

        public record MetaData(string? ImageName, string? Owner, DateTime? CreateTime, DateTime? ModifiedTime);


        [HttpGet]
        public IActionResult Get([FromQuery] string? FileName, [FromQuery] string? OwnerName)
        {

            try
            {
                if (string.IsNullOrWhiteSpace(FileName) || string.IsNullOrWhiteSpace(OwnerName))
                    return BadRequest("BadRequest, FileName and OwnerName are REQUIRED!");
                
                var imageName = Path.GetFileName(FileName);
                var imagePath = Path.Combine(folderPath, imageName);
                var metaPath = imagePath +  ".json";

                if (!System.IO.File.Exists(imagePath) )
                    return BadRequest("BadRequest, Image File NOT found!");
                if (!System.IO.File.Exists(metaPath))  
                    return BadRequest("BadRequest, MetaData File NOT found!");

                var metaJson = System.IO.File.ReadAllText(metaPath);
                var metaData = JsonSerializer.Deserialize<MetaData>(metaJson);

                if (metaData is null )
                    return BadRequest("BadRequest, MetaData is INVALID!");

                if (metaData.Owner != OwnerName)
                    return Forbid("Forbidden, OwnerName does NOT match!");

                System.IO.File.Delete(imagePath);
                System.IO.File.Delete(metaPath);

                return Ok("Deleted!");

            }
            catch
            {
                return StatusCode(500, "Internal Server Error occurred while processing the request.");
            }
        }
    }
}
