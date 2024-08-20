using System;
using System.ComponentModel.DataAnnotations;
public enum FlightStatus
{
    Scheduled,
    Delayed,
    Cancelled,
    InAir,
    Landed
}

namespace FlightInformationAPI.Models
{
    public class Flight
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string FlightNumber { get; set; }

        [Required]
        public string Airline { get; set; }

        [Required]
        public string DepartureAirport { get; set; }

        [Required]
        public string ArrivalAirport { get; set; }

        [Required]
        public DateTime DepartureTime { get; set; }

        [Required]
        public DateTime ArrivalTime { get; set; }

        [Required]
        public FlightStatus Status { get; set; }

    }
}
