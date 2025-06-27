using Microsoft.EntityFrameworkCore;
using OneApp_minimalApi.AppContext;
using OneApp_minimalApi.Configurations;
using OneApp_minimalApi.Contracts.DockerDeployer;
using OneApp_minimalApi.Contracts.Vault;
using OneApp_minimalApi.Interfaces;
using OneApp_minimalApi.Models;

namespace OneApp_minimalApi.Services;

/// <summary>
/// Provides services for managing Docker configurations.
/// </summary>
public class DockerConfigsService : IDockerConfigsService
{
    private readonly ApplicationContext _context; // Database context
    private readonly ILogger<FilesDetailService> _logger; // Logger for logging information and errors
    private readonly IUtilityServices _utilityServices; // Utility services for encryption and other utilities
    private readonly IHashicorpVaultService _hashicorpVaultService;
    private const string DockerParameters = "DockerParameters";

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerConfigsService"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="logger">The logger for logging information and errors.</param>
    /// <param name="utilityServices">The utility services for encryption and other utilities.</param>
    public DockerConfigsService(ApplicationContext context, ILogger<FilesDetailService> logger, IUtilityServices utilityServices, IHashicorpVaultService hashicorpVaultService)
    {
        _context = context;
        _logger = logger;
        _utilityServices = utilityServices;
        _hashicorpVaultService = hashicorpVaultService;
    }

    /// <summary>
    /// Retrieves the list of Docker configurations from the database.
    /// </summary>
    /// <returns>A list of Docker configurations ordered by name.</returns>
    public async Task<IEnumerable<DockerConfigListDto>> GetDockerConfigList()
    {
        try
        {
            var configsList = await _context.DockerConfig.Where(x => x.IsActive)
                .Select(i => new DockerConfigListDto()
                {
                    Id = i.Id,
                    Name = i.Name,
                    Description = i.Description,
                    DockerIcon = i.Icon
                })
                .OrderBy(x => x.Name).ToListAsync();

            return configsList;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error reading Docker configurations: {ex.Message}");
            return null!;
        }
    }

    /// <summary>
    /// Retrieves a Docker configuration by its ID.
    /// </summary>
    /// <param name="id">The unique identifier of the Docker configuration.</param>
    /// <returns>The Docker configuration details.</returns>
    public async Task<DockerConfigsDto?> GetDockerConfig(int id)
    {
        try
        {
            var dockerConfig = await _context.DockerConfig.Include(x=>x.NasSettings)
                .Include(x=>x.DockerRepositorySettings)
                .Where(x=>x.Id==id).FirstOrDefaultAsync();
            if (dockerConfig == null)
            {
                _logger.LogWarning($"Docker configuration with ID {id} not found.");
                return null!;
            }

            return Mapper.FromDockerModelToDto(dockerConfig);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving Docker configuration: {ex.Message}");
            return null!;
        }
    }

    /// <summary>
    /// Adds a new Docker configuration.
    /// </summary>
    /// <param name="dockerConfigs">The Docker configuration data to add.</param>
    /// <returns>The added Docker configuration details.</returns>
    public async Task<DockerConfigsDto?> AddDockerConfig(DockerConfigsDto dockerConfigs)
    {
        try
        {
            //dockerConfigs.Password = await _utilityServices.EncryptString(dockerConfigs.Password);

            if (string.IsNullOrEmpty(dockerConfigs.ImageVersion))
            {
                dockerConfigs.ImageVersion = "1.0";
            }

            var dockerConfig = Mapper.FromDockerConfigDtoToDockerModel(dockerConfigs);

            if (dockerConfigs.SettingNasId == 0)
            {
                dockerConfig.NasSettings = null;    
            }
            else
            {
                dockerConfig.NasSettings = await _context.DDSettings.FindAsync(dockerConfigs.SettingNasId);
            }

            if (dockerConfigs.SettingRegistryId == 0)
            {
                dockerConfig.DockerRepositorySettings = null;    
            }
            else
            {
                dockerConfig.DockerRepositorySettings = await _context.DDSettings.FindAsync(dockerConfigs.SettingRegistryId);
            }
            
            _context.DockerConfig.Add(dockerConfig);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Docker configuration added successfully.");

            return dockerConfigs;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error adding Docker configuration: {ex.Message}");
            return null!;
        }
    }

    /// <summary>
    /// Updates an existing Docker configuration.
    /// </summary>
    /// <param name="id">The unique identifier of the Docker configuration to update.</param>
    /// <param name="dockerConfigsDto">The updated Docker configuration data.</param>
    /// <returns>The updated Docker configuration details.</returns>
    public async Task<DockerConfigsDto?> UpdateDockerConfig(int id, DockerConfigsDto dockerConfigsDto)
    {
        try
        {
            var existingItem = await _context.DockerConfig.FindAsync(id);
            if (existingItem == null)
            {
                _logger.LogWarning($"Docker configuration with ID {id} not found.");
                return null;
            }

            existingItem.LastUpdatedDate = DateTime.Now;

            if (!string.IsNullOrEmpty(dockerConfigsDto.Name)) existingItem.Name = dockerConfigsDto.Name;
            existingItem.Description = dockerConfigsDto.Description;
            existingItem.Note = dockerConfigsDto.Note;
            existingItem.AppEntryName = dockerConfigsDto.AppEntryName;
            existingItem.AppName = dockerConfigsDto.AppName;
            existingItem.Branch = dockerConfigsDto.Branch;
            existingItem.BuildProject = dockerConfigsDto.BuildProject;
            // existingItem.DockerCommand = dockerConfigsDto.DockerCommand;
            existingItem.DockerFileName = dockerConfigsDto.DockerFileName;
            existingItem.FolderContainer1 = dockerConfigsDto.FolderContainer1;
            existingItem.FolderContainer2 = dockerConfigsDto.FolderContainer2;
            existingItem.FolderContainer3 = dockerConfigsDto.FolderContainer3;
            existingItem.FolderFrom1 = dockerConfigsDto.FolderFrom1;
            existingItem.FolderFrom2 = dockerConfigsDto.FolderFrom2;
            // existingItem.Host = dockerConfigsDto.Host;
            existingItem.NasLocalFolderPath = dockerConfigsDto.NasLocalFolderPath;
            // existingItem.Password = dockerConfigsDto.Password;
            existingItem.FolderFrom3 = dockerConfigsDto.FolderFrom3;
            existingItem.Icon = dockerConfigsDto.Icon;
            // existingItem.User = dockerConfigsDto.User;
            existingItem.PortAddress = dockerConfigsDto.PortAddress;
            existingItem.SkdVersion = dockerConfigsDto.SkdVersion;
            existingItem.SolutionFolder = dockerConfigsDto.SolutionFolder;
            existingItem.SolutionRepository = dockerConfigsDto.SolutionRepository;
            existingItem.ImageVersion = dockerConfigsDto.ImageVersion;
            existingItem.noCache = dockerConfigsDto.noCache;
            
            if (dockerConfigsDto.SettingRegistryId  > 0 )
            {
                existingItem.DockerRepositorySettings =
                    await _context.DDSettings.FindAsync(dockerConfigsDto.SettingRegistryId);    
            }
            if (dockerConfigsDto.SettingNasId  > 0 )
            {
                existingItem.NasSettings =
                    await _context.DDSettings.FindAsync(dockerConfigsDto.SettingNasId);    
            }

            _context.DockerConfig.Update(existingItem);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Docker configuration updated successfully.");

            return Mapper.FromDockerModelToDto(existingItem);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating Docker configuration: {ex.Message}");
            return null!;
        }
    }

    /// <summary>
    /// Deletes a Docker configuration by marking it as inactive.
    /// </summary>
    /// <param name="id">The unique identifier of the Docker configuration to delete.</param>
    /// <returns>A boolean indicating whether the deletion was successful.</returns>
    public async Task<bool> DeleteDockerConfig(int id)
    {
        try
        {
            var existingItem = await _context.DockerConfig.FindAsync(id);
            if (existingItem == null)
            {
                _logger.LogWarning($"Docker configuration with ID {id} not found.");
                return false;
            }

            existingItem.IsActive = false;
            existingItem.LastUpdatedDate = DateTime.Now;

            _context.DockerConfig.Update(existingItem);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Docker configuration deleted successfully.");

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deleting Docker configuration: {ex.Message}");
            return false!;
        }
    }

    #region Parameters

    public async Task<DockerParameterDto?> GetDockerParameter(int id)
    {
        try
        {
            var dockerParameter = await _context.DockerParameters.Include(x=>x.DockerConfig)
                .Where(x=>x.Id==id && x.IsActive).FirstOrDefaultAsync();
            if (dockerParameter == null)
            {
                _logger.LogWarning($"Docker parameter with ID {id} not found.");
                return null!;
            }

            return new DockerParameterDto()
            {
                CidData = dockerParameter.CidData, Id = dockerParameter.Id,
                DockerConfigId = dockerParameter.DockerConfig.Id, ParameterName = dockerParameter.DockerParameter,
                ParameterValue = dockerParameter.ParameterValue
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving Docker parameter: {ex.Message}");
            return null!;
        }
    }
    
    public async Task<IEnumerable<DockerParameterDto>> GetDockerParametersList(int dockerConfigId)
    {
        try
        {
            var parametersList = await _context.DockerParameters.Include(x=>x.DockerConfig)
                .Where(x => x.DockerConfig.Id == dockerConfigId && x.IsActive)
                .Select(i => new DockerParameterDto()
                {
                    Id = i.Id,
                    DockerConfigId = i.DockerConfig.Id,
                    ParameterName = i.DockerParameter,
                    ParameterValue = i.ParameterValue,
                    CidData = i.CidData
                })
                .OrderBy(x => x.ParameterName).ToListAsync();

            return parametersList;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error reading Docker parameters: {ex.Message}");
            return null!;
        }
    }
    
    public async Task<DockerParameterDto?> AddDockerParameter(DockerParameterDto dockerParameterDto)
    {
        try
        {
            if (dockerParameterDto.CidData)
            {
                var vaultGuid = Guid.NewGuid().ToString();
                //save cid in vault
                var result = await _hashicorpVaultService.CreateSecret(new SecretRequestDTO()
                {
                    Key = vaultGuid, Value = dockerParameterDto.ParameterValue,
                    MountVolume = "secrets", Path = DockerParameters
                });
                if (result.Data == null)
                {
                    _logger.LogError($"Error creating secret in vault: {result.Message}");
                    return null!;
                }
                dockerParameterDto.ParameterValue = vaultGuid;
            }
            
            var dockerParameter = new DockerParameters()
            {
                CidData = dockerParameterDto.CidData,
                DockerConfig = await _context.DockerConfig.FindAsync(dockerParameterDto.DockerConfigId),
                DockerParameter = dockerParameterDto.ParameterName,
                ParameterValue = dockerParameterDto.ParameterValue, CreatedDate = DateTime.Now, IsActive = true
            };

            _context.DockerParameters.Add(dockerParameter);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Docker parameter added successfully.");

            return dockerParameterDto;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error adding Docker parameter: {ex.Message}");
            return null!;
        }
    }
    
    public async Task<DockerParameterDto?> UpdateDockerParameter(int id, DockerParameterDto dockerParameterDto)
    {
        try
        {
            var existingItem = await _context.DockerParameters.FindAsync(id);
            if (existingItem == null)
            {
                _logger.LogWarning($"Docker parameter with ID {id} not found.");
                return null;
            }
            
            if(dockerParameterDto.CidData && dockerParameterDto.ParameterValue != existingItem.ParameterValue)
            {
                //update cid in vault
                
                var result = await _hashicorpVaultService.UpdateSecret(new SecretRequestDTO()
                {
                    Key = existingItem.ParameterValue, Value = dockerParameterDto.ParameterValue,
                    MountVolume = "secrets", Path = DockerParameters
                });
                if (result.Data == null)
                {
                    _logger.LogError($"Error updating secret in vault: {result.Message}");
                    return null!;
                }
                dockerParameterDto.ParameterValue = existingItem.ParameterValue;
            }

            existingItem.LastUpdatedDate = DateTime.Now;
            existingItem.CidData = dockerParameterDto.CidData;
            existingItem.DockerConfig = await _context.DockerConfig.FindAsync(dockerParameterDto.DockerConfigId);
            existingItem.DockerParameter = dockerParameterDto.ParameterName;
            existingItem.ParameterValue = dockerParameterDto.ParameterValue;

            _context.DockerParameters.Update(existingItem);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Docker parameter updated successfully.");

            return dockerParameterDto;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating Docker parameter: {ex.Message}");
            return null!;
        }
    }

    public async Task<bool> DeleteDockerParameter(int id)
    {
        try
        {
            var existingItem = await _context.DockerParameters.FindAsync(id);
            if (existingItem == null)
            {
                _logger.LogWarning($"Docker parameter with ID {id} not found.");
                return false;
            }

            existingItem.IsActive = false;
            existingItem.LastUpdatedDate = DateTime.Now;

            _context.DockerParameters.Update(existingItem);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Docker parameter deleted successfully.");

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deleting Docker parameter: {ex.Message}");
            return false!;
        }   
    }

    #endregion

    #region Folder Mounts

    public async Task<DockerFolderMountsDto> GetDockerFolderMounts(int id)
    {
        try
        {
            var dockerFolderMounts = await _context.DockerFolderMounts.Include(x=>x.DockerConfig)
                .Where(x=>x.Id==id && x.IsActive).FirstOrDefaultAsync();
            if (dockerFolderMounts == null)
            {
                _logger.LogWarning($"Docker folder mounts with ID {id} not found.");
                return null!;
            }

            return new DockerFolderMountsDto()
            {
                Id = dockerFolderMounts.Id,
                DockerConfigId = dockerFolderMounts.DockerConfig.Id, SourceHost =  dockerFolderMounts.SourceHost,
                DestinationContainer = dockerFolderMounts.DestinationContainer
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving Docker folder mounts: {ex.Message}");
            return null!;
        }
    }

    public async Task<IEnumerable<DockerFolderMountsDto>> GetDockerFolderMountsList(int dockerConfigId)
    {
        try
        {
            var mountsList = await _context.DockerFolderMounts.Include(x=>x.DockerConfig)
                .Where(x => x.DockerConfig.Id == dockerConfigId && x.IsActive)
                .Select(i => new DockerFolderMountsDto()
                {
                    Id = i.Id,
                    DockerConfigId = i.DockerConfig.Id,
                    SourceHost = i.SourceHost,
                    DestinationContainer = i.DestinationContainer
                })
                .OrderBy(x => x.SourceHost).ToListAsync();

            return mountsList;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error reading Docker folder mounts: {ex.Message}");
            return null!;
        }
    }
    public async Task<DockerFolderMountsDto?> AddDockerFolderMounts(DockerFolderMountsDto dockerFolderMountsDto)
    {
        try
        {
            var dockerFolderMounts = new DockerFolderMounts()
            {
                DockerConfig = await _context.DockerConfig.FindAsync(dockerFolderMountsDto.DockerConfigId),
                SourceHost = dockerFolderMountsDto.SourceHost,
                DestinationContainer = dockerFolderMountsDto.DestinationContainer,
                CreatedDate = DateTime.Now, IsActive = true
            };

            _context.DockerFolderMounts.Add(dockerFolderMounts);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Docker folder mounts added successfully.");

            return dockerFolderMountsDto;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error adding Docker folder mounts: {ex.Message}");
            return null!;
        }
    }
    
    public async Task<DockerFolderMountsDto> UpdateDockerFolderMounts(int id, DockerFolderMountsDto dockerFolderMountsDto)
    {
        try
        {
            var existingItem = await _context.DockerFolderMounts.FindAsync(id);
            if (existingItem == null)
            {
                _logger.LogWarning($"Docker folder mounts with ID {id} not found.");
                return null;
            }

            existingItem.LastUpdatedDate = DateTime.Now;
            existingItem.DockerConfig = await _context.DockerConfig.FindAsync(dockerFolderMountsDto.DockerConfigId);
            existingItem.SourceHost = dockerFolderMountsDto.SourceHost;
            existingItem.DestinationContainer = dockerFolderMountsDto.DestinationContainer;

            _context.DockerFolderMounts.Update(existingItem);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Docker folder mounts updated successfully.");

            return dockerFolderMountsDto;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating Docker folder mounts: {ex.Message}");
            return null!;
        }
    }
    
    public async Task<bool> DeleteDockerFolderMounts(int id)
    {
        try
        {
            var existingItem = await _context.DockerFolderMounts.FindAsync(id);
            if (existingItem == null)
            {
                _logger.LogWarning($"Docker folder mounts with ID {id} not found.");
                return false;
            }

            existingItem.IsActive = false;
            existingItem.LastUpdatedDate = DateTime.Now;

            _context.DockerFolderMounts.Update(existingItem);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Docker folder mounts deleted successfully.");

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deleting Docker folder mounts: {ex.Message}");
            return false!;
        }   
    }

    #endregion
}