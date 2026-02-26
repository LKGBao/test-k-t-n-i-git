using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    [Table("Asset_Purchase_Request_Item")]
    public class Asset_Purchase_Request_Item
    {
        public int Id { get; set; }

        /// <summary>
        /// Trong DB: cột Purchase_Code (INT) là FK tham chiếu tới Asset_Purchase_Request(Id).
        /// </summary>
        [Column("Purchase_Code")]
        public int Asset_Purchase_Request_Id { get; set; }

        public int Asset_Category_Id { get; set; }
        public int Quantity { get; set; }
        public DateTime? Created_Time { get; set; }
        public DateTime? Last_Modified_Time { get; set; }

        public Assets_Purchase_Request? Asset_Purchase_Request { get; set; }
        public Asset_Category? Asset_Category { get; set; }
    }
}
