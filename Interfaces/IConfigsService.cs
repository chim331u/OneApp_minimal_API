using OneApp_minimalApi.Contracts;
using OneApp_minimalApi.Contracts.Configs;

namespace OneApp_minimalApi.Interfaces;

public interface IConfigsService
{
    Task<IEnumerable<ConfigsDto>> GetConfigList();
    Task<ConfigsDto> GetConfig(int id);
    Task<string> GetConfigValue(string key);
    Task<ConfigsDto> UpdateConfig(int id, ConfigsDto config);
    Task<ConfigsDto> AddConfig(ConfigsDto config);
    Task<bool> DeleteConfig(int id);
    
    Task<ApiResponse<IEnumerable<SettingsDto>>> GetSettingsList();
    Task<ApiResponse<SettingsDto>> GetSetting(int settingId);
    Task<ApiResponse<SettingsDto>> AddSetting(SettingsDto settings);
    Task<ApiResponse<SettingsDto>> UpdateSetting(int originalId, SettingsDto settings);
    Task<ApiResponse<bool>> DeleteSetting(int id);

}