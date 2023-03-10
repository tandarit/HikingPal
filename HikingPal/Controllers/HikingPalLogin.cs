using HikingPal.Models;
using HikingPal.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace HikingPal.Controllers
{

    public partial class HikingPalController : Controller
    {
        [AllowAnonymous]
        [HttpPost("user")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserDTO>> CreateUser([FromBody] UserCreateRequest user)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdUser = await _userService.CreateUser(user);

            if (createdUser != null)
            {
                return Ok(new { 
                    UserName = createdUser.UserName,
                    Email = createdUser.Email,
                    FirstName = createdUser.FirstName,
                    LastName = createdUser.LastName                       
                });
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [Authorize]
        [HttpDelete("user/{userid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> DeleteUser(string userid)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var myClaims = User.Claims.ToArray();
            var isSuccesfullDelete = await _userService.DeleteUser(userid, myClaims);

            if (isSuccesfullDelete)
            {
                return Ok();
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [HttpGet("user")]
        [Authorize]
        public ActionResult GetCurrentUser()
        {
            var myClaims = User.Claims.ToArray();

            return Ok(new {
                        UserName = myClaims[0].Value,
                        Role = new Role()
                        {
                            RoleName = myClaims[1].Value
                        }
                    });
        }


        [AllowAnonymous]
        [HttpPost("user/login")]
        public async Task<ActionResult> LoginUser([FromBody] LoginUserRequest loginUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            User user = await _userService.Login(loginUser);

            if (user == null)
            {
                return Unauthorized();
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.Name,user.UserName),
                new Claim(ClaimTypes.Role, user.Role.RoleName),
                new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString())
            };

            var jwtResult = _jwtAuthService.GenerateTokens(user.UserName, claims, DateTime.Now);
            _logger.LogInformation($"User [{loginUser.UserName}] logged in the system.");

            return Ok(new LoginResult
            {
                UserName = loginUser.UserName,
                Role = user.Role,
                AccessToken = jwtResult.AccessToken,
                RefreshToken = jwtResult.RefreshToken.TokenString
            });
        }

        [HttpPost("user/logout")]
        [Authorize]
        public ActionResult Logout()
        {
            var userName = User.Identity?.Name;
            _jwtAuthService.RemoveRefreshTokenByUserName(userName);
            _logger.LogInformation($"User [{userName}] logged out the system.");
            return Ok();
        }

        [HttpPost("user/refresh-token")]
        [Authorize]
        public async Task<ActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            try
            {
                var userName = User.Identity?.Name;
                _logger.LogInformation($"User [{userName}] is trying to refresh JWT token.");

                if (string.IsNullOrWhiteSpace(request.RefreshToken))
                {
                    return Unauthorized();
                }

                var accessToken = await HttpContext.GetTokenAsync("Bearer", "access_token");
                var jwtResult = _jwtAuthService.Refresh(request.RefreshToken, accessToken, DateTime.Now);
                _logger.LogInformation($"User [{userName}] has refreshed JWT token.");
                return Ok(new LoginResult
                {
                    UserName = userName,
                    AccessToken = jwtResult.AccessToken,
                    RefreshToken = jwtResult.RefreshToken.TokenString
                });
            }
            catch (SecurityTokenException e)
            {
                return Unauthorized(e.Message); // return 401 so that the client side can redirect the user to login page
            }
        }
    }
}