using System.ComponentModel.DataAnnotations;

namespace OneApp_minimalApi.Models;

public class DockerParameters : BaseEntity
{
    [Key]
    public int Id { get; set; }

    public string DockerParameter { get; set; }
    public string ParameterValue { get; set; }
    public bool CidData { get; set; } = false;
   
    public DockerConfig DockerConfig { get; set; }
    
}