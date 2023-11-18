﻿using CommunityToolkit.Mvvm.Messaging;
using CustomizedShell.Models;
using Microsoft.Maui.Controls.Shapes;

namespace CustomizedShell.Pages;

public class SplashScreen : BasePage
{
    #region Private Properties
    private UserDAL _UserDAL = new();
    #endregion

    #region Constructor
    public SplashScreen()
    {
        NavigationPage.SetHasNavigationBar(this, false);
        Shell.SetNavBarIsVisible(this, false);
        Shell.SetTabBarIsVisible(this, false);

        Content = new Grid
        {
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center,
            Children =
            {
                new Border
                {
                    Padding = 0,
                    Margin = 0,
                    BackgroundColor = Application.Current.Resources["Primary"] as Color,
                    Stroke = Colors.Transparent,
                    StrokeShape = new RoundRectangle { CornerRadius = 24 },
                    Content = new Image
                    {
                        WidthRequest = 124,
                        HeightRequest = 124,
                        Source = "app_ic.png"
                    }
                }
            }
        };
    }
    #endregion

    #region Overrides
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        var users = await _UserDAL.GetAll();

        if (users != null && users.Count > 0)
        {
            var loggedInUser = GetLoggedInUser(users);
            if (loggedInUser != null)
            {
                WeakReferenceMessenger.Default.Send<InternalMessage>(new InternalMessage("signed-in"));
            }
            else
            {
                WeakReferenceMessenger.Default.Send<InternalMessage>(new InternalMessage("signed-out"));
            }
        }
        else
        {
            WeakReferenceMessenger.Default.Send<InternalMessage>(new InternalMessage("signed-out"));
        }
    }

    private User GetLoggedInUser(List<User> users)
    {
        foreach (var user in users)
        {
            if (user.IsLoggedIn)
            {
                return user;
            }
        }

        return null;
    }
    #endregion
}
