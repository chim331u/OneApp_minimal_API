using OneApp_minimalApi.Contracts;

namespace OneApp_minimalApi.Interfaces;

public interface IDockerCommandService
{
    Task<DockerCommandResponse<string>> SendSSHCommand(int deployId, string command);
    Task<DockerCommandResponse<string>> SendLocalhostCommand(int deployId, string command);
    Task<DockerCommandResponse<string>> CreateDockerFile(int deployId);
    Task<DockerCommandResponse<string>> BuildCommand(int deployId);
}