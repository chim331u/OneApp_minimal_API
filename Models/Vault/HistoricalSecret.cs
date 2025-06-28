using System.ComponentModel.DataAnnotations;

namespace OneApp_minimalApi.Models.Vault;

public class HistoricalSecret
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Key { get; set; }
    [Required]
    public string Value { get; set; }
    [Required]
    public DateTime SecretCreateDate { get; set; }
    
    public int SecretId { get; set; }
    [Required]
    public DateTime SecretEndDate { get; set; }
}