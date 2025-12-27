using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Task48.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class Update : Controller
    {
        static readonly string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

        public record MetaData(string? ImageName, string? Owner, DateTime? CreateTime, DateTime? ModifiedTime);

        public class UpdateImage
        {
            public IFormFile? ImageFile { get; set; }
            public string? Owner { get; set; }
        }


        [HttpPost]
        public async Task<IActionResult> Post([FromForm] UpdateImage Image )
        {
            IFormFile? image = Image.ImageFile;
            string? owner = Image.Owner;

            try
            {
                if (image is null || image.Length == 0)
                {
                    return BadRequest("BadRequest, No file uploaded.");
                }
                string fileName = image.FileName;

                var extension = Path.GetExtension(fileName).ToLowerInvariant();
                if (extension != ".jpg" && extension != ".jpeg")
                {
                    return BadRequest(extension + "BadRequest, isn't supported for this task, please uplode .jpg ONLY");
                }
                if (string.IsNullOrWhiteSpace(owner))
                {
                    return BadRequest("BadRequest, Owner field is REQUIRED for the task");
                }


                var imageName = Path.GetFileName(fileName);
                var imagePath = Path.Combine(folderPath, imageName);
                var metaPath = imagePath + ".json";

                if (!System.IO.File.Exists(imagePath))
                    return BadRequest("BadRequest, Image File NOT found to UPDATE!");
                if (!System.IO.File.Exists(metaPath))
                    return BadRequest("BadRequest, MetaData File NOT found!");

                var metaJson = System.IO.File.ReadAllText(metaPath);
                MetaData? metaData = JsonSerializer.Deserialize<MetaData>(metaJson);

                if (metaData is null)
                    return BadRequest("BadRequest, MetaData is INVALID!");

                if (!string.Equals(metaData.Owner, owner, StringComparison.OrdinalIgnoreCase)) // I replaced to this way since i noticed that the named COULD BE CASE INSENSITIVE
                    return Forbid("Forbidden, OwnerName does NOT match!");




                await using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);

                }

                DateTime now = DateTime.Now;
                MetaData updatedMeta = metaData with { ModifiedTime = now };

                await System.IO.File.WriteAllTextAsync(metaPath, JsonSerializer.Serialize(updatedMeta));

                return Ok("UPDATED!");
            }
            catch
            {
                return StatusCode(500, "Internal Server Error occurred while processing the request.");
            }

        }
    }
}
