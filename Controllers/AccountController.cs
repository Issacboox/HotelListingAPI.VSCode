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
        private readonly ILogger<AccountController> _logger;

        public AccountController(IAuthManager authManager, ILogger<AccountController> logger)
        {
            this._authManager = authManager;
            this._logger = logger;
        }

        [HttpPost]
        [Route("register")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Register([FromBody] ApiUserDto apiUserDto)
        {
            _logger.LogInformation($"Register Attempt for {apiUserDto.Email}");
            try
            {
                var errors = await _authManager.Register(apiUserDto);
                if (errors.Any())
                {
                    foreach (var error in errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                    // Return BadRequest if there are validation errors
                    return BadRequest(new ApiResponse<object>(StatusCodes.Status400BadRequest, false, "Error Creating User", null));
                }

                // Return OK if registration is successful
                return Ok(new ApiResponse<ApiUserDto>(StatusCodes.Status201Created, true, "Successfully Created", null));
            }
            catch (Exception ex)
            {
                // Log the exception details
                _logger.LogError(ex, $"Something went wrong in the {nameof(Register)} - User Registration attempt for {apiUserDto.Email}. Please contact support");

                // Return InternalServerError if an exception occurs
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>(StatusCodes.Status500InternalServerError, false, "An error occurred while processing your request.", null));
            }
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