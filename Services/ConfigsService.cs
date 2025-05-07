using Microsoft.EntityFrameworkCore;
using OneApp_minimalApi.AppContext;
using OneApp_minimalApi.Configurations;
using OneApp_minimalApi.Contracts;
using OneApp_minimalApi.Contracts.Configs;
using OneApp_minimalApi.Interfaces;
using OneApp_minimalApi.Models;

namespace OneApp_minimalApi.Services;

/// <summary>
/// Provides methods for managing application configurations.
/// </summary>
public class ConfigsService : IConfigsService
{
    private readonly ApplicationContext _context; // Database context
    private readonly ILogger<ConfigsService> _logger; // Logger for logging information and errors
    private readonly IConfiguration _config;
    private readonly IUtilityServices _utilityServices;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigsService"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="logger">The logger for logging information and errors.</param>
    /// <param name="config">The application configuration.</param>
    public ConfigsService(ApplicationContext context, ILogger<ConfigsService> logger, IConfiguration config,
        IUtilityServices utilityServices)
    {
        _context = context;
        _logger = logger;
        _config = config;
        _utilityServices = utilityServices;
    }

    /// <summary>
    /// Retrieves the list of active configurations.
    /// </summary>
    /// <returns>A list of active configurations filtered by the environment.</returns>
    public async Task<IEnumerable<ConfigsDto>> GetConfigList()
    {
        try
        {
            var configsList = await _context.Configuration.AsNoTracking()
                .OrderBy(x => x.Id)
                .Where(x => x.IsActive && x.IsDev == _config.GetValue("IsDev", false))
                .ToListAsync();

            return configsList.Select(configsDto => new ConfigsDto
            {
                Id = configsDto.Id,
                Value = configsDto.Value,
                Key = configsDto.Key
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving config list: {ex.Message}");
            return null!;
        }
    }

    /// <summary>
    /// Retrieves the value of a configuration by its key.
    /// </summary>
    /// <param name="key">The key of the configuration.</param>
    /// <returns>The value of the configuration.</returns>
    public Task<string> GetConfigValue(string key)
    {
        try
        {
            var configValue = _context.Configuration
                .Single(x => x.IsActive && x.IsDev == _config.GetValue("IsDev", false) && x.Key == key).Value!
                .ToString();

            return Task.FromResult(configValue);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving config value: {ex.Message}");
            return Task.FromResult<string>(null!);
        }
    }

    /// <summary>
    /// Retrieves a configuration by its ID.
    /// </summary>
    /// <param name="id">The unique identifier of the configuration.</param>
    /// <returns>The configuration details.</returns>
    public async Task<ConfigsDto> GetConfig(int id)
    {
        try
        {
            var config = await _context.Configuration.FindAsync(id);

            if (config != null)
                return new ConfigsDto
                {
                    Id = config.Id,
                    Key = config.Key,
                    Value = config.Value
                };
            _logger.LogWarning($"Config with ID {id} not found.");
            return null!;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving config: {ex.Message}");
            return null!;
        }
    }

    /// <summary>
    /// Adds a new configuration.
    /// </summary>
    /// <param name="config">The configuration data to add.</param>
    /// <returns>The details of the added configuration.</returns>
    public async Task<ConfigsDto> AddConfig(ConfigsDto config)
    {
        try
        {
            var newConfig = new Configs
            {
                CreatedDate = DateTime.Now,
                IsActive = true,
                Key = config.Key,
                Value = config.Value,
                IsDev = _config.GetValue("IsDev", false)
            };

            _context.Configuration.Add(newConfig);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Config added successfully.");

            return config;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error adding config: {ex.Message}");
            return null!;
        }
    }

    /// <summary>
    /// Updates an existing configuration.
    /// </summary>
    /// <param name="id">The unique identifier of the configuration to update.</param>
    /// <param name="config">The updated configuration data.</param>
    /// <returns>The details of the updated configuration.</returns>
    public async Task<ConfigsDto> UpdateConfig(int id, ConfigsDto config)
    {
        try
        {
            var existingConfig = await _context.Configuration.FindAsync(id);
            if (existingConfig == null)
            {
                _logger.LogWarning($"Config with ID {id} not found.");
                return null!;
            }

            existingConfig.Key = config.Key;
            existingConfig.Value = config.Value;
            existingConfig.LastUpdatedDate = DateTime.Now;

            _context.Configuration.Update(existingConfig);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Config updated successfully.");

            return new ConfigsDto
            {
                Id = existingConfig.Id,
                Key = existingConfig.Key,
                Value = existingConfig.Value
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating config: {ex.Message}");
            return null!;
        }
    }

    /// <summary>
    /// Deletes an existing configuration by marking it as inactive.
    /// </summary>
    /// <param name="id">The unique identifier of the configuration to delete.</param>
    /// <returns>A boolean indicating whether the deletion was successful.</returns>
    public async Task<bool> DeleteConfig(int id)
    {
        try
        {
            var existingConfig = await _context.Configuration.FindAsync(id);
            if (existingConfig == null)
            {
                _logger.LogWarning($"Config with ID {id} not found.");
                return false;
            }

            existingConfig.IsActive = false;
            existingConfig.LastUpdatedDate = DateTime.Now;

            _context.Configuration.Update(existingConfig);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Config deleted successfully.");

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deleting config: {ex.Message}");
            return false;
        }
    }

    public async Task<ApiResponse<IEnumerable<SettingsDto>>> GetSettingsList()
    {
        try
        {
            var settingList = await _context.DDSettings.Where(x => x.IsActive).ToListAsync();

            var setDto = settingList.Select(Mapper.FromSettingsToDto);

            return new ApiResponse<IEnumerable<SettingsDto>>(setDto, "Active setting list");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving setting List: {ex.Message}");
            return new ApiResponse<IEnumerable<SettingsDto>>(null!, $"Error retrieving setting List: {ex.Message}");
        }
    }

    public async Task<ApiResponse<SettingsDto>> GetSetting(int settingId)
    {
        try
        {
            var setting = await _context.DDSettings.FindAsync(settingId);

            if (setting != null)
                return new ApiResponse<SettingsDto>(Mapper.FromSettingsToDto(setting),
                    $"Setting with id:{settingId} found ");

            return new ApiResponse<SettingsDto>(null!, $"Setting with id:{settingId} not found.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Unable to find setting: {ex.Message}");
            return new ApiResponse<SettingsDto>(null!, $"Unable to find setting: {ex.Message}");
        }
    }

    public async Task<ApiResponse<SettingsDto>> AddSetting(SettingsDto settingsDto)
    {
        try
        {
            var setting = Mapper.FromSettingDtoToSettingsModel(settingsDto);

            setting.CreatedDate = DateTime.Now;
            setting.IsActive = true;
            setting.LastUpdatedDate = DateTime.Now;
            setting.Password = await _utilityServices.EncryptString(settingsDto.Password);

            await _context.DDSettings.AddAsync(setting);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Setting with ID {settingsDto.Id} created.");
            return new ApiResponse<SettingsDto>(settingsDto, "Setting created successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error adding setting: {ex.Message}");
            return new ApiResponse<SettingsDto>(null!, $"Error adding setting: {ex.Message}");
        }
    }

    public async Task<ApiResponse<SettingsDto>> UpdateSetting(int originalId, SettingsDto settings)
    {
        try
        {
            var existingItem = await _context.DDSettings.FindAsync(originalId);

            if (existingItem == null)
            {
                _logger.LogWarning($"Setting with ID {originalId} not found.");
                return new ApiResponse<SettingsDto>(null!, $"Setting with ID {originalId} not found.");
            }

            existingItem.LastUpdatedDate = DateTime.Now;

            if (!string.IsNullOrEmpty(settings.User)) existingItem.UserName = settings.User;

            if (!settings.Password.Equals(existingItem.Password))
            {
                existingItem.Password = await _utilityServices.EncryptString(settings.Password);    
            }
            
            if (!string.IsNullOrEmpty(settings.Alias)) existingItem.Alias = settings.Alias;
            existingItem.Address = settings.Address;
            existingItem.Type = settings.Type;
            existingItem.DockerCommandPath = settings.DockerCommandPath;
            existingItem.DockerFilePath = settings.DockerFilePath;
            existingItem.Note = settings.Note;
            existingItem.Type = settings.Type;
            existingItem.Note = settings.Note;
            
            _context.DDSettings.Update(existingItem);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Setting updated successfully.");

            return new ApiResponse<SettingsDto>(Mapper.FromSettingsToDto(existingItem),
                $"Setting updated successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating Docker configuration: {ex.Message}");
            return new ApiResponse<SettingsDto>(null!, $"Error updating Docker configuration: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> DeleteSetting(int id)
    {
        try
        {
            var existingItem = await _context.DDSettings.FindAsync(id);
            if (existingItem == null)
            {
                _logger.LogWarning($"Setting with ID {id} not found.");
                return new ApiResponse<bool>(false, $"Setting with ID {id} not found.");
            }

            existingItem.IsActive = false;
            existingItem.LastUpdatedDate = DateTime.Now;

            _context.DDSettings.Update(existingItem);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Setting deleted successfully.");

            return new ApiResponse<bool>(true, $"Setting deleted successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deleting setting: {ex.Message}");
            return new ApiResponse<bool>(false, $"Error deleting setting: {ex.Message}");
        }
    }
}