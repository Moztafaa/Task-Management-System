using System;
using System.ComponentModel.DataAnnotations;

namespace Task.Application.DTO;

public class ResigterUserDto
{
    [Required]
    [MinLength(5)]
    [MaxLength(50)]
    public string? Username { get; set; }

    [Required]
    [MaxLength(100)]
    [EmailAddress]
    public string? Email { get; set; }

    [Required]
    [MinLength(6)]
    public string? Password { get; set; }

    [Required]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    public string? ConfirmPassword { get; set; }

}
