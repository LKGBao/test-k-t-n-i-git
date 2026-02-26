using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    [Table("Asset_Quotation")]
    public class Asset_Quotation
    {
        public int Id { get; set; }

        // FK tới Asset_Purchase_Request
        public int Purchase_Id { get; set; }

        // FK tới User
        public int User_Id { get; set; }

        // FK tới Supplier
        public int Supplier_Id { get; set; }

        public string Description { get; set; }

        public string Status { get; set; }

        public DateTime? Created_Time { get; set; }

        public DateTime? Last_Modified_Time { get; set; }

        // Navigation properties
        public virtual Assets_Purchase_Request Asset_Purchase_Request { get; set; }

        public virtual User User { get; set; }

        public virtual Supplier Supplier { get; set; }

        public virtual ICollection<Asset_Quotation_Item> Asset_Quotation_Items { get; set; }
    }
}