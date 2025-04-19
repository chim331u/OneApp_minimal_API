using fc_minimalApi.Contracts.FilesDetail;

namespace fc_minimalApi.Interfaces
{
    public interface IHangFireJobService
    {
        Task ExecuteAsync(string fileName, string destinationFolder, CancellationToken cancellationToken);

        Task MoveFilesJob(List<FileMoveDto> filesToMove, CancellationToken cancellationToken);

        Task RefreshFiles(CancellationToken cancellationToken);
    }
}
