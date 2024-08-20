using FlightInformationAPI.Models;
using Microsoft.EntityFrameworkCore;
namespace FlightInformationAPI.Data
{
    public class FlightDBContext : DbContext
    {
        public FlightDBContext() {
            using (var context = new FlightDBContext())
            {
                var newFlight = new Flight
                {
                    Id = 0,
                    FlightNumber = "AB123",
                    Airline = "New York",
                    DepartureAirport = "abc",
                    ArrivalAirport = "abc",
                    DepartureTime = DateTime.Now,
                    ArrivalTime = DateTime.Now,
                    Status = FlightStatus.Landed,
                };
                context.Flights.Add(newFlight);
                context.SaveChanges();
            }
           
            }

        public FlightDBContext(DbContextOptions<FlightDBContext> options):base(options) { }
        public DbSet<Flight> Flights { get; set; }
    }
}
