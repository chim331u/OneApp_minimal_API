namespace fc_minimalApi.Contracts.FilesDetail;

public class FilesDetailRequest
{
    public string? Name { get; set; }
    public string? Path { get; set; }
    public double FileSize { get; set; }
    public string? FileCategory { get; set; }

}