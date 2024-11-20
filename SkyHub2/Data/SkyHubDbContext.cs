using Microsoft.EntityFrameworkCore;
using SkyHub2.Models.Flight_Details;
using SkyHub2.Models.Payment_Details;
using SkyHub2.Models.Roles;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace SkyHub2.Data
{
    public class SkyHub2DbContext : DbContext
    {
        public SkyHub2DbContext() { }
        public SkyHub2DbContext(DbContextOptions<SkyHub2DbContext> options) : base(options)
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var configBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
            var configSection = configBuilder.GetSection("ConnectionStrings");
            var conStr = configSection["ConStr"] ?? null;

            optionsBuilder.UseSqlServer(conStr);

        }

        public DbSet<Users> Users { get; set; }
        public DbSet<Passenger> Passenger { get; set; }
        public DbSet<FlightOwner> FlightOwner { get; set; }
        public DbSet<Admin> Admin { get; set; }
        public DbSet<Routes> Routes { get; set; }
        public DbSet<Flights> Flights { get; set; }
        public DbSet<SeatTypes> SeatTypes { get; set; }
        public DbSet<Seats> Seats { get; set; }
        public DbSet<Bookings> Bookings { get; set; }
        public DbSet<BookingItems> BookingItems { get; set; }
        public DbSet<BaggageInfos> BaggageInfos { get; set; }
        public DbSet<Payments> Payments { get; set; }
        public DbSet<Refunds> Refunds { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // schemas
            modelBuilder.Entity<Users>().ToTable("Users", schema: "Roles");
            modelBuilder.Entity<Passenger>().ToTable("Passenger", schema: "Roles");
            modelBuilder.Entity<FlightOwner>().ToTable("FlightOwner", schema: "Roles");
            modelBuilder.Entity<Admin>().ToTable("Admin", schema: "Roles");
            modelBuilder.Entity<Routes>().ToTable("Routes", schema: "Flight_Details");
            modelBuilder.Entity<Flights>().ToTable("Flights", schema: "Flight_Details");
            modelBuilder.Entity<SeatTypes>().ToTable("SeatTypes", schema: "Flight_Details");
            modelBuilder.Entity<Seats>().ToTable("Seats", schema: "Flight_Details");
            modelBuilder.Entity<Bookings>().ToTable("Bookings", schema: "Flight_Details");
            modelBuilder.Entity<BookingItems>().ToTable("BookingItems", schema: "Flight_Details");
            modelBuilder.Entity<BaggageInfos>().ToTable("BaggageInfo", schema: "Flight_Details");
            modelBuilder.Entity<Payments>().ToTable("Payments", schema: "Payment_Details");
            modelBuilder.Entity<Refunds>().ToTable("Refunds", schema: "Payment_Details");

            modelBuilder.Entity<Users>(entity =>
            {
                entity.Property(u => u.Email)
                    .IsRequired()
                    .HasMaxLength(100);

                // email validation
                entity.HasAnnotation("Relational:CheckConstraint:CHK_Users_Email", "[Email] LIKE '%@%'");

                entity.Property(u => u.PasswordHash)
                    .IsRequired();

                // password validation 
                entity.HasAnnotation(
                    "Relational:CheckConstraint:CHK_Users_Password",
                    "[PasswordHash] LIKE '%[A-Z]%' AND " +
                    "[PasswordHash] LIKE '%[0-9]%' AND " +
                    "[PasswordHash] LIKE '%[^a-zA-Z0-9]%' AND " +
                    "LEN([PasswordHash]) >= 8"
                );
            });


            // Passenger-User (1:1)
            modelBuilder.Entity<Passenger>()
                .HasOne(c => c.User)
                .WithOne(u => u.Customer)
                .HasForeignKey<Passenger>(c => c.UserId);

            // FlightOwner-User (1:1)
            modelBuilder.Entity<FlightOwner>()
                .HasOne(f => f.User)
                .WithOne(u => u.FlightOwner)
                .HasForeignKey<FlightOwner>(f => f.UserId);

            // Admin-User (1:1)
            modelBuilder.Entity<Admin>()
                .HasOne(a => a.User)
                .WithOne(u => u.Admin)
                .HasForeignKey<Admin>(a => a.UserId);

            // Routes-FlightOwner (Many-to-1)
            modelBuilder.Entity<Routes>()
                .HasOne(r => r.FlightOwner)
                .WithMany(o => o.Routes)
                .HasForeignKey(r => r.OwnerId);

            // Flights-FlightOwner (Many-to-1)
            modelBuilder.Entity<Flights>()
                .HasOne(f => f.FlightOwner)
                .WithMany(o => o.Flights)
                .HasForeignKey(f => f.FlightOwnerId);

            // Flights-Routes (Many-to-1)
            modelBuilder.Entity<Flights>()
                .HasOne(f => f.Route)
                .WithMany(r => r.Flights)
                .HasForeignKey(f => f.RouteId);

            // Flights-Seats (1-to-Many)
            modelBuilder.Entity<Seats>()
                .HasOne(s => s.Flight)
                .WithMany(f => f.Seats)
                .HasForeignKey(s => s.FlightId);

            // Seats-SeatTypes (Many-to-1)
            modelBuilder.Entity<Seats>()
                .HasOne(s => s.SeatType)
                .WithMany(st => st.Seats)
                .HasForeignKey(s => s.SeatTypeId);

            // Bookings-User (Many-to-1)
            modelBuilder.Entity<Bookings>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.UserId);

            // Bookings-Flight (Many-to-1)
            modelBuilder.Entity<Bookings>()
                .HasOne(b => b.Flight)
                .WithMany(f => f.Bookings)
                .HasForeignKey(b => b.FlightId);

            // BookingItems-Bookings (Many-to-1)
            modelBuilder.Entity<BookingItems>()
                .HasOne(bi => bi.Booking)
                .WithMany(b => b.BookingItems)
                .HasForeignKey(bi => bi.BookingId);

            // BookingItems-Seats (Many-to-1)
            modelBuilder.Entity<BookingItems>()
                .HasOne(bi => bi.Seat)
                .WithMany(s => s.BookingItems)
                .HasForeignKey(bi => bi.SeatId);

            // BookingItems-SeatTypes (Many-to-1)
            modelBuilder.Entity<BookingItems>()
                .HasOne(bi => bi.SeatType)
                .WithMany(st => st.BookingItems)
                .HasForeignKey(bi => bi.SeatTypeId);

            // BaggageInfos-Flights(1-to-1)
            modelBuilder.Entity<BaggageInfos>()
                .HasOne(b => b.Flight)
                .WithOne(f => f.BaggageInfo)
                .HasForeignKey<BaggageInfos>(b => b.FlightId)
                .OnDelete(DeleteBehavior.Cascade);

            // Payments-Bookings (1-to-1)
            modelBuilder.Entity<Payments>()
            .HasOne(p => p.Booking)
                .WithOne(b => b.Payment)
                .HasForeignKey<Payments>(p => p.BookingId);

            // Refunds-Payment (1-to-1)
            modelBuilder.Entity<Refunds>()
                .HasOne(r => r.Payment)
                .WithOne(p => p.Refund)
                .HasForeignKey<Refunds>(r => r.PaymentId);

            base.OnModelCreating(modelBuilder);
        }

        private static byte[] HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }
    }
}
