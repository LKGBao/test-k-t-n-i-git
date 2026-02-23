namespace Nets_System.Models
{
    public class Role
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }

        public DateTime? Created_Time { get; set; }
        public DateTime? Last_Modified_Time { get; set; }

        public ICollection<User>? Users { get; set; }
    }
}
