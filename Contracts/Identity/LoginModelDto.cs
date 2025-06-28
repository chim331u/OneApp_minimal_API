using System.ComponentModel.DataAnnotations;

namespace OneApp_minimalApi.Contracts.Identity;

public class LoginModelDto
{
    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}