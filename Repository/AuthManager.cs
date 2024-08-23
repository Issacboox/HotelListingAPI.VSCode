using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using HotelListingAPI.VSCode.Contracts;
using HotelListingAPI.VSCode.Data;
using HotelListingAPI.VSCode.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace HotelListingAPI.VSCode.Repository
{
    public class AuthManager : IAuthManager
    {
        private readonly IMapper _mapper;
        private readonly UserManager<ApiUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthManager> _logger;
        private ApiUser _user;
        private const string _loginProvider = "HotelListingApi";
        private const string _refreshToken = "RefreshToken";

        public AuthManager(IMapper mapper, UserManager<ApiUser> userManager, IConfiguration configuration, ILogger<AuthManager> logger)
        {
            _mapper = mapper;
            _userManager = userManager;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<IEnumerable<IdentityError>> Register(ApiUserDto userDto)
        {
            try
            {
                _user = _mapper.Map<ApiUser>(userDto);
                _user.UserName = userDto.Email;

                var result = await _userManager.CreateAsync(_user, userDto.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(_user, "User");
                    _logger.LogInformation($"User registered successfully: {userDto.Email}");
                }
                else
                {
                    _logger.LogWarning($"User registration failed for {userDto.Email}. Errors: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
                return result.Errors;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred during registration for user {userDto.Email}");
                throw; // Rethrow to handle it in higher-level logic if necessary
            }
        }

        public async Task<AuthResponseDto> Login(LoginDto loginDto)
        {
            try
            {
                _user = await _userManager.FindByEmailAsync(loginDto.Email);
                if (_user == null)
                {
                    _logger.LogWarning($"Login failed: User not found for email {loginDto.Email}");
                    return null;
                }

                bool isValidUser = await _userManager.CheckPasswordAsync(_user, loginDto.Password);
                if (!isValidUser)
                {
                    _logger.LogWarning($"Login failed: Invalid password for user {loginDto.Email}");
                    return null;
                }

                var token = await GenerateToken();
                var refreshToken = await CreateRefreshToken();

                _logger.LogInformation($"Login successful for user {loginDto.Email}");

                return new AuthResponseDto
                {
                    Token = token,
                    UserId = _user.Id,
                    RefreshToken = refreshToken
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred during login attempt for user {loginDto.Email}");
                throw; // Rethrow to handle it in higher-level logic if necessary
            }
        }

        public async Task<string> GenerateToken()
        {
            try
            {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSetting:Key"]));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var roles = await _userManager.GetRolesAsync(_user);
                var roleClaims = roles.Select(x => new Claim(ClaimTypes.Role, x)).ToList();
                var emailClaim = new Claim(JwtRegisteredClaimNames.Email, _user.Email);

                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, _user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, _user.Email),
                    new Claim("uid", _user.Id),
                    emailClaim
                }
                .Concat(roleClaims).ToList();

                var token = new JwtSecurityToken(
                    issuer: _configuration["JwtSetting:Issuer"],
                    audience: _configuration["JwtSetting:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(Convert.ToInt32(_configuration["JwtSetting:DurationInMinutes"])),
                    signingCredentials: credentials
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
                _logger.LogInformation($"JWT generated successfully for user {_user.Email}");
                return tokenString;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while generating token for user {_user?.Email}");
                throw; 
            }
        }

        public async Task<string> CreateRefreshToken()
        {
            try
            {
                await _userManager.RemoveAuthenticationTokenAsync(_user, _loginProvider, _refreshToken);
                var newRefreshToken = await _userManager.GenerateUserTokenAsync(_user, _loginProvider, _refreshToken);
                var result = await _userManager.SetAuthenticationTokenAsync(_user, _loginProvider, _refreshToken, newRefreshToken);

                _logger.LogInformation($"Refresh token created successfully for user {_user.Email}");
                return newRefreshToken;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while creating refresh token for user {_user.Email}");
                throw; 
            }
        }

        public async Task<AuthResponseDto> VerifyRefreshToken(AuthResponseDto request)
        {
            try
            {
                var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
                var tokenContent = jwtSecurityTokenHandler.ReadJwtToken(request.Token);
                var username = tokenContent.Claims.ToList().FirstOrDefault(q => q.Type == JwtRegisteredClaimNames.Email)?.Value;
                _user = await _userManager.FindByNameAsync(username);

                if (_user == null)
                {
                    _logger.LogWarning($"Refresh token verification failed: User not found for token {request.Token}");
                    return new AuthResponseDto
                    {
                        Token = null,
                        UserId = null,
                        RefreshToken = "Invalid user"
                    };
                }

                var isValidRefreshToken = await _userManager.VerifyUserTokenAsync(_user, _loginProvider, _refreshToken, request.RefreshToken);

                if (isValidRefreshToken)
                {
                    var token = await GenerateToken();
                    var newRefreshToken = await CreateRefreshToken();
                    _logger.LogInformation($"Refresh token verified successfully for user {_user.Email}");
                    return new AuthResponseDto
                    {
                        Token = token,
                        UserId = _user.Id,
                        RefreshToken = newRefreshToken
                    };
                }

                _logger.LogWarning($"Refresh token verification failed: Invalid refresh token for user {_user.Email}");
                return new AuthResponseDto
                {
                    Token = null,
                    UserId = _user.Id,
                    RefreshToken = "Invalid refresh token"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while verifying refresh token for user {request.Token}");
                throw; 
            }
        }
    }
}
