using Microsoft.EntityFrameworkCore;
using OneApp_minimalApi.AppContext;
using OneApp_minimalApi.Configurations;
using OneApp_minimalApi.Contracts.DockerDeployer;
using OneApp_minimalApi.Interfaces;

namespace OneApp_minimalApi.Services;

public class DockerConfigsService : IDockerConfigsService
{
    private readonly ApplicationContext _context; // Database context
    private readonly ILogger<FilesDetailService> _logger; // Logger for logging information and error
    private readonly IUtilityServices _utilityServices; // Utility services for encryption and other utilities

    public DockerConfigsService(ApplicationContext context, ILogger<FilesDetailService> logger,
        IUtilityServices utilityServices)
    {
        _context = context;
        _logger = logger;
        _utilityServices = utilityServices;
    }


    /// <summary>
    /// Get the list of Docker configurations from the database.
    /// </summary>
    /// <returns>Docker Config list orderd by name</returns>
    public async Task<IEnumerable<DockerConfigurationDto>> GetDockerConfigList()
    {
        try
        {
            var configsList = await _context.DockerConfig.Where(x => x.IsActive)
                .Select(i => new DockerConfigurationDto()
                    { Id = i.Id, Name = i.Name, Description = i.Description, DockerIcon = i.RestoreProject })
                .OrderBy(x => x.Name).ToListAsync();

            return configsList;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error reading docker configs: {ex.Message}");
            return null!;
        }
    }

    public async Task<DockerConfigsDto?> GetDockerConfig(int id)
    {
        try
        {
            // Find the Docker Config by its ID
            var dockerConfig = await _context.DockerConfig.FindAsync(id);
            if (dockerConfig == null)
            {
                _logger.LogWarning($"Docker Config with ID {id} not found.");
                return null!;
            }

            // Return the details of the docker config
            return Mapper.FromDockerModelToDto(dockerConfig);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving docker config: {ex.Message}");
            return null!;
        }
    }
    
    public async Task<DockerConfigsDto?> AddDockerConfig(DockerConfigsDto dockerConfigs)
    {
        try
        {
            dockerConfigs.Password = await _utilityServices.EncryptString(dockerConfigs.Password);
            var _dockerConfig = Mapper.FromDockerConfigDtoToDockerModel(dockerConfigs);

            // Add the book to the database
            _context.DockerConfig.Add(_dockerConfig);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Docker Configuration added successfully.");

            // Return the details of the created docker config
            return dockerConfigs;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error adding docker configuration: {ex.Message}");
            return null!;
        }
    }

    public async Task<DockerConfigsDto?> UpdateDockerConfig(int id, DockerConfigsDto dockerConfigsDto)
    {
        try
        {
            // Find the existing item by its ID
            var existingItem = await _context.DockerConfig.FindAsync(id);
            if (existingItem == null)
            {
                _logger.LogWarning($"Docker Configuration with ID {id} not found.");
                return null;
            }

            // Update the item
            existingItem.LastUpdatedDate = DateTime.Now;
            if (!string.IsNullOrEmpty(dockerConfigsDto.Name)) existingItem.Name = dockerConfigsDto.Name;
            if (!string.IsNullOrEmpty(dockerConfigsDto.Description))
                existingItem.Description = dockerConfigsDto.Description;
            if (!string.IsNullOrEmpty(dockerConfigsDto.Note)) existingItem.Note = dockerConfigsDto.Note;
            if (!string.IsNullOrEmpty(dockerConfigsDto.AppEntryName))
                existingItem.AppEntryName = dockerConfigsDto.AppEntryName;
            if (!string.IsNullOrEmpty(dockerConfigsDto.AppName)) existingItem.AppName = dockerConfigsDto.AppName;
            if (!string.IsNullOrEmpty(dockerConfigsDto.Branch)) existingItem.Branch = dockerConfigsDto.Branch;
            if (!string.IsNullOrEmpty(dockerConfigsDto.BuildProject))
                existingItem.BuildProject = dockerConfigsDto.BuildProject;
            if (!string.IsNullOrEmpty(dockerConfigsDto.DockerCommand))
                existingItem.DockerCommand = dockerConfigsDto.DockerCommand;
            if (!string.IsNullOrEmpty(dockerConfigsDto.DockerFileName))
                existingItem.DockerFileName = dockerConfigsDto.DockerFileName;
            if (!string.IsNullOrEmpty(dockerConfigsDto.FolderContainer1))
                existingItem.FolderContainer1 = dockerConfigsDto.FolderContainer1;
            if (!string.IsNullOrEmpty(dockerConfigsDto.FolderContainer2))
                existingItem.FolderContainer2 = dockerConfigsDto.FolderContainer2;
            if (!string.IsNullOrEmpty(dockerConfigsDto.FolderContainer3))
                existingItem.FolderContainer3 = dockerConfigsDto.FolderContainer3;
            if (!string.IsNullOrEmpty(dockerConfigsDto.FolderFrom1))
                existingItem.FolderFrom1 = dockerConfigsDto.FolderFrom1;
            if (!string.IsNullOrEmpty(dockerConfigsDto.FolderFrom2))
                existingItem.FolderFrom2 = dockerConfigsDto.FolderFrom2;
            if (!string.IsNullOrEmpty(dockerConfigsDto.Host)) existingItem.Host = dockerConfigsDto.Host;
            if (!string.IsNullOrEmpty(dockerConfigsDto.NasLocalFolderPath))
                existingItem.NasLocalFolderPath = dockerConfigsDto.NasLocalFolderPath;
            if (!string.IsNullOrEmpty(dockerConfigsDto.Password)) existingItem.Password = dockerConfigsDto.Password;
            if (!string.IsNullOrEmpty(dockerConfigsDto.FolderFrom3))
                existingItem.FolderFrom3 = dockerConfigsDto.FolderFrom3;
            if (!string.IsNullOrEmpty(dockerConfigsDto.RestoreProject))
                existingItem.RestoreProject = dockerConfigsDto.RestoreProject;
            if (!string.IsNullOrEmpty(dockerConfigsDto.User)) existingItem.User = dockerConfigsDto.User;
            if (!string.IsNullOrEmpty(dockerConfigsDto.PortAddress))
                existingItem.PortAddress = dockerConfigsDto.PortAddress;
            if (!string.IsNullOrEmpty(dockerConfigsDto.SkdVersion))
                existingItem.SkdVersion = dockerConfigsDto.SkdVersion;
            if (!string.IsNullOrEmpty(dockerConfigsDto.SolutionFolder))
                existingItem.SolutionFolder = dockerConfigsDto.SolutionFolder;
            if (!string.IsNullOrEmpty(dockerConfigsDto.SolutionRepository))
                existingItem.SolutionRepository = dockerConfigsDto.SolutionRepository;
            if (!string.IsNullOrEmpty(dockerConfigsDto.RestoreProject))
                existingItem.RestoreProject = dockerConfigsDto.RestoreProject;


            // Save the changes to the database
            _context.DockerConfig.Update(existingItem);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Docker Config updated successfully.");

            // Return the docker config dto
            return Mapper.FromDockerModelToDto(existingItem);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating Docker Config: {ex.Message}");
            return null!;
        }
    }

    public async Task<bool> DeleteDockerConfig(int id)
    {
        try
        {
            // Find the existing item by its ID
            var existingItem = await _context.DockerConfig.FindAsync(id);
            if (existingItem == null)
            {
                _logger.LogWarning($"Docker Config with ID {id} not found.");
                return false;
            }

            // Update the Docker Config
            existingItem.IsActive = false;
            existingItem.LastUpdatedDate = DateTime.Now;

            // Save the changes to the database
            _context.DockerConfig.Update(existingItem);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Docker Config deleted successfully.");

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deleting Docker Config: {ex.Message}");
            return false!;
        }
    }
}