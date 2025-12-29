using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Task49.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TransferOwnership : Controller
    {
        public record MetaData(string? ImageName, string? Owner, DateTime? CreateTime, DateTime? ModifiedTime);
        static readonly string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
        private static IEnumerable<MetaData> GetMetaDatas()
        {
            if (!Directory.Exists(folderPath))
            {
                return Enumerable.Empty<MetaData>();
            }

            var files = Directory.GetFiles(folderPath, "*.json");


            return files.Select(path =>
            {
                try
                {
                    var json = System.IO.File.ReadAllText(path);
                    return JsonSerializer.Deserialize<MetaData>(json);
                }
                catch
                {
                    return null;
                }
            })
                .Where(meta => meta is not null)
                .Cast<MetaData>();

        }

        [HttpGet]
        public IActionResult Get([FromQuery] string? OldOwner, [FromQuery] string? NewOwner)
        {
            try
            {
                
                if (string.IsNullOrWhiteSpace(OldOwner) || string.IsNullOrWhiteSpace(NewOwner))
                    return BadRequest("Old and New Owners Are REQUIRED!");
                if (string.Equals(OldOwner, NewOwner, StringComparison.OrdinalIgnoreCase))
                    return BadRequest("THIS IS THE SAME OWNER!");
                if(!Directory.Exists(folderPath))
                    return NotFound("UPLOAD FOLDER NOT FOUND!");


                var metadata = Directory.GetFiles(folderPath, "*.json");

                foreach (var mf in metadata)
                {
                    var json = System.IO.File.ReadAllText(mf);
                    var meta = JsonSerializer.Deserialize<MetaData>(json);
                    if(meta is not null && string.Equals(meta.Owner, OldOwner, StringComparison.OrdinalIgnoreCase))
                    {
                        var updatedMeta = meta with { Owner = NewOwner, ModifiedTime = DateTime.UtcNow };
                        var updatedJson = JsonSerializer.Serialize(updatedMeta);
                        System.IO.File.WriteAllText(mf, updatedJson);
                    }
                }

                var getMetas = GetMetaDatas().ToList();

                var result = getMetas.Where(meta => string.Equals(meta.Owner, NewOwner, StringComparison.OrdinalIgnoreCase))
                    .Select(meta => new { FileName = meta.ImageName, Owner = meta.Owner, meta.ModifiedTime })
                    .ToArray();

                return Ok(result);
            }
            catch
            {
               return StatusCode(500, "internal server error");
            }
        }
    }
}
