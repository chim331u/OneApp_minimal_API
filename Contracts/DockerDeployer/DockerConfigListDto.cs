namespace OneApp_minimalApi.Contracts.DockerDeployer;

/// <summary>
/// Represents a Data Transfer Object (DTO) for Docker configurations.
/// </summary>
public class DockerConfigListDto
{
    /// <summary>
    /// Gets or sets the unique identifier for the Docker configuration.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the Docker configuration.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the icon associated with the Docker configuration.
    /// </summary>
    public string? DockerIcon { get; set; }

    /// <summary>
    /// Gets or sets the description of the Docker configuration.
    /// </summary>
    public string? Description { get; set; }
}