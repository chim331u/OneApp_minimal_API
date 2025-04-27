namespace OneApp_minimalApi.Contracts.DockerDeployer;

public class DockerConfigurationDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? DockerIcon { get; set; }
    public string? Description { get; set; }
}