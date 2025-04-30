using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Module3.Models;

public class Role
{
    [Key]
    public int Id { get; set; }
    
    [MaxLength(100)]
    [DefaultValue("Пользователь")]
    public string Name { get; set; }
    
    public List<User> Users { get; set; }
}