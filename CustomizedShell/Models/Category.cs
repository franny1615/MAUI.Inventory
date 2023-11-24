﻿using Maui.Components.DAL;
using Maui.Components.Interfaces;
using SQLite;

namespace CustomizedShell.Models;

[Table("category")]
public class Category : ISearchable
{
    [PrimaryKey, AutoIncrement, Column("_id")]
    public int Id { get; set; } = -1;

    public int UserID { get; set; } = -1;

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string[] SearchableTerms => Name.Split(" ", StringSplitOptions.RemoveEmptyEntries);

    [Ignore]
    public ImageSource Icon { get; set; } = "category.png";

    [Ignore]
    public Color IconBackgroundColor { get; set; } = Application.Current.Resources["Primary"] as Color;
}

public class CategoryDAL : BaseDAL<Category>, IDAL<Category> { }
