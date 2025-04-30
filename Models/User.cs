using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Module3.Models;

public class User
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Login { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Password { get; set; }
    public DateTime? LastLogin { get; set; }
    
    [DefaultValue(false)]
    public bool IsBlocked { get; set; }
    
    [DefaultValue(0)]
    public int FaileDattempts { get; set; }

    public int RoleId { get; set; }
    
    [ForeignKey("RoleId")]
    public Role Role { get; set; }
}
