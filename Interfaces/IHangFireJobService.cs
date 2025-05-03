using OneApp_minimalApi.Contracts.FilesDetail;

namespace OneApp_minimalApi.Interfaces
{
    public interface IHangFireJobService
    {
        Task ExecuteAsync(string fileName, string destinationFolder, CancellationToken cancellationToken);

        Task MoveFilesJob(List<FileMoveDto> filesToMove, CancellationToken cancellationToken);

        Task RefreshFiles(CancellationToken cancellationToken);

        Task ExecuteFullDeploy(int dockerConfigId, CancellationToken cancellationToken);
    }
}
