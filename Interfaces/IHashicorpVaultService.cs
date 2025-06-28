using OneApp_minimalApi.Contracts;
using OneApp_minimalApi.Contracts.Vault;

namespace OneApp_minimalApi.Interfaces;

public interface IHashicorpVaultService
{
    Task<ApiResponse<SecretResponseDTO>> GetSecret(string key, string path, string mountPoint);
    Task<ApiResponse<List<string>?>> GetListSecretsKeys(string path, string mountPoint);
    Task<ApiResponse<SecretResponseDTO>> CreateSecret(SecretRequestDTO secret);
    Task<ApiResponse<SecretResponseDTO>> UpdateSecret(SecretRequestDTO secret);
}