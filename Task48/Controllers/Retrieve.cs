using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

    namespace Task48.Controllers
    {
        [Route("[controller]")]
        [ApiController]
        public class Retrieve : Controller
        {
            static readonly string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

            public record MetaData(string? ImageName, string? Owner, DateTime? CreateTime, DateTime? ModifiedTime);


            [HttpGet("images-bytes")]
            public IActionResult Get([FromQuery] string? FileName, [FromQuery] string? FileOwner)
            {


                try {
                    if (string.IsNullOrWhiteSpace(FileName) || string.IsNullOrWhiteSpace(FileOwner))
                        return BadRequest("BadRequest, FileName and OwnerName are REQUIRED!");

                    var imageName = Path.GetFileName(FileName);
                    var imagePath = Path.Combine(folderPath, imageName);
                    var metaPath = imagePath + ".json";

                    if (!System.IO.File.Exists(imagePath))
                        return NotFound("BadRequest, Image File NOT found!");
                    if (!System.IO.File.Exists(metaPath))
                        return NotFound("BadRequest, MetaData File NOT found!");

                    var metaJson = System.IO.File.ReadAllText(metaPath);
                    var metaData = JsonSerializer.Deserialize<MetaData>(metaJson);

                    if (metaData is null)
                        return BadRequest("BadRequest, MetaData is NULL!");

                    if (!string.Equals(metaData.Owner, FileOwner, StringComparison.OrdinalIgnoreCase)) // I replaced to this way since i noticed that the named COULD BE CASE INSENSITIVE
                        return Forbid("BadRequest, OwnerName does NOT match!");

                    var imageBytes = System.IO.File.ReadAllBytes(imagePath);


                    string extension = Path.GetExtension(imageName);



                    return File(imageBytes, (string.Equals(extension, ".jpg", StringComparison.OrdinalIgnoreCase) ? "image/jpg" : "image/jpeg"), imageName);



                }
                catch
                {
                    return StatusCode(500, "Internal Server Error occurred while processing the request.");
                }

            }
        }
    }
