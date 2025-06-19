namespace OneApp_minimalApi.Contracts.Vault;

public class SecretRequestDTO
{
    public string Key { get; set; }
    public string Value { get; set; } 
    public string Path { get; set; } 
    public string MountVolume { get; set; } 
    
}