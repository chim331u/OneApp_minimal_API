using fc_minimalApi.Contracts.FilesDetail;

namespace fc_minimalApi.Interfaces;

public interface IFilesDetailService
{
    Task<List<string?>> GetDbCategoryList();
    Task<IEnumerable<FilesDetailResponse>> GetFileList();
    Task<FilesDetailResponse?> GetFilesDetailById(int id);
    Task<FilesDetailResponse> AddFileDetailAsync(FilesDetailRequest filesDetailRequest);
}