using System.ComponentModel.DataAnnotations.Schema;

namespace Nets_System.Models
{
    [Table("Asset_Category")]
    public class Asset_Category
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime? Created_Time { get; set; }
        public DateTime? Last_Modified_Time { get; set; }
    }
}
