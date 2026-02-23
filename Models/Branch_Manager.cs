using System.ComponentModel.DataAnnotations.Schema;

namespace Nets_System.Models
{
    [Table("Branch_Manager")]
    public class Branch_Manager
    {
        public int Id { get; set; }
        public int Branch_Id { get; set; }
        public int User_Id { get; set; }

        public DateTime? Start_Date { get; set; }
        public DateTime? End_Date { get; set; }

        public DateTime? Created_Time { get; set; }
        public DateTime? Last_Modified_Time { get; set; }

        public Branch? Branch { get; set; }
        public User? User { get; set; }
    }
}
