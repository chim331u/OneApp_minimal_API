namespace OneApp_minimalApi.Contracts.DockerDeployer;

public class DeployDetailDto
{
    public int Id { get; set; }

    public DateTime DeployStart { get; set; }
    public DateTime DeployEnd { get; set; }
    public string? Duration { get; set; }

    public bool? Result { get; set; }

    public string? LogFilePath { get; set; }

    public int DockerConfigId { get; set; }

    public string Note { get; set; }
    
}