using OneApp_minimalApi.Contracts;
using OneApp_minimalApi.Contracts.Vault;
using OneApp_minimalApi.Interfaces;
using VaultSharp;
using VaultSharp.V1.AuthMethods;
using VaultSharp.V1.AuthMethods.UserPass;
using VaultSharp.V1.Commons;
using VaultSharp.V1.SecretsEngines.KeyValue.V2;

namespace OneApp_minimalApi.Services;

public class HashiCorpVaultService : IHashicorpVaultService
{
    private readonly ILogger<HashiCorpVaultService> _logger; // Logger for logging information and errors
    private readonly IConfiguration _configuration;

    private IVaultClient vaultClient;
    private string _vaultAddress; //= "http://127.0.0.1:8200";// Example token, replace with configuration

    private const string VaultUsername = "VAULT_U"; // Example username, replace with your actual username
    private const string VaultPassword = "VAULT_P"; // Example password, replace with your actual password
    private string _vaultUsername; /// Example username, replace with your actual username
    private string _vaultPassword;


    public HashiCorpVaultService(IConfiguration configuration, ILogger<HashiCorpVaultService> logger)
    {
        _configuration = configuration;
        _logger = logger;
        //for debug only
        _vaultUsername = (_configuration != null && _configuration.GetSection("IsDev").Value != null
            ? configuration[VaultUsername]
            : Environment.GetEnvironmentVariable(VaultUsername));

        _vaultPassword = _configuration.GetSection("IsDev").Value != null
            ? configuration[VaultPassword]
            : Environment.GetEnvironmentVariable(VaultPassword);
        
        _vaultAddress = configuration.GetConnectionString("HashicorpVaultConnection");

        if (string.IsNullOrEmpty(_vaultAddress))
        {
            _logger.LogWarning("Connection string not set for HashicorpVaultService");
        }

        if (string.IsNullOrEmpty(_vaultAddress))
        {
            _logger.LogWarning("MasterKey is not set for HashiCorpVaultService");
        }

        // Initialize one of the several auth methods.
        //IAuthMethodInfo authMethod = new TokenAuthMethodInfo(_vaultToken);
        IAuthMethodInfo authMethod = new UserPassAuthMethodInfo(_vaultUsername, _vaultPassword);

        // Initialize settings. You can also set proxies, custom delegates etc. here.
        var vaultClientSettings = new VaultClientSettings(_vaultAddress, authMethod);

        vaultClient = new VaultClient(vaultClientSettings);
    }

    public async Task<ApiResponse<SecretResponseDTO>> GetSecret(string key, string path, string mountPoint)
    {
        try
        {
            Secret<SecretData> kv2Secret = await vaultClient.V1.Secrets.KeyValue.V2
                .ReadSecretAsync(path: path, mountPoint: mountPoint);

            if (kv2Secret.Data.Data.TryGetValue(key, out var value))
            {
                _logger.LogInformation($"Key '{key}' - Psw: ********");
                // Return the secret value
                return new ApiResponse<SecretResponseDTO>(new SecretResponseDTO { Key = key, Value = value.ToString() },
                    $"Secret found in Vault at path '{path}'");
            }

            _logger.LogInformation($"Key '{key}' - Psw: not found in Vault at path '{path}'");
            // Return the secret value
            return new ApiResponse<SecretResponseDTO>(null, $"Key '{key}' not found in Vault at path '{path}'");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error search Key '{key}' at path '{path}': {ex.Message}");
            return new ApiResponse<SecretResponseDTO>(null, $"Error search Key '{key}' at path '{path}': {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<string>?>> GetListSecretsKeys(string path, string mountPoint)
    {
        try
        {
            Secret<SecretData> kv2Secret = await vaultClient.V1.Secrets.KeyValue.V2
                .ReadSecretAsync(path: path, mountPoint: mountPoint);

            if (kv2Secret.Data.Data.Keys.Any())
            {
                _logger.LogInformation($"Found '{kv2Secret.Data.Data.Keys.Count}' keys in Vault at path '{path}'");
                // Return the secret keys list
                var a = kv2Secret.Data.Data.Keys.ToList();

                return new ApiResponse<List<string>?>(kv2Secret.Data.Data.Keys.ToList(),
                    $"Found '{kv2Secret.Data.Data.Keys.Count}' keys in Vault at path '{path}'");
            }

            _logger.LogInformation($"Keys not found in Vault at path '{path}'");
            // Return the secret value
            return new ApiResponse<List<string>?>(null, $"Keys not found in Vault at path '{path}'");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error: Keys not found in Vault at path '{path}': {ex.Message}");
            return new ApiResponse<List<string>?>(null,
                $"Error: Keys not found in Vault at path '{path}': {ex.Message}");
        }
    }

    public async Task<ApiResponse<SecretResponseDTO>> CreateNewSecret(SecretRequestDTO secret)
    {
        try
        {
            var newSecret = new Dictionary<string, object> { { secret.Key, secret.Value } };
            var writtenValue =
                await vaultClient.V1.Secrets.KeyValue.V2.WriteSecretAsync(secret.Path, newSecret, null,
                    secret.MountVolume);

            return new ApiResponse<SecretResponseDTO>(
                new SecretResponseDTO() { Key = secret.Key, Value = secret.Value.ToString() },
                $"New secret added at path '{secret.Path}' with key '{secret.Key}'");
        }
        catch (Exception e)
        {
            _logger.LogError($"Unable to write secret '{secret.Key}' at path '{secret.Path}': {e.Message}");
            return new ApiResponse<SecretResponseDTO>(null,
                $"Unable to write secret '{secret.Key}' at path '{secret.Path}': {e.Message}");
        }
    }

    public async Task<ApiResponse<SecretResponseDTO>> CreateSecret(SecretRequestDTO secret)
    {
        try
        {
            var valueToBeCombined = new Dictionary<string, object> { { secret.Key, secret.Value } };

            var patchSecretDataRequest = new PatchSecretDataRequest() { Data = valueToBeCombined };
            var metadata = await vaultClient.V1.Secrets.KeyValue.V2.PatchSecretAsync(secret.Path,
                patchSecretDataRequest, mountPoint: secret.MountVolume);

            return new ApiResponse<SecretResponseDTO>(
                new SecretResponseDTO() { Key = secret.Key, Value = secret.Value.ToString() },
                $"New secret added at path '{secret.Path}' with key '{secret.Key}'");
        }
        catch (Exception e)
        {
            _logger.LogError($"Unable to write secret '{secret.Key}' at path '{secret.Path}': {e.Message}");
            return new ApiResponse<SecretResponseDTO>(null,
                $"Unable to write secret '{secret.Key}' at path '{secret.Path}': {e.Message}");
        }
    }

    public async Task<ApiResponse<SecretResponseDTO>> UpdateSecret(SecretRequestDTO secret)
    {
        try
        {
            var valueToBeCombined = new Dictionary<string, object> { { secret.Key, secret.Value } };

            var patchSecretDataRequest = new PatchSecretDataRequest() { Data = valueToBeCombined };
            var metadata = await vaultClient.V1.Secrets.KeyValue.V2.PatchSecretAsync(secret.Path,
                patchSecretDataRequest, mountPoint: secret.MountVolume);
            return new ApiResponse<SecretResponseDTO>(
                new SecretResponseDTO() { Key = secret.Key, Value = secret.Value.ToString() },
                $"Secret updated at path '{secret.Path}' with key '{secret.Key}'");
        }
        catch (Exception e)
        {
            _logger.LogError($"Unable to update secret '{secret.Key}' at path '{secret.Path}': {e.Message}");
            return new ApiResponse<SecretResponseDTO>(null,
                $"Unable to update secret '{secret.Key}' at path '{secret.Path}': {e.Message}");
        }
    }
}