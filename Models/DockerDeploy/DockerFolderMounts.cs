using System.ComponentModel.DataAnnotations;

namespace OneApp_minimalApi.Models;

public class DockerFolderMounts : BaseEntity
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string SourceHost { get; set; }

    [Required]
    public string DestinationContainer { get; set; }

    [Required]
    public DockerConfig DockerConfig { get; set; }
    
}