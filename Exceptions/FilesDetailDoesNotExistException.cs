namespace OneApp_minimalApi.Exceptions;

public class FilesDetailDoesNotExistException: Exception
{
    private int id { get; set; }

    public FilesDetailDoesNotExistException(int id) : base($"Files detail with id {id} does not exist")
    {
        this.id = id;
    } 
    
}