﻿using Maui.Components.DAL;
using SQLite;

namespace CustomizedShell.Models;

[Table("user")]
public class User
{
    [PrimaryKey, AutoIncrement, Column("_id")]
    public int Id { get; set; } = -1;

    [MaxLength(250), Unique]
    public string Username { get; set; }

    [MaxLength(250), Unique]
    public string Password { get; set; }

    public string Email { get; set; }

    public bool IsLoggedIn { get; set; }
}

public class UserDAL : BaseDAL<User> { }