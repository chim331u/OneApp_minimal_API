namespace OneApp_minimalApi.Contracts.DockerDeployer;

public class DockerParameterDto
{
    public int Id { get; set; }
    public string ParameterName { get; set; }
    public string ParameterValue { get; set; }
    public bool CidData { get; set; } = false;
    public int DockerConfigId { get; set; }
    
}