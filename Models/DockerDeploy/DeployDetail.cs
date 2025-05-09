using System.ComponentModel.DataAnnotations;

namespace OneApp_minimalApi.Models
{
    public class DeployDetail : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        public DateTime DeployStart { get; set; }
        public DateTime DeployEnd { get; set; }
        public string? Duration { get; set; }

        public bool? Result { get; set; }

        public string? LogFilePath { get; set; }

        public DockerConfig? DockerConfig { get; set; }

    }
}
