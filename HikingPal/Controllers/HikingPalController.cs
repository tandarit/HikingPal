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
    public partial class HikingPalController : Controller
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

        [HttpDelete("hikes/{hikeid}")]
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

        [HttpPut("hikes")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> ModifyHike([FromBody] Hike modifyHike)
        {
            //todo: fix the hike to hikedto
            var result = await _hikeService.ModifyHike(modifyHike);

            if (result)
            {
                return Accepted();
            }
            return BadRequest();
        }


        [HttpPost("hikes/subscribe")]
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

        [HttpDelete("hikes/unsubscribe")]
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


        [HttpGet("hikes/{id}")]
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

        [HttpGet("hikes")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetAllHike()
        {
            var hikes = await _hikeService.GetHikes();

            return Ok(hikes);
        }

        [HttpPost("hikes/createhike")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CreateHike([FromBody] HikeDTO newHike)
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


        

    }
}
