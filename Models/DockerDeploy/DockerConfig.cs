using System.ComponentModel.DataAnnotations;

namespace OneApp_minimalApi.Models
{
    public class DockerConfig : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string? Description { get; set; }

        // [Required]
        // public string DockerCommand { get; set; }

        [Required]
        public string SolutionRepository { get; set; }

        [Required]
        public string AppEntryName { get; set; }
        [Required]
        public string AppName { get; set; }

        [Required]
        public string DockerFileName { get; set; }

        [Required]
        public string Host { get; set; }

        // [Required]
        // public string User { get; set; }
        //
        // [Required]
        // public string Password { get; set; }

        [Required]
        public string PortAddress { get; set; }

        [Required]
        public string NasLocalFolderPath { get; set; }

        public string? SolutionFolder { get; set; }

        [Required]
        public string SkdVersion { get; set; }

        public string? FolderFrom1 { get; set; }
        public string? FolderContainer1 { get; set; }

        public string? FolderFrom2 { get; set; }
        public string? FolderContainer2 { get; set; }

        public string? FolderFrom3 { get; set; }
        public string? FolderContainer3 { get; set; }

        public string? Icon { get; set; }
        public string? BuildProject { get; set; }
        public string? Branch { get; set; }
        
        public string? ImageVersion { get; set; }
        public bool noCache { get; set; } = false;
        
        public Settings? NasSettings { get; set; }

        public Settings? DockerRepositorySettings { get; set; }

        public ICollection<DeployDetail>? DeployDetails { get; set; }
    }
}