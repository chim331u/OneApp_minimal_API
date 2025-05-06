namespace OneApp_minimalApi.Contracts.Configs;

public class SettingsDto
{
    public int Id { get; set; }
    public string User { get; set; }
    public string Password { get; set; }

    public string? Alias { get; set; }

    public string? Address { get; set; } //https://hub.docker.com/

    public string? Type { get; set; } //DD, Nas, DockerRegistry
    public string? DockerCommandPath { get; set; } //share/.../docker
    public string? DockerFilePath { get; set; } // /root/Dockerfile
    public string? Note { get; set; }
}