using OneApp_minimalApi.Contracts.Configs;
using OneApp_minimalApi.Contracts.DockerDeployer;
using OneApp_minimalApi.Contracts.FilesDetail;
using OneApp_minimalApi.Models;

namespace OneApp_minimalApi.Configurations;

/// <summary>
/// Provides mapping methods for converting between models and DTOs.
/// </summary>
public static class Mapper
{
    /// <summary>
    /// Maps a <see cref="FilesDetail"/> model to a <see cref="FilesDetailRequest"/> DTO.
    /// </summary>
    /// <param name="filesDetail">The <see cref="FilesDetail"/> model to map.</param>
    /// <returns>A <see cref="FilesDetailRequest"/> DTO.</returns>
    public static FilesDetailRequest FromRequestToFilesDetail(FilesDetail filesDetail)
    {
        return new FilesDetailRequest()
        {
            FileCategory = filesDetail.FileCategory,
            FileSize = filesDetail.FileSize,
            Name = filesDetail.Name,
            Path = filesDetail.Path
        };
    }

    /// <summary>
    /// Maps a <see cref="DockerConfig"/> model to a <see cref="DockerConfigsDto"/> DTO.
    /// </summary>
    /// <param name="dockerConfig">The <see cref="DockerConfig"/> model to map.</param>
    /// <returns>A <see cref="DockerConfigsDto"/> DTO.</returns>
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
            Note = dockerConfig.Note,
            ImageVersion = dockerConfig.ImageVersion,
        };
    }

    /// <summary>
    /// Maps a <see cref="DockerConfigsDto"/> DTO to a <see cref="DockerConfig"/> model.
    /// </summary>
    /// <param name="dockerConfigsDto">The <see cref="DockerConfigsDto"/> DTO to map.</param>
    /// <returns>A <see cref="DockerConfig"/> model.</returns>
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

    /// <summary>
    /// Maps a <see cref="DeployDetail"/> model to a <see cref="DeployDetailDto"/> DTO.
    /// </summary>
    /// <param name="deployDetail">The <see cref="DeployDetail"/> model to map.</param>
    /// <returns>A <see cref="DeployDetailDto"/> DTO.</returns>
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

    public static Settings FromSettingDtoToSettingsModel(SettingsDto settingsDto)
    {
        return new Settings()
        {
            Id = settingsDto.Id,
            Address = settingsDto.Address,
            Alias = settingsDto.Alias,
            DockerCommandPath = settingsDto.DockerCommandPath,
            Dd_Password = settingsDto.Password,
            Dd_User = settingsDto.User,
            DockerFilePath = settingsDto.DockerFilePath,
            Note = settingsDto.Note, Type = settingsDto.Type
        };
    }

    public static SettingsDto FromSettingsToDto(Settings settings)
    {
        return new SettingsDto()
        {
            Id = settings.Id, Address = settings.Address,
            Alias = settings.Alias, DockerCommandPath = settings.DockerCommandPath,
            DockerFilePath = settings.DockerFilePath, Note = settings.Note, Type = settings.Type,
            Password = settings.Dd_Password, User = settings.Dd_User
        };
    }
}