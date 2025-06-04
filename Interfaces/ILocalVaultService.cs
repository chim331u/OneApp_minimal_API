using OneApp_minimalApi.Contracts;
using OneApp_minimalApi.Contracts.Vault;
using OneApp_minimalApi.Models.Vault;

namespace OneApp_minimalApi.Interfaces;

public interface ILocalVaultService
{
    Task<ApiResponse<bool>> CreateDatabase();
    
    Task<ApiResponse<List<SecretsListDto>>> GetListSecrets();
    Task<ApiResponse<SecretResponseDTO>> GetSecret(string key);
    
    Task<ApiResponse<SecretResponseDTO>> StoreSecret(SecretRequestDTO secret);
    Task<ApiResponse<SecretResponseDTO>> UpdateSecret(int id, SecretRequestDTO secret);
    Task<ApiResponse<SecretResponseDTO>> UpdateSecret(int id, SecretRequestDTO secret, bool passwordChange);
    Task<ApiResponse<bool>> DeleteSecret(int id);
    Task<ApiResponse<List<HistoricalSecret>>> GetHistorySecretList();


}