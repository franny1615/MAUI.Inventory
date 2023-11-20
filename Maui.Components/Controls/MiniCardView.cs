﻿using CommunityToolkit.Maui.Markup;
using Microsoft.Maui.Controls.Shapes;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;
using System.Runtime.CompilerServices;

namespace Maui.Components.Controls;

public class MiniCardView : ContentView
{
    #region Events
    public event EventHandler Clicked;
    #endregion

    #region Public Properties
    public static readonly BindableProperty ImageBackgroundColorProperty = BindableProperty.Create(
        nameof(ImageBackgroundColorProperty),
        typeof(Color),
        typeof(MiniCardView),
        null);

    public Color ImageBackgroundColor
    {
        get => (Color)GetValue(ImageBackgroundColorProperty);
        set => SetValue(ImageBackgroundColorProperty, value);
    }

    public static readonly BindableProperty ImageSourceProperty = BindableProperty.Create(
        nameof(ImageSourceProperty),
        typeof(ImageSource),
        typeof(MiniCardView),
        null
    );

    public ImageSource ImageSource
    {
        get => (ImageSource)GetValue(ImageSourceProperty);
        set => SetValue(ImageSourceProperty, value);
    }

    public static readonly BindableProperty TitleProperty = BindableProperty.Create(
        nameof(TitleProperty),
        typeof(string),
        typeof(MiniCardView),
        null);

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly BindableProperty CardColorProperty = BindableProperty.Create(
        nameof(CardColorProperty),
        typeof(Color),
        typeof(MiniCardView),
        null);

    public Color CardColor
    {
        get => (Color)GetValue(CardColorProperty);
        set => SetValue(CardColorProperty, value);
    }

    public static readonly BindableProperty ContentColorProperty = BindableProperty.Create(
        nameof(ContentColorProperty),
        typeof(Color),
        typeof(MiniCardView),
        null);

    public Color ContentColor
    {
        get => (Color)GetValue(ContentColorProperty);
        set => SetValue(ContentColorProperty, value);
    }
    #endregion

    #region Private Helpers
    private readonly Border _ContentContainer = new()
    {
        Stroke = Colors.Transparent,
        StrokeShape = new RoundRectangle { CornerRadius = 8 },
    };
    private readonly Grid _ContentLayout = new()
    {
        Padding = 8,
        RowDefinitions = Rows.Define(Star),
        ColumnDefinitions = Columns.Define(30, Star),
        ColumnSpacing = 8,
    };
    private readonly Border _ImageContainer = new()
    {
        Stroke = Colors.Transparent,
        StrokeShape = new RoundRectangle { CornerRadius = 8 },
        HeightRequest = 30,
        WidthRequest = 30,
    };
    private readonly Image _Image = new()
    {
        WidthRequest = 20,
        HeightRequest = 20,
    };
    private readonly Label _Title = new()
    {
        FontSize = 16,
        FontAttributes = FontAttributes.Bold,
        HorizontalTextAlignment = TextAlignment.Start,
        VerticalOptions = LayoutOptions.Center,
    };
    #endregion

    #region Constructor
    public MiniCardView()
    {
        this.TapGesture(async () =>
        {
            await this.ScaleTo(0.95, 70);
            await this.ScaleTo(1.0, 70);

            Clicked?.Invoke(this, null);
        });

        _ImageContainer.Content = _Image;

        _ContentLayout.Children.Add(_ImageContainer.Row(0).Column(0).Center());
        _ContentLayout.Children.Add(_Title.Row(0).Column(1));

        _ContentContainer.Content = _ContentLayout;
        Content = _ContentContainer;
    }
    #endregion

    #region Override
    protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        if (propertyName == ImageSourceProperty.PropertyName)
        {
            _Image.Source = ImageSource;
        }
        else if (propertyName == TitleProperty.PropertyName)
        {
            _Title.Text = Title;
        }
        else if (propertyName == ImageBackgroundColorProperty.PropertyName)
        {
            _ImageContainer.BackgroundColor = ImageBackgroundColor;
        }
        else if (propertyName == CardColorProperty.PropertyName)
        {
            _ContentContainer.BackgroundColor = CardColor;
        }
        else if (propertyName == ContentColorProperty.PropertyName)
        {
            _Title.TextColor = ContentColor;
        }
    }
    #endregion

}
