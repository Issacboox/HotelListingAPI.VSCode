using HotelListingAPI.VSCode.Data;
using HotelListingAPI.VSCode.Models.User;
using Microsoft.AspNetCore.Identity;

namespace HotelListingAPI.VSCode.Contracts
{
    public interface IAuthManager
    {
        Task<IEnumerable<IdentityError>> Register(ApiUserDto userDto);
        Task<AuthResponseDto> Login(LoginDto loginDto);
        Task<string> GenerateToken();
        Task<string> CreateRefreshToken();
        Task<AuthResponseDto> VerifyRefreshToken(AuthResponseDto request);
    }
}