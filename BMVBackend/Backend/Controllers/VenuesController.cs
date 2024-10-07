using Backend.DTO.Slot;
using Backend.DTO.Venue;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VenuesController : ControllerBase
    {
        IVenuesService _service;
        public VenuesController(IVenuesService service)
        {
            _service = service;
        }

        // GET: api/<VenueController>
        [HttpGet]
        public IActionResult Get([FromQuery] bool? topRated, [FromQuery] bool? topBooked)
        {
            var res = new GetVenuesDTO();
            if (topRated!= null && topRated == true)
            {
                var x = _service.GetTopRatedVenues();

                res.TopRatedVenues = x;
            }
            if(topBooked != null && topBooked == true)
            {
                res.TopBookedVenues = _service.GetTopBookedVenues();
            }
            if((topRated != null && topRated == true) || (topBooked != null && topBooked == true))
            {
                return Ok(res);
            }
            var v = _service.GetAllVenues();
            if (v == null) {
                return BadRequest();
            }
            Console.WriteLine(46);
            return Ok(v);
        }

        // GET api/<VenueController>/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var v = _service.GetVenueById(id);
            if(v == null)
            {
                return NotFound();
            }
            var cservice = new CategoryService();
            var cName = cservice.GetCategoryById(v.CategoryId).Name;
            return Ok(v);
        }

        // POST api/<VenueController>
        [HttpPost]
        [Authorize]
        public IActionResult Post(PostVenueDTO venueWithSlotDetails)
        {
            var providerId = User.Claims.FirstOrDefault(c => c.Type == "ProviderId")?.Value;
            Console.WriteLine(providerId);
            if(providerId == null)
            {
                return BadRequest();
            }
            var v = _service.AddVenue(Convert.ToInt32(providerId),venueWithSlotDetails);
            return Ok(v);
        }

        // PUT api/<VenueController>/5
        [HttpPut("{id}")]
        [Authorize]
        public IActionResult Put(int id, [FromBody] PutVenueDTO v)
        {
            var providerId = User.Claims.FirstOrDefault(c => c.Type == "ProviderId")?.Value;
            if (providerId == null)
            {
                return BadRequest();
            }
            var cv = _service.GetVenueById(id);
            if (cv == null || cv.ProviderId != Convert.ToUInt32(providerId)) { 
                return BadRequest();
            }
            var uv = _service.UpdateVenue(id, v);
            if (uv != null)
            {
                return Ok(uv);
            }
            return NotFound();
        }

        // DELETE api/<VenueController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
