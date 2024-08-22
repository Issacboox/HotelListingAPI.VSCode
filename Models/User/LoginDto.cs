using System.ComponentModel.DataAnnotations;

namespace HotelListingAPI.VSCode.Models.User
{
    public class LoginDto
    {

        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [StringLength(15, ErrorMessage = "Your password is limited to {2} to {1} chareacters")]
        public string Password { get; set; }
    }
}