using AutoMapper;
using Mango.Services.AuthAPI.Data;
using Mango.Services.AuthAPI.Models.Dto;
using Mango.Services.AuthAPI.Services.IServices;
using Mango.Services.CouponAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Xml;

namespace Mango.Services.AuthAPI.Controllers
{
    [ApiController]
    [Route("api/Auth")]
    public class AuthAPIController : ControllerBase
    {
        private readonly IAuthService _authService;
        private ResponseDto _response;
        
        public AuthAPIController( 
            IAuthService authService)
        {
            _authService = authService;
            _response = new ResponseDto();
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDto dto)
        {
            try
            {
                var errorMessage = await _authService.Register(dto);
                if (!errorMessage.IsNullOrEmpty())
                {
                    _response.Message = errorMessage;
                    _response.IsSuccess = false;
                    return BadRequest(_response);
                }

                _response.Message = "User Created Successfully";
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message;
                _response.IsSuccess = false;
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
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
