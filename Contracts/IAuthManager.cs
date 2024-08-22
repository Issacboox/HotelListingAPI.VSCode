using HotelListingAPI.VSCode.Models.User;
using Microsoft.AspNetCore.Identity;

namespace HotelListingAPI.VSCode.Contracts
{
    public interface IAuthManager
    {
        Task<IEnumerable<IdentityError>> Register(ApiUserDto userDto);
        Task<bool> Login(LoginDto loginDto);
    }
}