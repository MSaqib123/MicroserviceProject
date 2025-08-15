using Mango.Services.AuthAPI.Models.Dto;
using Mango.Services.CouponAPI.Models;
using Microsoft.AspNetCore.Identity.Data;

namespace Mango.Services.AuthAPI.Services.IServices
{
    public interface IAuthService
    {
        Task<bool> AssignRole(string email, string roleName);

        //Task<UserDto> Register(RegistrationRequestDto registrationRequestDto);
        Task<string> Register(RegistrationRequestDto registrationRequestDto);
        Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto);
        Task<ResponseDto> GetAllusers();
    }
}
