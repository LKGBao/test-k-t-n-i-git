using System.Data;

namespace WebApplication1.Models
{
    public class User
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? First_Name { get; set; }
        public string? Last_Name { get; set; }
        public string? Status { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }

        public DateTime? Created_Time { get; set; }
        public DateTime? Last_Modified_Time { get; set; }

        // Navigation
        public Role? Role { get; set; }
        public int? Role_Id { get; set; }
        public ICollection<Branch_Manager>? BranchManagers { get; set; }
    }
}
