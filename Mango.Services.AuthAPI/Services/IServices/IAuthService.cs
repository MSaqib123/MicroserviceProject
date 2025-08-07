using Mango.Services.AuthAPI.Models.Dto;
using Microsoft.AspNetCore.Identity.Data;

namespace Mango.Services.AuthAPI.Services.IServices
{
    public interface IAuthService
    {
        //Task<UserDto> Register(RegistrationRequestDto registrationRequestDto);
        Task<string> Register(RegistrationRequestDto registrationRequestDto);
        Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto);
    }
}
