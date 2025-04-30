using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Module3.Models;

public class User
{
    [Key]
    public int id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string login { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string password { get; set; }
    public DateTime? lastlogin { get; set; }
    public bool isblocked { get; set; } = false;
    public int failedattempts { get; set; } = 0;

    public int roleid { get; set; }
    
    [ForeignKey("roleid")]
    public Role role { get; set; }
}
