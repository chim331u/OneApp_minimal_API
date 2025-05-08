using Microsoft.EntityFrameworkCore;
using OneApp_minimalApi.AppContext;
using OneApp_minimalApi.Configurations;
using OneApp_minimalApi.Contracts;
using OneApp_minimalApi.Contracts.Configs;
using OneApp_minimalApi.Interfaces;

namespace OneApp_minimalApi.Services;

public class SettingsService : ISettingsService
{
    private readonly ApplicationContext _context; // Database context
    private readonly ILogger<SettingsService> _logger; // Logger for logging information and errors
    private readonly IConfiguration _config;
    private readonly IUtilityServices _utilityServices;

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsService"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="logger">The logger for logging information and errors.</param>
    /// <param name="config">The application configuration.</param>
    public SettingsService(ApplicationContext context, ILogger<SettingsService> logger, IConfiguration config,
        IUtilityServices utilityServices)
    {
        _context = context;
        _logger = logger;
        _config = config;
        _utilityServices = utilityServices;
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