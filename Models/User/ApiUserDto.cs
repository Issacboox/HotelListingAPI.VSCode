using System.ComponentModel.DataAnnotations;

namespace HotelListingAPI.VSCode.Models.User
{
    public class ApiUserDto : LoginDto
    {
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

    }
}


