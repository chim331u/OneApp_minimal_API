namespace OneApp_minimalApi.Contracts.DockerDeployer;

public class NasSettingDto
{
    /// <summary>
    /// Gets or sets the unique identifier for the setting.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the username for the setting.
    /// </summary>
    public string User { get; set; }

    /// <summary>
    /// Gets or sets the password for the setting.
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// Get or sets the NasAlias for the setting.
    /// </summary>
    public string? Alias { get; set; }
    
    /// <summary>
    /// gets or sets the address of the NAS.
    /// </summary>
    public string? Address { get; set; }
    
    /// <summary>
    /// gets or sets the Path of application in the NAS.
    /// </summary>
    public string? NasPath { get; set; }
    
    /// <summary>
    /// gets or sets the Path of Docker in the NAS.
    /// </summary>
    public string? DockerCommandPath { get; set; }
    
    /// <summary>
    /// gets or sets the Path where the Dockerfile is saved in the NAS.
    /// </summary>
    public string? DockerFilePath { get; set; }
}