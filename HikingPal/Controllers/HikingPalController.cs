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
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class HikingPalController : Controller
    {
        private readonly IUserService _userService;
        private readonly IHikeService _hikeService;
        private readonly IJwtAuthService _jwtAuthService;
        private readonly ILogger<HikingPalController> _logger;

        public HikingPalController(ILogger<HikingPalController> logger,
            IUserService userService,
            IHikeService hikeService,
            IJwtAuthService jwtAuthService)
        {
            _logger = logger;
            _userService = userService;
            _hikeService = hikeService;
            _jwtAuthService = jwtAuthService;
        }

        [HttpDelete("deletehike/{hikeid}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> DeleteHike(string hikeid)
        {           
           var result = await _hikeService.DeleteHike(hikeid);
           
            if(result) { 
               return Ok(); 
            }
           return BadRequest();
        }

        [HttpPut("modifyhike")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> ModifyHike([FromBody] Hike modifyHike)
        {
            var result = await _hikeService.ModifyHike(modifyHike);

            if (result)
            {
                return Accepted();
            }
            return BadRequest();
        }


        [HttpPost("subscribe")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> SubscribeHike([FromBody] HikeUser subscribleHike)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _hikeService.SubscribeHike(subscribleHike, true);
            return Created($"/gethike/{subscribleHike.HikeID.ToString()}", null);
        }

        [HttpDelete("unsubscribe")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> UnSubscribeHike([FromBody] HikeUser subscribleHike)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _hikeService.SubscribeHike(subscribleHike, false);
            return Created($"/gethike/{subscribleHike.HikeID.ToString()}", null);
        }


        [HttpGet("gethike/{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> GetHike(string id)
        {  

            if(String.IsNullOrEmpty(id) && Guid.TryParse(id, out _))
            {
                return BadRequest();
            }

            var hike = await _hikeService.GetHike(id);

            return Ok(hike);
        }

        [HttpGet("gethikes")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetAllHike()
        {
            var hikes = await _hikeService.GetHikes();

            return Ok(hikes);
        }

        [HttpPost("createhike")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CreateHike([FromBody] Hike newHike)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userGuidString = User.Claims.ToArray()[2].Value;

            var createdHike = await _hikeService.CreateHike(newHike, userGuidString);

            if (createdHike != null)
            {
                return Created($"/gethike/{createdHike.HikeID}", createdHike);
            }
            else
            {
                return BadRequest(ModelState);
            }
        }


        [AllowAnonymous]
        [HttpPost("createuser")]
        public async Task<ActionResult> CreateUser([FromBody] UserCreateRequest user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdUser = await _userService.CreateUser(user);

            if (createdUser != null)
            {
                return Ok(new LoginResult
                {
                    UserName = createdUser.UserName,
                    Role = createdUser.Role
                });
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

            return Ok(new LoginResult
            {
                UserName = myClaims[0].Value,
                Role = new Role() { 
                    RoleName = myClaims[1].Value
                }   
            });            
        }


        [AllowAnonymous]
        [HttpPost("login")]
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

        [HttpPost("logout")]
        [Authorize]
        public ActionResult Logout()
        {
            // optionally "revoke" JWT token on the server side --> add the current token to a block-list
            // https://github.com/auth0/node-jsonwebtoken/issues/375

            var userName = User.Identity?.Name;
            _jwtAuthService.RemoveRefreshTokenByUserName(userName);
            _logger.LogInformation($"User [{userName}] logged out the system.");
            return Ok();
        }

        [HttpPost("refresh-token")]
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
