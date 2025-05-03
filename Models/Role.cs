using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Module3.Models;

public class Role
{
    [Key]
    public int id { get; set; }
    
    [MaxLength(100)]
    public string name { get; set; } = "Пользователь";
    
    public List<User>? users { get; set; }
}