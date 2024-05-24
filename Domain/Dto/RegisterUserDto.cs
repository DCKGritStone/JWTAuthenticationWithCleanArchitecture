using System.ComponentModel.DataAnnotations;

namespace Domain.Dto
{
    //  public record RegisterUserDto(string? Name, string? Email, string? Password);

    //OR
    public record RegisterUserDto
    {
        [Required]
        public string? Name { get; set; }

        [Required, EmailAddress]
        public string? Email { get; set; }

        [Required]
        public string? Password { get; set; }

        [Required, Compare(nameof(Password))]
        public string? ConfirmPassword { get; set; }
    }
}
