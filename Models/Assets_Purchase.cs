using System.ComponentModel.DataAnnotations.Schema;

namespace Nets_System.Models
{
    [Table("Asset_Purchase_Request")]
    public class Assets_Purchase_Request
    {
        public int Id { get; set; }
        public int Branch_Manager_Id { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; }
        public string? Note { get; set; }
        public DateTime Created_Time { get; set; }
        public DateTime Last_Modified_Time { get; set; }

        public Branch_Manager BranchManager { get; set; }
        public ICollection<Asset_Purchase_Request_Item>? Items { get; set; }
    }
}
