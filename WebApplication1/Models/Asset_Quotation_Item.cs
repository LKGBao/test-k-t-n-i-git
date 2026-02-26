using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    [Table("Asset_Quotation_Item")]
    public class Asset_Quotation_Item
    {
        public int Id { get; set; }

        // FK Asset_Quotation
        public int Request_Quotation_Id { get; set; }

        public decimal Unit_Price { get; set; }

        public decimal DiscountPercent { get; set; }

        public DateTime? Created_Time { get; set; }

        public DateTime? Last_Modified_Time { get; set; }

        // Navigation properties
        public virtual Asset_Quotation Asset_Quotation { get; set; }
    }
}
