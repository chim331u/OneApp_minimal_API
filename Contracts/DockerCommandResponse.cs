namespace OneApp_minimalApi.Contracts;

public class DockerCommandResponse<T>
{
    public T Data { get; set; }
    public string Message { get; set; }
    public bool IsSuccess { get; set; }

    public DockerCommandResponse(T data, string message, bool isSuccess)
    {
        Data = data;
        Message = message;
        IsSuccess = isSuccess;
    }
}