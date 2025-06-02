using OneApp_minimalApi.Contracts.DD;

namespace OneApp_minimalApi.Interfaces;

public interface IDDService
{
    Task<string> GetLinks(string link);
    Task<string> GetLinks(int threadId);
    Task<string> UseLink(int linkId);
    Task<List<ThreadsDto>> GetActiveThreads();
    Task<List<Ed2kLinkDto>> GetActiveLinks(int threadId);
    Task<DDSettingDto> AddSetting(DDSettingDto dto);
}


       