using OneApp_minimalApi.Contracts.DockerDeployer;

namespace OneApp_minimalApi.Interfaces;

public interface IDockerConfigsService
{
    Task<IEnumerable<DockerConfigurationDto>> GetDockerConfigList();
    Task<DockerConfigsDto?> GetDockerConfig(int id);
    Task<DockerConfigsDto?> AddDockerConfig(DockerConfigsDto dockerConfigs);
    Task<DockerConfigsDto?> UpdateDockerConfig(int id, DockerConfigsDto dockerConfigs);
    Task<bool> DeleteDockerConfig(int id);
}