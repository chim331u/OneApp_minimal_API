using Microsoft.EntityFrameworkCore;
using OneApp_minimalApi.AppContext;
using OneApp_minimalApi.Configurations;
using OneApp_minimalApi.Contracts.DockerDeployer;
using OneApp_minimalApi.Interfaces;

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
                    DockerIcon = i.RestoreProject
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
            var dockerConfig = await _context.DockerConfig.FindAsync(id);
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
            if (!string.IsNullOrEmpty(dockerConfigsDto.Description)) existingItem.Description = dockerConfigsDto.Description;
            if (!string.IsNullOrEmpty(dockerConfigsDto.Note)) existingItem.Note = dockerConfigsDto.Note;
            if (!string.IsNullOrEmpty(dockerConfigsDto.AppEntryName)) existingItem.AppEntryName = dockerConfigsDto.AppEntryName;
            if (!string.IsNullOrEmpty(dockerConfigsDto.AppName)) existingItem.AppName = dockerConfigsDto.AppName;
            if (!string.IsNullOrEmpty(dockerConfigsDto.Branch)) existingItem.Branch = dockerConfigsDto.Branch;
            if (!string.IsNullOrEmpty(dockerConfigsDto.BuildProject)) existingItem.BuildProject = dockerConfigsDto.BuildProject;
            if (!string.IsNullOrEmpty(dockerConfigsDto.DockerCommand)) existingItem.DockerCommand = dockerConfigsDto.DockerCommand;
            if (!string.IsNullOrEmpty(dockerConfigsDto.DockerFileName)) existingItem.DockerFileName = dockerConfigsDto.DockerFileName;
            if (!string.IsNullOrEmpty(dockerConfigsDto.FolderContainer1)) existingItem.FolderContainer1 = dockerConfigsDto.FolderContainer1;
            if (!string.IsNullOrEmpty(dockerConfigsDto.FolderContainer2)) existingItem.FolderContainer2 = dockerConfigsDto.FolderContainer2;
            if (!string.IsNullOrEmpty(dockerConfigsDto.FolderContainer3)) existingItem.FolderContainer3 = dockerConfigsDto.FolderContainer3;
            if (!string.IsNullOrEmpty(dockerConfigsDto.FolderFrom1)) existingItem.FolderFrom1 = dockerConfigsDto.FolderFrom1;
            if (!string.IsNullOrEmpty(dockerConfigsDto.FolderFrom2)) existingItem.FolderFrom2 = dockerConfigsDto.FolderFrom2;
            if (!string.IsNullOrEmpty(dockerConfigsDto.Host)) existingItem.Host = dockerConfigsDto.Host;
            if (!string.IsNullOrEmpty(dockerConfigsDto.NasLocalFolderPath)) existingItem.NasLocalFolderPath = dockerConfigsDto.NasLocalFolderPath;
            if (!string.IsNullOrEmpty(dockerConfigsDto.Password)) existingItem.Password = dockerConfigsDto.Password;
            if (!string.IsNullOrEmpty(dockerConfigsDto.FolderFrom3)) existingItem.FolderFrom3 = dockerConfigsDto.FolderFrom3;
            if (!string.IsNullOrEmpty(dockerConfigsDto.RestoreProject)) existingItem.RestoreProject = dockerConfigsDto.RestoreProject;
            if (!string.IsNullOrEmpty(dockerConfigsDto.User)) existingItem.User = dockerConfigsDto.User;
            if (!string.IsNullOrEmpty(dockerConfigsDto.PortAddress)) existingItem.PortAddress = dockerConfigsDto.PortAddress;
            if (!string.IsNullOrEmpty(dockerConfigsDto.SkdVersion)) existingItem.SkdVersion = dockerConfigsDto.SkdVersion;
            if (!string.IsNullOrEmpty(dockerConfigsDto.SolutionFolder)) existingItem.SolutionFolder = dockerConfigsDto.SolutionFolder;
            if (!string.IsNullOrEmpty(dockerConfigsDto.SolutionRepository)) existingItem.SolutionRepository = dockerConfigsDto.SolutionRepository;
            if (!string.IsNullOrEmpty(dockerConfigsDto.ImageVersion)) existingItem.ImageVersion = dockerConfigsDto.ImageVersion;

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