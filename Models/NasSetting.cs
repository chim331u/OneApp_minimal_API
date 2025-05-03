namespace OneApp_minimalApi.Models;

public class NasSetting :BaseEntity
{
    //nas
    public int Id { get; set; }
    public string? Address { get; set; }
    public string? NasPath { get; set; }
    public string? DockerCommandPath { get; set; }
    public string? DockerFilePath { get; set; }
}