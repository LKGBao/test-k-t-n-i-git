using System.ComponentModel.DataAnnotations.Schema;

namespace Nets_System.Models
{
    [Table("Asset")]
    public class Asset
    {
        public int Id { get; set; }

        public int? Branch_Id { get; set; }

        public int? Category_Id { get; set; }

        public string? Name { get; set; }

        public string? Barcode { get; set; }

        public string? Status { get; set; }

        public DateTime? Created_Time { get; set; }

        public DateTime? Last_Modified_Time { get; set; }
    }
}
