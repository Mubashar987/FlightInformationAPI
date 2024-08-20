using Moq;
using Xunit;
using FlightInformationAPI.Controllers;
using FlightInformationAPI.Data;
using FlightInformationAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace FlightInformationAPI.Tests
{
    public class FlightsControllerTests
    {
        private readonly FlightsController _controller;
        private readonly FlightDBContext _context;
        private readonly Mock<ILogger<FlightsController>> _loggerMock;

        public FlightsControllerTests()
        {            
            // Initialize a new in-memory database context for each test
            var options = new DbContextOptionsBuilder<FlightDBContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())  // Ensure a unique database for each test run
                .Options;

            _context = new FlightDBContext(options);

            // Create Mock ILogger
            _loggerMock = new Mock<ILogger<FlightsController>>();

            // Initialize the controller with the mocked ILogger
            _controller = new FlightsController(_context, _loggerMock.Object);
        }

        [Fact]
        public async Task GetFlights_ReturnsAllFlights()
        {
            // Arrange
            var flight1 = new Flight { Id = 1, FlightNumber = "AA123", Airline = "American Airlines", DepartureAirport = "JFK", ArrivalAirport = "LAX", DepartureTime = DateTime.Now, ArrivalTime = DateTime.Now.AddHours(6), Status = FlightStatus.Scheduled };
            var flight2 = new Flight { Id = 2, FlightNumber = "BA456", Airline = "British Airways", DepartureAirport = "LHR", ArrivalAirport = "JFK", DepartureTime = DateTime.Now, ArrivalTime = DateTime.Now.AddHours(8), Status = FlightStatus.Scheduled };
            _context.Flights.AddRange(flight1, flight2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetFlights();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Flight>>>(result);
            var flights = Assert.IsType<List<Flight>>(actionResult.Value);
            Assert.Equal(2, flights.Count);
        }

        [Fact]
        public async Task GetFlight_ReturnsFlightById()
        {
            // Arrange
            var flight = new Flight { Id = 4, FlightNumber = "AA123", Airline = "American Airlines", DepartureAirport = "JFK", ArrivalAirport = "LAX", DepartureTime = DateTime.Now, ArrivalTime = DateTime.Now.AddHours(6), Status = FlightStatus.Scheduled };
            _context.Flights.Add(flight);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetFlight(4);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Flight>>(result);
            var flightResult = Assert.IsType<Flight>(actionResult.Value);
            Assert.Equal("AA123", flightResult.FlightNumber);
        }

        [Fact]
        public async Task DeleteFlight_RemovesFlight()
        {
            // Arrange
            var flight = new Flight { Id = 1, FlightNumber = "AA123", Airline = "American Airlines", DepartureAirport = "JFK", ArrivalAirport = "LAX", DepartureTime = DateTime.Now, ArrivalTime = DateTime.Now.AddHours(6), Status = FlightStatus.Scheduled };
            _context.Flights.Add(flight);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.DeleteFlight(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
            var flightInDb = await _context.Flights.FindAsync(1);
            Assert.Null(flightInDb);
        }

        [Fact]
        public async Task SearchFlights_ReturnsFilteredFlights()
        {
            // Arrange
            var flight1 = new Flight { Id = 1, FlightNumber = "AA123", Airline = "American Airlines", DepartureAirport = "JFK", ArrivalAirport = "LAX", DepartureTime = DateTime.Now, ArrivalTime = DateTime.Now.AddHours(6), Status = FlightStatus.Scheduled };
            var flight2 = new Flight { Id = 2, FlightNumber = "BA456", Airline = "British Airways", DepartureAirport = "LHR", ArrivalAirport = "JFK", DepartureTime = DateTime.Now, ArrivalTime = DateTime.Now.AddHours(8), Status = FlightStatus.Scheduled };
            _context.Flights.AddRange(flight1, flight2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.SearchFlights(airline: "British Airways");

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Flight>>>(result);
            var flights = Assert.IsType<List<Flight>>(actionResult.Value);
            Assert.Single(flights);
            Assert.Equal("BA456", flights[0].FlightNumber);
        }
    }

}