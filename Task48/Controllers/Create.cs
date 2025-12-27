using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Task48.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Create : Controller
    {

        static readonly string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");

        public class UploadImage
        {
            public IFormFile? ImageFile { get; set; }
            public string? Owner { get; set; }
        }

        public record MetaData (string? ImageName, string? Owner, DateTime? CreateTime, DateTime? ModifiedTime);

        [HttpPost]
        public async Task<IActionResult> Post([FromForm] UploadImage Image )
        {
            
            IFormFile? image = Image.ImageFile;
            string? owner = Image.Owner;

            try
            {
                if (image is null || image.Length == 0)
                {
                    return BadRequest("BadRequest, No file uploaded.");
                }
                var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
                if (extension != ".jpg" && extension != ".jpeg") 
                {
                    return BadRequest(extension + "BadRequest, isn't supported for this task, please uplode .jpg ONLY");
                }
                if(string.IsNullOrWhiteSpace(owner))
                {
                    return BadRequest("BadRequest, Owner field is REQUIRED for the task");
                }

                Directory.CreateDirectory(folderPath);

                var imageName = Path.GetFileName(image.FileName) ;
                var finalName = imageName;
                var baseName = Path.GetFileNameWithoutExtension(imageName);
                var imagePath = Path.Combine(folderPath, finalName);

                int i = 1;
                while (System.IO.File.Exists(imagePath))
                {
                    finalName = baseName + "_" + i + extension;
                    imagePath = Path.Combine(folderPath, finalName);
                    i++;
                }

                

                await using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);

                }

                DateTime now = DateTime.Now;
                MetaData metaData = new MetaData(finalName, owner, now, now );

                var metaPath = Path.Combine(folderPath, $"{finalName}.json"); 
                await System.IO.File.WriteAllTextAsync(metaPath, JsonSerializer.Serialize(metaData));

                return Ok("Created!");

            }
            catch
            {
                return StatusCode( 500 , "An internal server error occurred.");
            }
            
        }
    }
}
