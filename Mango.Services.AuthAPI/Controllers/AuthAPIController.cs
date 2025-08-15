using AutoMapper;
using Mango.Services.AuthAPI.Data;
using Mango.Services.AuthAPI.Models.Dto;
using Mango.Services.AuthAPI.Services;
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
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            try
            {
                var responseDto = await _authService.Login(dto);

                if (responseDto.User == null)
                {
                    _response.Message = "Username and Password is invalid";
                    _response.IsSuccess = false;
                    return BadRequest(_response);
                }
                _response.Result = responseDto;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message;
                _response.IsSuccess = false;
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }

        }


        [HttpPost("assignRole")]
        public async Task<IActionResult> AssignRole([FromBody] RegistrationRequestDto dto)
        {
            try
            {
                var result = await _authService.AssignRole(dto.Email,dto.Role);
                if (!result)
                {
                    _response.Message = $"Assigning the role of {dto.Role} to {dto.Email} is Faild";
                    _response.IsSuccess = false;
                    return BadRequest(_response);
                }
                _response.Message = $"Assigning the role of {dto.Role} to {dto.Email} is Successfully done";
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message;
                _response.IsSuccess = false;
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }

        }


        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var result = await _authService.GetAllusers();
                _response.Result = result;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message;
                _response.IsSuccess = false;
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }

        }


    }
}
