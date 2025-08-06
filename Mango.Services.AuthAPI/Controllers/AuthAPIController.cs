using AutoMapper;
using Mango.Services.AuthAPI.Data;
using Mango.Services.CouponAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Xml;

namespace Mango.Services.AuthAPI.Controllers
{
    [ApiController]
    [Route("api/Auth")]
    public class AuthAPIController : ControllerBase
    {
        private readonly AppDbContext _db;
        private ResponseDto _response;
        private readonly IMapper _mapper;
        
        public AuthAPIController(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _response = new ResponseDto();
            _mapper = mapper;
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register()
        {
            try
            {
                return Ok();
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message;
                _response.IsSuccess = false;
                return NotFound();
            }
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(int id)
        {
            try
            {
                return Ok();
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message;
                _response.IsSuccess = false;
                return NotFound();
            }

        }



    }
}
