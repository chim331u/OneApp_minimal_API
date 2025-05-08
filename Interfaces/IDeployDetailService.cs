using OneApp_minimalApi.Contracts.DockerDeployer;

namespace OneApp_minimalApi.Interfaces;

public interface IDeployDetailService
{
    Task<DeployDetailDto> GetDeployDetailById(int id);
    Task<List<DeployDetailDto>> GetDeployDetails(int dockerConfigId);
    Task<DeployDetailDto> AddDeployDetail(DeployDetailDto deployDetailDto);
    Task<DeployDetailDto> UpdateDeployDetail(int id, DeployDetailDto deployDetailDto);
    Task<bool> DeleteDeployDetail(int id);
}