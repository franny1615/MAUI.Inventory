﻿using System.Globalization;
using CommunityToolkit.Mvvm.Messaging;
using Inventory.MobileApp.Models;
using Inventory.MobileApp.Pages;
using Inventory.MobileApp.Services;

namespace Inventory.MobileApp;

public partial class App : Application
{
	public App()
	{
		Resources.MergedDictionaries.Add(new Resources.Styles.Colors());
        Resources.MergedDictionaries.Add(new Resources.Styles.Styles());

		SessionService.APIUrl = "https://192.168.1.28";

		WeakReferenceMessenger.Default.Register<InternalMsg>(this, (_, msg) =>  { CheckAppStateOn(msg.Value); });

		LanguageService.SetCulture(new CultureInfo(SessionService.CurrentLanguageCulture));
		UIService.ApplyTheme();

		CheckAppStateOn(InternalMessage.CheckAuth);
	}
	~App()
	{
		WeakReferenceMessenger.Default.UnregisterAll(this);
	}

	private void CheckAppStateOn(InternalMessage message, bool fromOnResume = false)
	{
		if (message == InternalMessage.LoggedIn)
		{
			if (!fromOnResume) 
			{
				SessionService.IsFirstInstall = false;
				MainPage = new NavigationPage(PageService.Dashboard());
			}
		}
		else if (message == InternalMessage.LoggedOut)
		{
			var nav = new NavigationPage();
			nav.PushAsync(PageService.Login());
			MainPage = nav;
		}
		else if (message == InternalMessage.CheckAuth)
		{
			if (MainPage is NavigationPage nav && nav.CurrentPage is RegisterPage register) // they might have quit to go check email.
				return;

			if (SessionService.IsFirstInstall)
				MainPage = new NavigationPage(PageService.Login());
			else if (SessionService.IsAuthValid()) 
				CheckAppStateOn(InternalMessage.LoggedIn, fromOnResume);
			else 
				CheckAppStateOn(InternalMessage.LoggedOut, fromOnResume);
		}
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		Window window = base.CreateWindow(activationState);
		window.Created += WindowCreated;
		window.Stopped += WindowStopped;
		window.Resumed += WindowResumed;
		return window;
	}

	private void WindowCreated(object? sender, EventArgs e) { }
	private void WindowStopped(object? sender, EventArgs e) { }
	private void WindowResumed(object? sender, EventArgs e)
	{
		CheckAppStateOn(InternalMessage.CheckAuth, true);
	}
}
