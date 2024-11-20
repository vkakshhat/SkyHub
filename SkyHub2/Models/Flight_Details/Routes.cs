using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SkyHub2.Models.Roles;

namespace SkyHub2.Models.Flight_Details
{
    public class Routes
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Ensures IDENTITY(1,1)
        public int RouteId { get; set; }

        [Required, StringLength(100)]
        public string Origin { get; set; }

        [Required, StringLength(100)]
        public string Destination { get; set; }

        [Required]
        public TimeSpan Duration { get; set; }

        [Required]
        public decimal Distance { get; set; }

        [Required]
        public int OwnerId { get; set; }

        // Navigation Properties
        public FlightOwner FlightOwner { get; set; }
        public ICollection<Flights> Flights { get; set; }
    }
}
