namespace OneApp_minimalApi.Contracts.DockerDeployer;

public class DockerCommandRequestDto
{
    public int DockerId { get; set; }
    public string Command { get; set; } = string.Empty;
    public bool IsRemote { get; set; } = true;
}