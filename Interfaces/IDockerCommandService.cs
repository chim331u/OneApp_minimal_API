using OneApp_minimalApi.Contracts;

namespace OneApp_minimalApi.Interfaces;

public interface IDockerCommandService
{
    #region Commands

    Task<DockerCommandResponse<string>> GetBuildCommand(int deployId);
    Task<DockerCommandResponse<string>> GetRunningContainersCommand();
    Task<DockerCommandResponse<string>> GetImageListCommand();
    Task<DockerCommandResponse<string>> GetRunContainerCommand(int deployId);
    Task<DockerCommandResponse<string>> GetPushImageCommand(int deployId);

    #endregion

    #region Actions

    Task<DockerCommandResponse<string>> SendSSHCommand(int deployId, string command);
    Task<DockerCommandResponse<string>> SendLocalhostCommand(int deployId, string command);
    Task<DockerCommandResponse<string>> CreateDockerFile(int deployId);
    Task<DockerCommandResponse<string>> UploadDockerFile(int deployId);
    Task<DockerCommandResponse<string>> BuildImage(int deployId);
    Task<DockerCommandResponse<string>> GetRemoteRunningContainers(int deployId);
    Task<DockerCommandResponse<string>> GetRemoteImageList(int deployId);
    Task<DockerCommandResponse<string>> RemoveRemoteRunningContainers(int deployId);
    Task<DockerCommandResponse<string>> RemoveRemoteImagesList(int deployId);
    Task<DockerCommandResponse<string>> RunContainer(int deployId);
    Task<DockerCommandResponse<string>> PushImage(int deployId);
    

    #endregion
}