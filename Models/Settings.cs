using System.ComponentModel.DataAnnotations;
using OneApp_minimalApi.Contracts.Enum;

namespace OneApp_minimalApi.Models;

public class Settings : BaseEntity
{
    [Key] public int Id { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string? Alias { get; set; }
    public string? Address { get; set; } //https://hub.docker.com/
    public SettingType? Type { get; set; } //DD, Nas, DockerRegistry
    public string? DockerCommandPath { get; set; } //share/.../docker
    public string? DockerFilePath { get; set; } // /root/Dockerfile

}