using System.ComponentModel.DataAnnotations;

namespace OneApp_minimalApi.Models;

public class DD_Settings  : BaseEntity
{
    [Key]
    public int Id { get; set; }
    public string Dd_User { get; set; }
    public string Dd_Password { get; set; }
    
    public string Alias { get; set; }

    public NasSetting? NasSetting { get; set; }
    

    
}