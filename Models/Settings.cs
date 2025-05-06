using System.ComponentModel.DataAnnotations;

namespace OneApp_minimalApi.Models;

public class Settings : BaseEntity
{
    [Key] public int Id { get; set; }
    public string Dd_User { get; set; }
    public string Dd_Password { get; set; }
    public string? Alias { get; set; }
    public string? Address { get; set; } //https://hub.docker.com/
    public string? Type { get; set; } //DD, Nas, DockerRegistry
    public string? DockerCommandPath { get; set; } //share/.../docker
    public string? DockerFilePath { get; set; } // /root/Dockerfile

}