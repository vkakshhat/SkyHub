using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkyHub2.Models.Roles
{
    public class Admin
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Ensures IDENTITY(1,1)
        public int AdminId { get; set; }

        [Required]
        public int UserId { get; set; }

        // Navigation properties
        public Users User { get; set; }
    }
}
