namespace OneApp_minimalApi.Contracts.Vault;

public class SecretRequestDTO
{
    public string Key { get; set; }
    public string Value { get; set; } 
    public DateTime CreateDate { get; set; }
}