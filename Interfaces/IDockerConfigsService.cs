using OneApp_minimalApi.Contracts.DockerDeployer;

namespace OneApp_minimalApi.Interfaces;

public interface IDockerConfigsService
{
    Task<IEnumerable<DockerConfigListDto>> GetDockerConfigList();
    Task<DockerConfigsDto?> GetDockerConfig(int id);
    Task<DockerConfigsDto?> AddDockerConfig(DockerConfigsDto dockerConfigs);
    Task<DockerConfigsDto?> UpdateDockerConfig(int id, DockerConfigsDto dockerConfigs);
    Task<bool> DeleteDockerConfig(int id);

     Task<DockerParameterDto?> GetDockerParameter(int id);
    Task<IEnumerable<DockerParameterDto>> GetDockerParametersList(int dockerConfigId);
    Task<DockerParameterDto?> AddDockerParameter(DockerParameterDto dockerParameter);
    Task<DockerParameterDto?> UpdateDockerParameter(int id, DockerParameterDto dockerParameter);
    Task<bool> DeleteDockerParameter(int id);
    
    Task<DockerFolderMountsDto?> GetDockerFolderMounts(int id);
    Task<IEnumerable<DockerFolderMountsDto>> GetDockerFolderMountsList(int dockerConfigId);
    Task<DockerFolderMountsDto?> AddDockerFolderMounts(DockerFolderMountsDto dockerFolderMounts);
    Task<DockerFolderMountsDto?> UpdateDockerFolderMounts(int id, DockerFolderMountsDto dockerFolderMounts);
    Task<bool> DeleteDockerFolderMounts(int id);
    
    
}