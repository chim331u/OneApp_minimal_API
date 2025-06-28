using System.ComponentModel.DataAnnotations;

namespace OneApp_minimalApi.Models.Identity;

public class TokenModelDto
{
    [Required]
    public string AccessToken { get; set; } = string.Empty;

    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}