using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FlightInformationAPI.Data;
using FlightInformationAPI.Models;


namespace FlightInformationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightsController : ControllerBase
    {
        private readonly FlightDBContext _context;
        private readonly ILogger<FlightsController> _logger;
        
        //public FlightsController(FlightDBContext context)
        //{
        //    _context = context;
        //}
        public FlightsController(FlightDBContext context, ILogger<FlightsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/flights
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Flight>>> GetFlights()
        {
            return await _context.Flights.ToListAsync();
        }

        // GET: api/flights/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Flight>> GetFlight(int id)
        {
            _logger.LogInformation("Fetching flight with ID {FlightId}", id);
            var flight = await _context.Flights.FindAsync(id);

            if (flight == null)
            {
                _logger.LogWarning("Flight with ID {FlightId} not found", id);
                return NotFound();
            }

            return flight;
        }

        // POST: api/flights
        [HttpPost]
        public async Task<ActionResult<Flight>> PostFlight(Flight flight)
        {
            _context.Flights.Add(flight);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetFlight), new { id = flight.Id }, flight);
        }

        // PUT: api/flights/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFlight(int id, Flight flight)
        {
            if (id != flight.Id)
            {
                return BadRequest();
            }

            _context.Entry(flight).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FlightExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/flights/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFlight(int id)
        {
            var flight = await _context.Flights.FindAsync(id);
            if (flight == null)
            {
                return NotFound();
            }

            _context.Flights.Remove(flight);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/flights/search
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Flight>>> SearchFlights([FromQuery] string airline = null, [FromQuery] string departureAirport = null, [FromQuery] string arrivalAirport = null, [FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
        {
            var query = _context.Flights.AsQueryable();

            if (!string.IsNullOrEmpty(airline))
            {
                query = query.Where(f => f.Airline.Contains(airline));
            }

            if (!string.IsNullOrEmpty(departureAirport))
            {
                query = query.Where(f => f.DepartureAirport.Contains(departureAirport));
            }

            if (!string.IsNullOrEmpty(arrivalAirport))
            {
                query = query.Where(f => f.ArrivalAirport.Contains(arrivalAirport));
            }

            if (startDate.HasValue)
            {
                query = query.Where(f => f.DepartureTime >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(f => f.ArrivalTime <= endDate.Value);
            }

            return await query.ToListAsync();
        }

        private bool FlightExists(int id)
        {
            return _context.Flights.Any(e => e.Id == id);
        }

    }
}
