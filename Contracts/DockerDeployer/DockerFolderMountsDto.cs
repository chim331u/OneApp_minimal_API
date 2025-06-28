namespace OneApp_minimalApi.Contracts.DockerDeployer;

public class DockerFolderMountsDto
{
    public int Id { get; set; }

    public string SourceHost { get; set; }

    public string DestinationContainer { get; set; }

    public int DockerConfigId { get; set; }
}