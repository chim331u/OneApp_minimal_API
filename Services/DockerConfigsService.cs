using Microsoft.EntityFrameworkCore;
using OneApp_minimalApi.AppContext;
using OneApp_minimalApi.Configurations;
using OneApp_minimalApi.Contracts.DockerDeployer;
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

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerConfigsService"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="logger">The logger for logging information and errors.</param>
    /// <param name="utilityServices">The utility services for encryption and other utilities.</param>
    public DockerConfigsService(ApplicationContext context, ILogger<FilesDetailService> logger, IUtilityServices utilityServices)
    {
        _context = context;
        _logger = logger;
        _utilityServices = utilityServices;
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
            var dockerConfig = await _context.DockerConfig.Include(x=>x.NasSettings).Where(x=>x.Id==id).FirstOrDefaultAsync();
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
            dockerConfigs.Password = await _utilityServices.EncryptString(dockerConfigs.Password);

            if (string.IsNullOrEmpty(dockerConfigs.ImageVersion))
            {
                dockerConfigs.ImageVersion = "1.0";
            }

            var _dockerConfig = Mapper.FromDockerConfigDtoToDockerModel(dockerConfigs);

            if (dockerConfigs.SettingNasId == 0)
            {
                _dockerConfig.NasSettings = null;    
            }
            else
            {
                _dockerConfig.NasSettings = await _context.DDSettings.FindAsync(dockerConfigs.SettingNasId);
            }

            if (dockerConfigs.SettingRegistryId == 0)
            {
                _dockerConfig.DockerRepositorySettings = null;    
            }
            else
            {
                _dockerConfig.DockerRepositorySettings = await _context.DDSettings.FindAsync(dockerConfigs.SettingRegistryId);
            }
            
            _context.DockerConfig.Add(_dockerConfig);
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
            existingItem.DockerCommand = dockerConfigsDto.DockerCommand;
            existingItem.DockerFileName = dockerConfigsDto.DockerFileName;
            existingItem.FolderContainer1 = dockerConfigsDto.FolderContainer1;
            existingItem.FolderContainer2 = dockerConfigsDto.FolderContainer2;
            existingItem.FolderContainer3 = dockerConfigsDto.FolderContainer3;
            existingItem.FolderFrom1 = dockerConfigsDto.FolderFrom1;
            existingItem.FolderFrom2 = dockerConfigsDto.FolderFrom2;
            existingItem.Host = dockerConfigsDto.Host;
            existingItem.NasLocalFolderPath = dockerConfigsDto.NasLocalFolderPath;
            existingItem.Password = dockerConfigsDto.Password;
            existingItem.FolderFrom3 = dockerConfigsDto.FolderFrom3;
            existingItem.Icon = dockerConfigsDto.Icon;
            existingItem.User = dockerConfigsDto.User;
            existingItem.PortAddress = dockerConfigsDto.PortAddress;
            existingItem.SkdVersion = dockerConfigsDto.SkdVersion;
            existingItem.SolutionFolder = dockerConfigsDto.SolutionFolder;
            existingItem.SolutionRepository = dockerConfigsDto.SolutionRepository;
            existingItem.ImageVersion = dockerConfigsDto.ImageVersion;
            
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
}