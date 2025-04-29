namespace OneApp_minimalApi.Contracts.DockerDeployer;

public class DockerConfigsDto
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string? Description { get; set; }

    public string DockerCommand { get; set; }

    public string SolutionRepository { get; set; }

    public string AppEntryName { get; set; }
    public string AppName { get; set; }

    public string DockerFileName { get; set; }

    public string Host { get; set; }

    public string User { get; set; }

    public string Password { get; set; }

    public string PortAddress { get; set; }
    
    public string NasLocalFolderPath { get; set; }

    public string? SolutionFolder { get; set; }

    public string SkdVersion { get; set; }

    public string? FolderFrom1 { get; set; }
    public string? FolderContainer1 { get; set; }

    public string? FolderFrom2 { get; set; }
    public string? FolderContainer2 { get; set; }

    public string? FolderFrom3 { get; set; }
    public string? FolderContainer3 { get; set; }

    public string? RestoreProject { get; set; }
    public string? BuildProject { get; set; }
    public string? Branch { get; set; }
    
    public string? ImageVersion { get; set; }
    public string? Note { get; set; }
}