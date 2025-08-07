using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Mango.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ITokenProvider _tokenProvider;

        public AuthController(
            IAuthService authService,
            ITokenProvider tokenProvider
            )
        {
            _authService = authService;
            _tokenProvider = tokenProvider;
        }

        [HttpGet]
        public IActionResult Login()
        {
            LoginRequestDto loginrequestdto = new();
            return View(loginrequestdto);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDto obj)
        {
            ResponseDto responseDto = await _authService.LoginAsync(obj);

            if (responseDto != null && responseDto.IsSuccess)
            {
                LoginResponseDto loginResponseDto = JsonConvert.DeserializeObject<LoginResponseDto>(Convert.ToString(responseDto.Result));

                //====== Set Token after Retrievning from Server =========
                await SignInWithClaimsFromJwt(loginResponseDto);
                _tokenProvider.SetToken(loginResponseDto.Token);

                TempData["success"] = "Login Successfully";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["error"] = responseDto.Message;
                return View(obj);
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            var roleList = new List<SelectListItem>()
            {
                new SelectListItem{Text=SD.RoleAdmin,Value=SD.RoleAdmin,Selected=true},
                new SelectListItem{Text=SD.RoleCustomer,Value=SD.RoleCustomer},
            };
            ViewBag.RoleList = roleList;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegistrationRequestDto obj)
        {
            ResponseDto result = await _authService.RegisterAsync(obj);
            ResponseDto assingRole;

            if (result != null && result.IsSuccess)
            {
                if (string.IsNullOrEmpty(obj.Role))
                {
                    obj.Role = SD.RoleCustomer;
                }
                assingRole = await _authService.AssignRoleAsync(obj);
                if (assingRole != null && assingRole.IsSuccess)
                {
                    TempData["success"] = "Registration Successful";
                    return RedirectToAction(nameof(Login));
                }
            }
            else
            {
                TempData["error"] = result.Message;
            }

            var roleList = new List<SelectListItem>()
            {
                new SelectListItem{Text=SD.RoleAdmin,Value=SD.RoleAdmin},
                new SelectListItem{Text=SD.RoleCustomer,Value=SD.RoleCustomer},
            };

            ViewBag.RoleList = roleList;
            return View(obj);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            //_tokenProvider.ClearToken();
            return RedirectToAction("Index", "Home");
        }


        /// <summary>
        /// This method signs in a user using the JWT token returned from login.
        /// It reads the token, extracts the user data (claims), and then sets up a login session using cookies.
        /// </summary>
        private async Task SignInWithClaimsFromJwt(LoginResponseDto model)
        {
            // Create a handler to read and process the JWT token
            var handler = new JwtSecurityTokenHandler();

            // Decode (read) the JWT token so we can get user information (like email, ID, name, etc.)
            var jwt = handler.ReadJwtToken(model.Token);

            // Create a new identity object using cookie authentication (not JWT, but browser-friendly)
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);

            // ========== Add Claims from JWT ==========
            // These are like small pieces of info about the user (email, id, username) that we'll store for later use

            // Add the user's email from the JWT token
            identity.AddClaim(new Claim(
                JwtRegisteredClaimNames.Email,
                jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email)?.Value ?? string.Empty
            ));

            // Add the user's ID (this is usually stored in the 'sub' field of the token)
            identity.AddClaim(new Claim(
                JwtRegisteredClaimNames.Sub,
                jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Sub)?.Value ?? string.Empty
            ));

            // Add the user's username (stored as 'name' in the token)
            identity.AddClaim(new Claim(
                JwtRegisteredClaimNames.Name,
                jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Name)?.Value ?? string.Empty
            ));

            // ========== Add .NET-specific Claims ==========
            // These are used by ASP.NET Core's authentication system internally
            // Set the ClaimTypes.Name — this is what .NET uses when you call User.Identity.Name
            // Here we're using the user's email as their identity name
            identity.AddClaim(new Claim(
                ClaimTypes.Name,
                jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email)?.Value ?? string.Empty
            ));

            // Set the ClaimTypes.Role — this is used for role-based authorization, like [Authorize(Roles = "Admin")]
            // We're assuming there's a 'role' field in the JWT token
            identity.AddClaim(new Claim(
                ClaimTypes.Role,
                jwt.Claims.FirstOrDefault(u => u.Type == "role")?.Value ?? string.Empty
            ));

            // ========== Final Step: Sign in the user ==========
            // Create a ClaimsPrincipal object — this represents the user and their identity
            var principal = new ClaimsPrincipal(identity);

            // Sign in the user using cookie-based authentication
            // This will store a secure cookie in the browser so the user stays logged in on future requests
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }




        /*
            🧠 THE SIMPLE ANSWER
            ✅ GenerateToken()
            ➡️ Creates a JWT token on the server during login
            ➡️ Token includes user info (claims), so we can send it securely to the client

            ✅ SignInUser()
            ➡️ Reads that token back on the client or front-end side (like in MVC app or web frontend)
            ➡️ Extracts the info (claims) from that token and logs in the user using cookies

            🧩 So what’s the relationship?
            GenerateToken() happens on the Auth API / server side when the user logs in
            SignInUser() happens on the client/web app side after receiving the token

            They work together like this:

            🔁 Login Flow Step-by-Step:
            Step	What Happens	                        Code Involved
            1️⃣	    User logs in (POST: /login) 	        Frontend or UI sends login info
            2️⃣	    Server checks credentials	            In AuthAPI (e.g., using Identity)
            3️⃣	    Server creates JWT token	            ✅ JwtTokenGenerator.GenerateToken()
            4️⃣	    Server sends back the token 	        Like: { token: "eyJhb..." }
            5️⃣	    Client receives the token	            LoginResponseDto has .Token
            6️⃣	    Client reads token & signs in	        ✅ SignInUser(model)
            7️⃣	    Now client is logged in via cookie
         
         
         */


    }
}
