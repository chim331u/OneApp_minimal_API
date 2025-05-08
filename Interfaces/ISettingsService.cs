using OneApp_minimalApi.Contracts;
using OneApp_minimalApi.Contracts.Configs;

namespace OneApp_minimalApi.Interfaces;

public interface ISettingsService
{
        Task<ApiResponse<IEnumerable<SettingsDto>>> GetSettingsList();
        Task<ApiResponse<SettingsDto>> GetSetting(int settingId);
        Task<ApiResponse<SettingsDto>> AddSetting(SettingsDto settings);
        Task<ApiResponse<SettingsDto>> UpdateSetting(int originalId, SettingsDto settings);
        Task<ApiResponse<bool>> DeleteSetting(int id);
}