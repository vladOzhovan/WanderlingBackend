using System.ComponentModel.DataAnnotations;

namespace Wanderling.Api.Dtos
{
    public class RegisterDto
    {
        [Required]
        [MinLength(5)]
        public string? Username { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [MinLength(5)]
        public string? Password { get; set; }
    }
}
