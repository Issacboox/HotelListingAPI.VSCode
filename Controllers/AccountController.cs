using HotelListingAPI.VSCode.Contracts;
using HotelListingAPI.VSCode.Models.ApiResponse;
using HotelListingAPI.VSCode.Models.User;
using Microsoft.AspNetCore.Mvc;


namespace HotelListingAPI.VSCode.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAuthManager _authManager;

        public AccountController(IAuthManager authManager)
        {
            this._authManager = authManager;
        }

        [HttpPost]
        [Route("register")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Register([FromBody] ApiUserDto apiUserDto)
        {
            var errors = await _authManager.Register(apiUserDto);
            if (errors.Any())
            {
                foreach (var error in errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
                // return BadRequest(ModelState);
                return BadRequest(new ApiResponse<object>(StatusCodes.Status400BadRequest, false, "Error Creating User", null));
            }

            return Ok(new ApiResponse<ApiUserDto>(StatusCodes.Status201Created, true, "Succesfully Created", null));
        }


        [HttpPost]
        [Route("login")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Login([FromBody] LoginDto loginDto)
        {
            var authResponse = await _authManager.Login(loginDto);

            if (authResponse == null)
            {
                return Unauthorized(new ApiResponse<object>(StatusCodes.Status401Unauthorized, true, "Unauthorized", null));
            }

            return Ok(new ApiResponse<AuthResponseDto>(StatusCodes.Status200OK, true, "Login Successfully!", authResponse));
        }


        [HttpPost]
        [Route("refreshtoken")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> RefreshToken([FromBody] AuthResponseDto req)
        {
            var authResponse = await _authManager.VerifyRefreshToken(req);

            if (authResponse == null)
            {
                return Unauthorized(new ApiResponse<object>(StatusCodes.Status401Unauthorized, true, "Unauthorized", null));
            }

            return Ok(new ApiResponse<AuthResponseDto>(StatusCodes.Status200OK, true, "Login Successfully!", authResponse));
        }
    }
}