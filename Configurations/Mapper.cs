using fc_minimalApi.Contracts.FilesDetail;
using fc_minimalApi.Models;

namespace fc_minimalApi.Configurations;

public static class Mapper
{
    public static FilesDetailRequest FromRequestToFilesDetail(FilesDetail filesDetail)
    {
        return new FilesDetailRequest()
        { 
            FileCategory = filesDetail.FileCategory, FileSize = filesDetail.FileSize,
            Name = filesDetail.Name, Path = filesDetail.Path
         };
    }
}