using OneApp_minimalApi.Contracts.Enum;

namespace OneApp_minimalApi.Contracts.DockerDeployer;

/// <summary>
/// Represents a Data Transfer Object (DTO) for Docker configurations.
/// </summary>
public class DockerConfigsDto
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
    /// Gets or sets the description of the Docker configuration.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the Docker command used for the configuration.
    /// </summary>
    public string DockerCommand { get; set; }

    /// <summary>
    /// Gets or sets the solution repository URL.
    /// </summary>
    public string SolutionRepository { get; set; }

    /// <summary>
    /// Gets or sets the entry name of the application.
    /// </summary>
    public string AppEntryName { get; set; }

    /// <summary>
    /// Gets or sets the name of the application.
    /// </summary>
    public string AppName { get; set; }

    /// <summary>
    /// Gets or sets the name of the Docker file.
    /// </summary>
    public string DockerFileName { get; set; }

    /// <summary>
    /// Gets or sets the host address for the Docker configuration.
    /// </summary>
    public string Host { get; set; }

    /// <summary>
    /// Gets or sets the username for authentication.
    /// </summary>
    public string User { get; set; }

    /// <summary>
    /// Gets or sets the password for authentication.
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// Gets or sets the port address for the Docker configuration.
    /// </summary>
    public string PortAddress { get; set; }

    /// <summary>
    /// Gets or sets the NAS local folder path.
    /// </summary>
    public string NasLocalFolderPath { get; set; }

    /// <summary>
    /// Gets or sets the solution folder path.
    /// </summary>
    public string? SolutionFolder { get; set; }

    /// <summary>
    /// Gets or sets the SDK version used for the configuration.
    /// </summary>
    public string SkdVersion { get; set; }

    /// <summary>
    /// Gets or sets the first source folder path.
    /// </summary>
    public string? FolderFrom1 { get; set; }

    /// <summary>
    /// Gets or sets the first container folder path.
    /// </summary>
    public string? FolderContainer1 { get; set; }

    /// <summary>
    /// Gets or sets the second source folder path.
    /// </summary>
    public string? FolderFrom2 { get; set; }

    /// <summary>
    /// Gets or sets the second container folder path.
    /// </summary>
    public string? FolderContainer2 { get; set; }

    /// <summary>
    /// Gets or sets the third source folder path.
    /// </summary>
    public string? FolderFrom3 { get; set; }

    /// <summary>
    /// Gets or sets the third container folder path.
    /// </summary>
    public string? FolderContainer3 { get; set; }

    /// <summary>
    /// Gets or sets the restore project path.
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// Gets or sets the build project path.
    /// </summary>
    public string? BuildProject { get; set; }

    /// <summary>
    /// Gets or sets the branch name for the repository.
    /// </summary>
    public string? Branch { get; set; }

    /// <summary>
    /// Gets or sets the image version for the Docker configuration.
    /// </summary>
    public string? ImageVersion { get; set; }

    /// <summary>
    /// Get or sets if the build should be with no-cache.
    /// </summary>
    public bool noCache { get; set; }

    /// <summary>
    /// Gets or sets additional notes for the Docker configuration.
    /// </summary>
    public string? Note { get; set; }

    public int SettingNasId { get; set; }
    public string? NasAlias { get; set; }

    public string? NasAddress { get; set; } //https://hub.docker.com/

    public SettingType? NasType { get; set; } //DD, Nas, DockerRegistry
    public string? NasDockerCommandPath { get; set; } //share/.../docker
    public string? NasDockerFilePath { get; set; } // /root/Dockerfile
    public string? NasUser { get; set; }
    public string? NasPassword { get; set; }

    public int SettingRegistryId { get; set; }
    public string? RegistryAlias { get; set; }

    public string? RegistryAddress { get; set; } //https://hub.docker.com/

    public SettingType? RegistryType { get; set; } //DockerRegistry

    public string? RegistryUser { get; set; }
    
    public string? RegistryPassword { get; set; }
}