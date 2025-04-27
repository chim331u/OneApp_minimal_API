using Microsoft.EntityFrameworkCore;
using OneApp_minimalApi.AppContext;
using OneApp_minimalApi.Contracts.Configs;
using OneApp_minimalApi.Interfaces;
using OneApp_minimalApi.Models;

namespace OneApp_minimalApi.Services;

public class ConfigsService:IConfigsService
{
     private readonly ApplicationContext _context; // Database context
     private readonly ILogger<ConfigsService> _logger; // Logger for logging information and error
     private readonly IConfiguration _config;

        // Constructor to initialize the database context and logger
        /// <summary>
        /// Entry point for Configs Service
        /// </summary>
        /// <param name="context">Database context</param>
        /// <param name="logger">Logger for logging information and error</param>
        public ConfigsService(ApplicationContext context, ILogger<ConfigsService> logger, IConfiguration config)    
        {
            _context = context;
            _logger = logger;
            _config = config;
        }

        /// <summary>
        /// Get the list of configs
        /// </summary>
        /// <returns>List af all active configs by env</returns>
        public async Task<IEnumerable<ConfigsDto>> GetConfigList()
        {
            try
            {
                var configsList = await _context.Configuration.AsNoTracking().OrderBy(x => x.Id)
                    .Where(x => x.IsActive && x.IsDev == _config.GetValue("IsDev", false))
                    .ToListAsync();

                // Return the config
                return configsList.Select(configsDto => new ConfigsDto()
                {
                    
                    Id = configsDto.Id,
                    Value = configsDto.Value, 
                    Key = configsDto.Key
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error config list {ex.Message}");
                return null;
            }
        }
    
        public async Task<string> GetConfigValue(string key)
        {
            try
            {
                var configValue = _context.Configuration
                    .Single(x => x.IsActive && x.IsDev == _config.GetValue("IsDev", false) && x.Key == key).Value.ToString();

                return configValue;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }
        public async Task<ConfigsDto?> GetConfig(int id)
        {
            try
            {
                // Find the files config by its ID
                var config = await _context.Configuration.FindAsync(id);
                
                if (config == null)
                {
                    _logger.LogWarning($"Configs with ID {id} not found.");
                    return null!;
                }

                // Return the details of the config
                return new ConfigsDto()
                {
                    Id = config.Id, Key = config.Key, Value = config.Value
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving config: {ex.Message}");
                return null!;
            }
        }

        /// <summary>
        /// Add new config
        /// </summary>
        /// <param name="Config key-value"></param>
        /// <returns>Details of the config</returns>
        public async Task<ConfigsDto> AddConfig(ConfigsDto config)
        {
            try
            {
                var _config = new Configs
                {
                    CreatedDate = DateTime.Now,
                    IsActive = true,
                    Key = config.Key, 
                    Value = config.Value, IsDev = this._config.GetValue("IsDev", false)
                };

                // Add the config to the database
                _context.Configuration.Add(_config);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Config successfully.");

                // Return the details of the created config
                return config;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error adding configs: {ex.Message}");
                return null!;
            }
        }

        /// <summary>
        /// Update an existing config
        /// </summary>
        /// <param name="id">ID of the config to be updated</param>
        /// <param name="configDto">Key Value Config</param>
        /// <returns>Details of the updated config</returns>
        public async Task<ConfigsDto> UpdateConfig(int id, ConfigsDto config)
        {
            try
            {
                // Find the existing config by its ID
                var existingConfig = await _context.Configuration.FindAsync(id);
                if (existingConfig == null)
                {
                    _logger.LogWarning($"Config with ID {id} not found.");
                    return null!;
                }

                // Update the config
                
                existingConfig.Key = config.Key;
                existingConfig.Value = config.Value;
                existingConfig.LastUpdatedDate = DateTime.Now;
                
                
                _context.Configuration.Update(existingConfig);
                // Save the changes to the database
                await _context.SaveChangesAsync();
                _logger.LogInformation("Config updated successfully.");

                // Return the files details of the updated filesdetail
                return new ConfigsDto()
                {
                    Id = existingConfig.Id, Key = existingConfig.Key, Value = existingConfig.Value
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating Config: {ex.Message}");
                return null!;
            }
        }
        
        /// <summary>
        /// Delete an existing Config
        /// </summary>
        /// <param name="id">ID of the config to be deleted</param>
        /// <returns>Details of the deleted configs</returns>
        public async Task<bool> DeleteConfig(int id)
        {
            try
            {
                // Find the existing config by its ID
                var existingConfig = await _context.Configuration.FindAsync(id);
                if (existingConfig == null)
                {
                    _logger.LogWarning($"Config with ID {id} not found.");
                    return false;
                }

                // Update the Config
                existingConfig.IsActive = false;
                existingConfig.LastUpdatedDate = DateTime.Now;
                
                // Save the changes to the database
                _context.Configuration.Update(existingConfig);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Config deleted successfully.");
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting Config: {ex.Message}");
                return false!;
            }
        }
    
}