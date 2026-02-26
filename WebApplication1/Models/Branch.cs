using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    [Table("Branch")]
    public class Branch
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? Status { get; set; }

        public DateTime? Created_Time { get; set; }
        public DateTime? Last_Modified_Time { get; set; }

        // Navigation
        public ICollection<Branch_Manager>? BranchManagers { get; set; }
    }
}
