using Mango.Services.AuthAPI.Data;
using Mango.Services.AuthAPI.Models;
using Mango.Services.AuthAPI.Models.Dto;
using Mango.Services.AuthAPI.Services.IServices;
using Microsoft.AspNetCore.Identity;

namespace Mango.Services.AuthAPI.Services
{
    public class AuthService : IAuthService
    {

        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        //private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthService(AppDbContext db,
            //IJwtTokenGenerator jwtTokenGenerator,
            UserManager<ApplicationUser> userManager, 
            RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            //_jwtTokenGenerator = jwtTokenGenerator;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<LoginResponseDto> Login(LoginRequestDto dto)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.UserName == dto.UserName);
            bool isValid = await _userManager.CheckPasswordAsync(user,dto.Password);
            if(!isValid || user == null)
            {
                return new LoginResponseDto() { User=null,Token=""};
            }

            UserDto userDto = new()
            {
                Email = user.Email,
                ID = user.Id,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber
            };

            LoginResponseDto loginResponseDto = new LoginResponseDto()
            {
                User = userDto,
                Token = "",
            };

            return loginResponseDto;
        }

        //public async Task<UserDto> Register(RegistrationRequestDto registrationRequestDto)
        public async Task<string> Register(RegistrationRequestDto dto)
        {
            ApplicationUser user = new()
            {
                UserName = dto.Email,
                Email = dto.Email,
                NormalizedEmail = dto.Email.ToUpper(),
                Name = dto.Name,
                PhoneNumber = dto.PhoneNumber
            };

            try
            {
                var result = await _userManager.CreateAsync(user,dto.Password); //(user, registrationRequestDto.Password);
                if (result.Succeeded)
                {
                    var userToReturn = _db.ApplicationUsers.First(u => u.UserName == dto.Email);

                    UserDto userDto = new()
                    {
                        Email = userToReturn.Email,
                        ID = userToReturn.Id,
                        Name = userToReturn.Name,
                        PhoneNumber = userToReturn.PhoneNumber
                    };
                    //return userDto;
                    return "";
                }
                else
                {
                    return result.Errors.FirstOrDefault().Description;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return "Error Encountered";
            //return new UserDto();
        }
    }
}
