using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using HotelListingAPI.VSCode.Contracts;
using HotelListingAPI.VSCode.Data;
using HotelListingAPI.VSCode.Models.User;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace HotelListingAPI.VSCode.Repository
{
    public class AuthManager : IAuthManager
    {
        private readonly IMapper _mapper;
        private readonly UserManager<ApiUser> _userManager;
        private readonly IConfiguration _configuration;
        private ApiUser _user;
        private const string _loginProvider = "HotelListingApi";
        private const string _refreshToken = "RefreshToken";


        public AuthManager(IMapper mapper, UserManager<ApiUser> userManager, IConfiguration configuration)
        {
            this._mapper = mapper;
            this._userManager = userManager;
            this._configuration = configuration;
        }
        public async Task<IEnumerable<IdentityError>> Register(ApiUserDto userDto)
        {
            _user = _mapper.Map<ApiUser>(userDto);
            _user.UserName = userDto.Email;

            var result = await _userManager.CreateAsync(_user, userDto.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(_user, "User");
            }
            return result.Errors;
        }

        public async Task<AuthResponseDto> Login(LoginDto loginDto)
        {
            _user = await _userManager.FindByEmailAsync(loginDto.Email);
            bool isValidUser = await _userManager.CheckPasswordAsync(_user, loginDto.Password);

            if (_user == null || isValidUser == false)
            {
                return null;
            }

            var token = await GenerateToken();
            var refreshToken = await CreateRefreshToken(); // สร้าง RefreshToken ใหม่

            return new AuthResponseDto
            {
                Token = token,
                UserId = _user.Id,
                RefreshToken = refreshToken // คืนค่า RefreshToken กลับไปใน Login response
            };
        }

        public async Task<string> GenerateToken()
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

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<string> CreateRefreshToken()
        {
            await _userManager.RemoveAuthenticationTokenAsync(_user, _loginProvider, _refreshToken);
            var newRefreshToken = await _userManager.GenerateUserTokenAsync(_user, _loginProvider, _refreshToken);
            var result = await _userManager.SetAuthenticationTokenAsync(_user, _loginProvider, _refreshToken, newRefreshToken);
            return newRefreshToken;
        }

        public async Task<AuthResponseDto> VerifyRefreshToken(AuthResponseDto request)
        {
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var tokenContent = jwtSecurityTokenHandler.ReadJwtToken(request.Token);
            var username = tokenContent.Claims.ToList().FirstOrDefault(q => q.Type == JwtRegisteredClaimNames.Email)?.Value;
            _user = await _userManager.FindByNameAsync(username);

            if (_user == null)
            {
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
                return new AuthResponseDto
                {
                    Token = token,
                    UserId = _user.Id,
                    RefreshToken = await CreateRefreshToken()
                };
            }

            // กรณีที่ token ไม่ valid แต่ยังต้องการส่ง RefreshToken กลับไป
            return new AuthResponseDto
            {
                Token = null,
                UserId = _user.Id,
                RefreshToken = "Invalid refresh token" // หรือค่า null ตามที่คุณต้องการ
            };
        }

    }
}