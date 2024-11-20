using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SkyHub2.Models.Roles;

namespace SkyHub2.Models.Flight_Details
{
    public class Flights
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Ensures IDENTITY(1,1)
        public int FlightId { get; set; }

        [Required, StringLength(100)]
        public string FlightName { get; set; }

        [Required, StringLength(20)]
        public string FlightNumber { get; set; }

        [Required]
        public DateTime DepartureTime { get; set; }

        [Required]
        public DateTime ArrivalTime { get; set; }

        [Required]
        public decimal Fare { get; set; }

        [Required]
        public int TotalSeats { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int AvailableSeats { get; set; }

        [Required]
        public int FlightOwnerId { get; set; }

        [Required]
        public int RouteId { get; set; }

        // Navigation Properties
        public virtual FlightOwner FlightOwner { get; set; }
        public virtual Routes Route { get; set; }
        public virtual ICollection<Seats> Seats { get; set; }
        public virtual ICollection<Bookings> Bookings { get; set; }
        public virtual BaggageInfos BaggageInfo { get; set; }

    }

}
