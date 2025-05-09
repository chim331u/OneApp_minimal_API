namespace OneApp_minimalApi.Contracts;

/// <summary>
/// Represents a generic response for Docker commands.
/// </summary>
/// <typeparam name="T">The type of the data included in the response.</typeparam>
public class DockerCommandResponse<T>
{
    /// <summary>
    /// Gets or sets the data included in the response.
    /// </summary>
    public T Data { get; set; }

    /// <summary>
    /// Gets or sets the message included in the response.
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the command was successful.
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerCommandResponse{T}"/> class.
    /// </summary>
    /// <param name="data">The data to include in the response.</param>
    /// <param name="message">The message to include in the response.</param>
    /// <param name="isSuccess">A value indicating whether the command was successful.</param>
    public DockerCommandResponse(T data, string message, bool isSuccess)
    {
        Data = data;
        Message = message;
        IsSuccess = isSuccess;
    }
}