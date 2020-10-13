using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TestStore_server.Models;

namespace TestStore_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        [HttpPost("/token")]
        public IActionResult Token([FromBody]LoginData loginData)
        {
            var identity = GetIdentity(loginData.username, loginData.password);
            if (identity == null)
            {
                return BadRequest(new { errorText = "Invalid username or password." });
            }

            var now = DateTime.UtcNow;
            // creating JWT-token
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                access_token = encodedJwt,
                username = identity.Name
            };

            return Ok(response);
        }

        private ClaimsIdentity GetIdentity(string username, string password)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                var users = db.Users.ToList();

                User person = users.FirstOrDefault(x => x.email == username && x.password == password);
                if (person != null)
                {
                    var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, person.email),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, person.role)
                };
                    ClaimsIdentity claimsIdentity =
                    new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                        ClaimsIdentity.DefaultRoleClaimType);
                    return claimsIdentity;
                }
                return null;
            }
        }

        [Route("register")]
        [HttpPost]
        public IActionResult Register([FromBody] User request)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                db.Users.Add(request);
                db.SaveChanges();
            }

            var loginData = new LoginData { username = request.email, password = request.password };
            return Token(loginData);
        }

        [Authorize]
        [Route("getprofiledata")]
        [HttpGet]
        public IActionResult GetProfileData()
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                var users = db.Users.ToList();

                User userProfile = users.FirstOrDefault(x => x.email == User.Identity.Name);
                return Ok(userProfile);
            }
        }

        [Authorize(Roles = "admin")]
        [Route("getfullprofiledata")]
        [HttpGet]
        public IActionResult GetFullProfileData()
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                //var users = db.Users.ToList();

                //User userProfile = users.FirstOrDefault(x => x.email == User.Identity.Name);
                return Ok(db.Users.ToList());
            }
        }

        [Authorize(Roles = "admin")]
        [Route("updateprofiledata")]
        [HttpPost]
        public IActionResult UpdateProfileData([FromBody] User userToUpdate)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    var users = db.Users.ToList();
                    User userProfile = users.FirstOrDefault(x => x.email == userToUpdate.email);

                    userProfile.firstName = userToUpdate.firstName;
                    userProfile.lastName = userToUpdate.lastName;
                    userProfile.role = userToUpdate.role;

                    db.Users.Update(userProfile);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }

            return Ok();
        }

        [Authorize]
        [Route("updatecurrentuserprofiledata")]
        [HttpPost]
        public IActionResult UpdateCurrentUserProfileData([FromBody] User userToUpdate)
        {
            try
            {
                if (User.Identity.Name != userToUpdate.email)
                {
                    return NotFound("this is another user");
                }

                using (ApplicationContext db = new ApplicationContext())
                {
                    var users = db.Users.ToList();
                    User userProfile = users.FirstOrDefault(x => x.email == userToUpdate.email);

                    userProfile.firstName = userToUpdate.firstName;
                    userProfile.lastName = userToUpdate.lastName;
                    userProfile.role = userToUpdate.role;

                    db.Users.Update(userProfile);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }

            return Ok();
        }

        [Authorize]
        [Route("getrole")]
        [HttpGet]
        public IActionResult GetRole()
        {
            if (User.IsInRole("admin"))
            {
                return Ok("admin");
            }
            return Ok("user");
        }
        
    }
}
