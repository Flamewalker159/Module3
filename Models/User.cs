using System;

namespace Module3.Models;

public class User
{
    public int Id { get; set; }

    public string Login { get; set; }

    public string Password { get; set; }

    public bool IsFirstLogin { get; set; }

    public bool IsAdmin { get; set; }

    public bool IsBlocked { get; set; }

    public int CounterFailed { get; set; }

    public DateTime? LastLogin { get; set; }
}