namespace OneApp_minimalApi.Contracts.FilesDetail;

public class FilesDetailUpdateRequest
{
    public string? Name { get; set; }
    public string? Path { get; set; }
    public double FileSize { get; set; }
    public DateTime LastUpdateFile { get; set; }

    public string? FileCategory { get; set; }
    public bool IsToCategorize { get; set; }
    public bool IsNew { get; set; }
    public bool IsDeleted { get; set; }
    public bool IsNotToMove { get; set; }
}