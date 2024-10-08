﻿using System.Text.Json.Serialization;

namespace Inventory.API.Models;

public class User
{
    public int Id { get; set; } = -1;
    public int CompanyID { get; set; } = -1;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool IsDarkModeOn { get; set; } = false;
    public string Localization { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public bool IsCompanyOwner { get; set; } = false;
    public int InventoryPermissions { get; set; } = 0;
    public int PermissionId { get; set; } = 0;

    [JsonIgnore]
    public object PasswordHash { get; set; } = string.Empty;
    [JsonIgnore]
    public string Salt { get; set; } = string.Empty;

}
