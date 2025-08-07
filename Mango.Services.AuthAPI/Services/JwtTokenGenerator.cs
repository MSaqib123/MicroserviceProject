using Mango.Services.AuthAPI.Data;
using Mango.Services.AuthAPI.Models;
using Mango.Services.AuthAPI.Models.Dto;
using Mango.Services.AuthAPI.Services.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Mango.Services.AuthAPI.Services
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly JwtOption _jwtOptions;
        public JwtTokenGenerator(IOptions<JwtOption> jwtOptions)
        {
            _jwtOptions = jwtOptions.Value;
        }

        public string GenerateToken(ApplicationUser applicationUser, IEnumerable<string> roles)
        {
            /*
             🎯 What is a JWT Token?
            JWT stands for JSON Web Token.
            Think of it as a sealed envelope that contains information about a user, signed by your server so it can be trusted.
            ??????? It’s mostly used to tell the server: ???????
            ------ "Hey, I am this user. Here’s proof." --------
             📦 What’s Inside a JWT Token?
                A JWT token looks like this:

                eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.
                eyJzdWIiOiIxMjM0IiwibmFtZSI6IkFsaSIsImVtYWlsIjoiYWxpQGVtYWlsLmNvbSJ9.
                Xskajsdflkjwef098fje9wef9uwef098wef08

                This is one string made of 3 parts, separated by . (dot):
                Part	        Name	        Description
                🔐 Part 1	    Header	        Says which algorithm is used to sign the token (e.g., HS256)
                🧾 Part 2	    Payload	        The actual data (claims) about the user: ID, email, username, etc.
                🔏 Part 3	    Signature	    A hash that makes sure the token hasn’t been changed (signed by server)
             */


            //🔧 1. Create a token handler (JWT tool)
            //You are creating a tool(handler) that helps you create and write JWT tokens.
            // Think of it like a pen that knows how to write JWT letters.
            var tokenHandler = new JwtSecurityTokenHandler();


            //🔑 2. Get the secret key
            //Your app has a secret key (like a password) stored in settings.
            //Here you're converting it to bytes so you can use it to sign the token.
            // Think of it as using a secret ink for the signature part of your token letter.
            var key = Encoding.ASCII.GetBytes(_jwtOptions.Secret);


            //🧾 3. Create the list of user claims (information)
            // Think of this as writing inside the JWT letter:
            var claimList = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email,applicationUser.Email),
                new Claim(JwtRegisteredClaimNames.Sub,applicationUser.Id),
                new Claim(JwtRegisteredClaimNames.Name,applicationUser.UserName)
            };


            //✅ Optional: If you wanted to add roles (like admin, user), you could add them as well using:
            //claimList.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            //📜 4.Describe the token you want to generate
            /*
             I want to write a letter that:
                Is valid for 7 days
                Is written to API users
                Comes from my app
                Contains this user info
                And is signed with my secret pen
            */
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Audience = _jwtOptions.Audience,
                Issuer = _jwtOptions.Issuer,
                Subject = new ClaimsIdentity(claimList),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            //✍️ 5.Create and return the token 
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
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
