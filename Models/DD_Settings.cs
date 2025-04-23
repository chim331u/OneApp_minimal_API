using System.ComponentModel.DataAnnotations;

namespace fc_minimalApi.Models;

public class DD_Settings  : BaseEntity
{
    [Key]
    public int Id { get; set; }
    public string Dd_User { get; set; }
    public string Dd_Password { get; set; }
    
}