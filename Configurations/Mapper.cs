using OneApp_minimalApi.Contracts.DockerDeployer;
using OneApp_minimalApi.Contracts.FilesDetail;
using OneApp_minimalApi.Models;

namespace OneApp_minimalApi.Configurations;

public static class Mapper
{
    public static FilesDetailRequest FromRequestToFilesDetail(FilesDetail filesDetail)
    {
        return new FilesDetailRequest()
        {
            FileCategory = filesDetail.FileCategory, FileSize = filesDetail.FileSize,
            Name = filesDetail.Name, Path = filesDetail.Path
        };
    }

    public static DockerConfigsDto FromDockerModelToDto(DockerConfig dockerConfig)
    {
        return new DockerConfigsDto()
        {
            Id = dockerConfig.Id,
            Name = dockerConfig.Name,
            Description = dockerConfig.Description,
            RestoreProject = dockerConfig.RestoreProject,
            AppEntryName = dockerConfig.AppEntryName,
            AppName = dockerConfig.AppName,
            Branch = dockerConfig.Branch,
            BuildProject = dockerConfig.BuildProject,
            DockerCommand = dockerConfig.DockerCommand,
            DockerFileName = dockerConfig.DockerFileName,
            FolderContainer1 = dockerConfig.FolderContainer1,
            FolderContainer2 = dockerConfig.FolderContainer2,
            FolderContainer3 = dockerConfig.FolderContainer3,
            FolderFrom1 = dockerConfig.FolderFrom1,
            FolderFrom2 = dockerConfig.FolderFrom2,
            FolderFrom3 = dockerConfig.FolderFrom3,
            Host = dockerConfig.Host,
            NasLocalFolderPath = dockerConfig.NasLocalFolderPath,
            Password = dockerConfig.Password,
            PortAddress = dockerConfig.PortAddress,
            SkdVersion = dockerConfig.SkdVersion,
            SolutionFolder = dockerConfig.SolutionFolder,
            SolutionRepository = dockerConfig.SolutionRepository,
            User = dockerConfig.User,
            Note = dockerConfig.Note, ImageVersion = dockerConfig.ImageVersion,
        };
    }

    public static DockerConfig FromDockerConfigDtoToDockerModel(DockerConfigsDto dockerConfigsDto)
    {
        return new DockerConfig()
        {
            Id = dockerConfigsDto.Id,
            IsActive = true,
            CreatedDate = DateTime.Now,
            LastUpdatedDate = DateTime.Now,
            Name = dockerConfigsDto.Name,
            Description = dockerConfigsDto.Description,
            RestoreProject = dockerConfigsDto.RestoreProject,
            AppEntryName = dockerConfigsDto.AppEntryName,
            AppName = dockerConfigsDto.AppName,
            Branch = dockerConfigsDto.Branch,
            BuildProject = dockerConfigsDto.BuildProject,
            DockerCommand = dockerConfigsDto.DockerCommand,
            DockerFileName = dockerConfigsDto.DockerFileName,
            FolderContainer1 = dockerConfigsDto.FolderContainer1,
            FolderContainer2 = dockerConfigsDto.FolderContainer2,
            FolderContainer3 = dockerConfigsDto.FolderContainer3,
            FolderFrom1 = dockerConfigsDto.FolderFrom1,
            FolderFrom2 = dockerConfigsDto.FolderFrom2,
            FolderFrom3 = dockerConfigsDto.FolderFrom3,
            Host = dockerConfigsDto.Host,
            NasLocalFolderPath = dockerConfigsDto.NasLocalFolderPath,
            Password = dockerConfigsDto.Password,
            PortAddress = dockerConfigsDto.PortAddress,
            SkdVersion = dockerConfigsDto.SkdVersion,
            SolutionFolder = dockerConfigsDto.SolutionFolder,
            SolutionRepository = dockerConfigsDto.SolutionRepository,
            User = dockerConfigsDto.User,
            Note = dockerConfigsDto.Note,
            ImageVersion = dockerConfigsDto.ImageVersion
        };
    }

    public static DeployDetailDto FromDeployDetailToDto(DeployDetail deployDetail)
    {
        return new DeployDetailDto()
        {
            Id = deployDetail.Id,
            DeployEnd = deployDetail.DeployEnd,
            DeployStart = deployDetail.DeployStart,
            Duration = deployDetail.Duration,
            DockerConfigId = deployDetail.DockerConfig.Id,
            Note = deployDetail.Note,
            LogFilePath = deployDetail.LogFilePath,
            Result = deployDetail.Result,
        };
    }
}