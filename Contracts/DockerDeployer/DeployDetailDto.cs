namespace OneApp_minimalApi.Contracts.DockerDeployer;

/// <summary>
/// Represents a Data Transfer Object (DTO) for deployment details.
/// </summary>
public class DeployDetailDto
{
    /// <summary>
    /// Gets or sets the unique identifier for the deployment detail.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the start time of the deployment.
    /// </summary>
    public DateTime DeployStart { get; set; }

    /// <summary>
    /// Gets or sets the end time of the deployment.
    /// </summary>
    public DateTime DeployEnd { get; set; }

    /// <summary>
    /// Gets or sets the duration of the deployment as a string.
    /// </summary>
    public string? Duration { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the deployment was successful.
    /// </summary>
    public bool? Result { get; set; }

    /// <summary>
    /// Gets or sets the file path to the deployment log.
    /// </summary>
    public string? LogFilePath { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the associated Docker configuration.
    /// </summary>
    public int DockerConfigId { get; set; }

    /// <summary>
    /// Gets or sets additional notes about the deployment.
    /// </summary>
    public string Note { get; set; }
}