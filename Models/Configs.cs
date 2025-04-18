using System.ComponentModel.DataAnnotations;

namespace fc_minimalApi.Models
{
    public class Configs : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? Key { get; set; }
        [Required]
        public string? Value { get; set; }
        public bool IsDev { get; set; }

    }
}
