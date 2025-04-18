using fc_minimalApi.Contracts.FilesDetail;

namespace fc_minimalApi.Interfaces;

public interface IFilesDetailService
{
    Task<List<string?>> GetDbCategoryList();
    Task<IEnumerable<FilesDetailResponse>> GetFileList();
    Task<FilesDetailResponse?> GetFilesDetailById(int id);
    Task<FilesDetailResponse?> AddFileDetailAsync(FilesDetailRequest filesDetailRequest);
    Task<FilesDetailResponse?> UpdateFilesDetailAsync(int id, FilesDetailUpdateRequest filesDetail);
    Task<bool> DeleteFilesDetailAsync(int id);
    
    Task<IEnumerable<FilesDetailResponse?>> GetAllFiles(string fileCategory);
    Task<IEnumerable<FilesDetailResponse?>> GetLastViewList();

}