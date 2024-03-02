﻿using CommunityToolkit.Maui.Markup;
using System.Runtime.CompilerServices;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Maui.Components.Controls;

public class PickedEventArgs : EventArgs
{
    public string PickedItem { get; set; } = string.Empty;
}

public class MaterialPicker : ContentView
{
    #region Events
    public event EventHandler<PickedEventArgs> PickedItem;
    #endregion

    #region Public Properties
    public static readonly BindableProperty IconProperty = BindableProperty.Create(
        nameof(Icon),
        typeof(string),
        typeof(MaterialPicker),
        null);

    public string Icon
    {
        get => (string)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text),
        typeof(string),
        typeof(MaterialPicker),
        null);

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
        nameof(ItemsSource),
        typeof(List<string>),
        typeof(MaterialPicker),
        null);

    public List<string> ItemsSource
    {
        get => (List<string>)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(
        nameof(SelectedItem),
        typeof(string),
        typeof(MaterialPicker),
        null);

    public string SelectedItem
    {
        get => (string)(GetValue(SelectedItemProperty));
        set => SetValue(SelectedItemProperty, value);
    }
    #endregion

    #region Private Properties
    private readonly Grid _ContentLayout = new() { ColumnDefinitions = Columns.Define(30, Star, Auto), ColumnSpacing = 8 };
    private readonly MaterialImage _Icon = new() { IconSize = 25, };
    private readonly Label _Text = new() { FontSize = 16, FontAttributes = FontAttributes.None, HorizontalOptions = LayoutOptions.Start };
    private readonly Picker _Picker = new() { FontSize = 16 };
    #endregion

    #region Constructor
    public MaterialPicker()
    {
        _Text.SetDynamicResource(Label.TextColorProperty, "TextColor");
        _Icon.SetDynamicResource(MaterialImage.IconColorProperty, "TextColor");

        _ContentLayout.Add(_Icon.Column(0).Center());
        _ContentLayout.Add(_Text.Column(1).Start().CenterVertical());
        _ContentLayout.Add(_Picker.Column(2).Center());

        Content = _ContentLayout;

        _Picker.SelectedIndexChanged += SelectedIndexChanged;
    }
    ~MaterialPicker()
    {
        _Picker.SelectedIndexChanged -= SelectedIndexChanged;
    }
    #endregion

    #region Helpers
    protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        if (propertyName == IconProperty.PropertyName)
        {
            _Icon.Icon = Icon;
        }
        else if (propertyName == TextProperty.PropertyName)
        {
            _Text.Text = Text;
        }
        else if (propertyName == ItemsSourceProperty.PropertyName)
        {
            _Picker.ItemsSource = ItemsSource;
        }
        else if (propertyName == SelectedItemProperty.PropertyName)
        {
            _Picker.SelectedItem = SelectedItem;
        }
    }

    private void SelectedIndexChanged(object sender, EventArgs e)
    {
        PickedItem?.Invoke(this, new PickedEventArgs
        {
            PickedItem = _Picker.SelectedItem.ToString(),
        });
    }
    #endregion
}
