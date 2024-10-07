using Backend.DTO.Provider;
using Backend.Helpers;
using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProvidersController : ControllerBase
    {
        IProvidersService _service;
        public ProvidersController(IProvidersService providerService) {
            _service = providerService;
        }
        // GET: api/<ProviderController>
        [HttpGet]
        [Authorize]
        public IActionResult Get()
        {
            var providerId = User.Claims.FirstOrDefault(c => c.Type == "ProviderId")?.Value;
            if(providerId == null)
            {
                return BadRequest();
            }
            var provider = _service.GetProviderById(Convert.ToInt32(providerId));
            if(provider == null)
            {
                return BadRequest();
            }
            return Ok(provider);
        }

        // GET api/<ProviderController>/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var provider = _service.GetProviderById(id);
            if (provider == null) {
                return NotFound();
            }
            return Ok(provider);
        }

        // POST api/<ProviderController>
        [HttpPost]
        public IActionResult Post([FromBody] PostProviderDTO provider)
        {
            Provider newProvider = new Provider();
            newProvider.Mobile = provider.Mobile;
            newProvider.Name = provider.Name;
            newProvider.Email = provider.Email;
            newProvider.Password = provider.Password;
            if (_service.AddProvider(newProvider))
            {
                return Ok(newProvider);
            }
            return BadRequest();
        }

        // PUT api/<ProviderController>/5
        [Authorize]
        [HttpPut]
        public IActionResult Put([FromBody] PutProviderDTO newProvider)
        {
            var providerId = User.Claims.FirstOrDefault(c => c.Type == "ProviderId")?.Value;
            Provider provider = new Provider() { Email=newProvider.Email, Mobile= newProvider.Mobile, Name= newProvider.Name, Password= newProvider.Password};
            var updatedProvider = _service.UpdateProvider(Convert.ToInt32(providerId), provider);
            if (updatedProvider == null)
            {
                return BadRequest();
            }
            return Ok(updatedProvider);
            
        }

        // DELETE api/<ProviderController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        [HttpPost]
        [Route("register")]
        public IActionResult Register(ProviderRegisterDTO value)
        {
            if (value == null)
            {
                return BadRequest();
            }
            var p = _service.RegisterProvider(value);
            if (p!=null)
            {
                return Ok(new TokenResult()
                {
                    Status = "success",
                    Token = new TokenHelper().GenerateProviderToken(p)
                });
            }
            return NotFound(new TokenResult() { Status = "failed", Token = null });
        }

        [HttpPost]
        [Route("login")]
        public IActionResult Validate(ProviderLoginDTO value)
        {
            if (value == null)
            {
                return BadRequest();
            }
            var p = _service.ValidateProvider(value);
            if (p != null)
            {
                return Ok(new TokenResult()
                {
                    Status = "success",
                    Token = new TokenHelper().GenerateProviderToken(p)
                });
            }
            return NotFound(new TokenResult() { Status = "failed", Token = null });
        }
    }
}
