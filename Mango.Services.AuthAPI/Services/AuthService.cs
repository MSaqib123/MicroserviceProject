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
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthService(AppDbContext db,
            IJwtTokenGenerator jwtTokenGenerator,
            UserManager<ApplicationUser> userManager, 
            RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _jwtTokenGenerator = jwtTokenGenerator;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<bool> AssignRole(string email, string roleName)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());
            if (user != null)
            {
                //---- 1st way for Await --------
                //var roleexit = await _roleManager.RoleExistsAsync(roleName);

                //---- 2nd way for Await --------
                //var roleexit = _roleManager.RoleExistsAsync(roleName).Result;

                //---- 3rd way for Await --------
                if (!_roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
                {
                    await _roleManager.CreateAsync(new IdentityRole(roleName));//.GetAwaiter().GetResult();
                }
                await _userManager.AddToRoleAsync(user, roleName);
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<LoginResponseDto> Login(LoginRequestDto dto)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.UserName.ToLower() == dto.UserName.ToLower());
            bool isValid = await _userManager.CheckPasswordAsync(user,dto.Password);
            if(!isValid || user == null)
            {
                return new LoginResponseDto() { User=null,Token=""};
            }

            //if user was found , Generate JWT Token
            var roles = await _userManager.GetRolesAsync(user);

            // if user exist  then create JWT token
            var jwtToken = _jwtTokenGenerator.GenerateToken(user, roles);

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
                Token = jwtToken,
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
