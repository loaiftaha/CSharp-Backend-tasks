using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Task49.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class Filter : Controller
    {
        static readonly string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

        public record MetaData(string? ImageName, string? Owner, DateTime? CreateTime, DateTime? ModifiedTime);

        public enum FilterType
        {
            ByModificationDate,
            ByCreationDateDescending,
            ByCreationDateAscending,
            ByOwner,
        }

        public class FilterParams
        {
            //CreationDate, ModificationDate, Owner and FilterType
            public DateTime? CreationDate { get; set; }
            public DateTime? ModificationDate { get; set; }
            public string? Owner { get; set; }
            public FilterType? FilterType { get; set; }
            

        }

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

        

        [HttpPost]
        public async Task<IActionResult> Post([FromForm] FilterParams filterParams )
        {

            DateTime? creationDate = filterParams.CreationDate;
            DateTime? modificationDate = filterParams.ModificationDate;
            string? owner = filterParams.Owner;
            FilterType? filterType = filterParams.FilterType;



            try
            {
                if (filterType is null || !Enum.IsDefined(typeof(FilterType), filterType.Value))
                {
                    return BadRequest("Filter type MUST BE ONLY ONE of: CreationDate, ModificationDate, Owner or FilterType types.");
                }

                var getMetas = GetMetaDatas().ToList();

                var query = from meta in getMetas
                            where meta.ImageName is not null && meta.Owner is not null
                            select meta;

                switch(filterType.Value)
                    {
                    case FilterType.ByModificationDate:
                        if (modificationDate is null)
                            return BadRequest("modificationDate is null while it SHOULD NOT, sorry!");
                        query = query.Where(meta => meta.ModifiedTime.HasValue && meta.ModifiedTime.Value < modificationDate.Value);
                        break;

                    case FilterType.ByCreationDateDescending:
                        if (creationDate is null)
                            return BadRequest("creationDate is null while it SHOULD NOT, sorry!");
                        query = query.Where(meta => meta.CreateTime.HasValue && meta.CreateTime.Value > creationDate.Value)
                            .OrderByDescending(meta => meta.CreateTime);
                        break;
                    case FilterType.ByCreationDateAscending:
                        if (creationDate is null)
                            return BadRequest("creationDate is null while it SHOULD NOT, sorry!");
                        query = query.Where(meta => meta.CreateTime.HasValue && meta.CreateTime.Value > creationDate.Value)
                            .OrderBy(meta => meta.CreateTime);
                        break; 
                    case FilterType.ByOwner:
                        if (string.IsNullOrWhiteSpace(owner))
                            return BadRequest("Owner name does NOT exist while it SHOULD, sorry!");
                        query = query.Where(meta => string.Equals(owner, meta.Owner, StringComparison.OrdinalIgnoreCase));
                        break;

                    default:
                        return BadRequest("OOPS, NO MATCH!");
                        
                }


                var res = query.Select(p => new { p.ImageName, p.Owner }).ToArray();
                return Ok(res);

            }
            catch
            {
                return StatusCode(500, "internal server error");
            }

        }
    }
}
