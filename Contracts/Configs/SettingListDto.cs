using OneApp_minimalApi.Contracts.Enum;

namespace OneApp_minimalApi.Contracts.Configs;

public class SettingListDto
{
    public int Id { get; set; }
    public string? Alias { get; set; }
    public SettingType? Type { get; set; }
}