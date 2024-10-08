﻿using Inventory.MobileApp.Controls;
using Inventory.MobileApp.Models;
using Inventory.MobileApp.Services;
using Inventory.MobileApp.ViewModels;
using Location = Inventory.MobileApp.Models.Location;

namespace Inventory.MobileApp.Pages;

public class LocationSearchPage : BasePage
{
    private readonly LocationSearchViewModel _ViewModel;
    private readonly SearchView<Location> _Search;
    private bool _IsEditing = false;

    public LocationSearchPage(LocationSearchViewModel locationSearchViewModel)
    {
        Title = LanguageService.Instance["Locations"];

        _ViewModel = locationSearchViewModel;
        _Search = new(locationSearchViewModel);
        _Search.CanAddItems = PermsUtils.IsAllowed(InventoryPermissions.CanAddLocation);
        _Search.SearchLayout = new LinearItemsLayout(ItemsLayoutOrientation.Vertical) { ItemSpacing = 12 };
        _Search.CardTemplate = new DataTemplate(() =>
        {
            var view = new LocationCardView();
            view.SetBinding(BindingContextProperty, ".");
            view.SetBinding(LocationCardView.DescriptionProperty, "Description");
            view.SetBinding(LocationCardView.BarcodeProperty, "Barcode");
            view.Clicked += DisplayLocationOptions;

            return view;
        });
        _Search.AddItem += AddLocation;

        Content = _Search;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (!_IsEditing)
        {
            _Search.TriggerRefresh();
        }
    }

    private async void DisplayLocationOptions(object? sender, EventArgs e)
    {
        if (sender is LocationCardView card && card.BindingContext is Location location)
        {
            bool canEdit = PermsUtils.IsAllowed(InventoryPermissions.CanEditLocation);

            string delete = LanguageService.Instance["Delete"];
            string print = LanguageService.Instance["Print"];
            string edit = LanguageService.Instance["Edit"];

            List<string> options = new List<string>();
            if (canEdit)
                options.Add(edit);

            options.Add(print);
            
            if (canEdit)
                options.Add(delete);

            string choice = await DisplayActionSheet(
                LanguageService.Instance["Options"],
                LanguageService.Instance["Cancel"],
                null,
                options.ToArray());

            if (choice == delete)
            {
                Delete(location);
            }
            else if (choice == edit)
            {
                EditLocation(location);
            }
            else if (choice == print)
            {
                string path = Path.Combine(FileSystem.CacheDirectory, "barcode.png");
                    File.WriteAllBytes(path, card.CurrentBarcode);
                    await Share.Default.RequestAsync(new ShareFileRequest
                    {
                        Title = LanguageService.Instance["Share Barcode"],
                        File = new ShareFile(path)
                    });
            }
        }
    }

    private async void Delete(Location location)
    {
        _Search.IsLoading = true;

        var response = await _ViewModel.DeleteLocation(location.Id);
        if (!string.IsNullOrEmpty(response.ErrorMessage))
        {
            _Search.IsLoading = false;
            this.DisplayCommonError(response.ErrorMessage);
            return;
        }

        switch(response.Data)
        {
            case DeleteResult.SuccesfullyDeleted:
                _Search.TriggerRefresh();
                break;
            case DeleteResult.LinkedToOtherItems:
                await DisplayAlert(
                    LanguageService.Instance["In Use"], 
                    $"{location.Description} - {LanguageService.Instance["is in use in at least one inventory item"]}.", 
                    LanguageService.Instance["OK"]);
                break;
        }
        
        _Search.IsLoading = false; // fail safe
    }

    private void AddLocation(object? sender, EventArgs e)
    {
        _IsEditing = true;
        Navigation.PushModalAsync(PageService.TakeUserInput(
            LanguageService.Instance["Enter the location description below."],
            "",
            Keyboard.Plain,
            submitted: async (location) =>
            {
                if (string.IsNullOrEmpty(location)) // canceled or entered empty text, don't do anything.
                    return;

                _Search.IsLoading = true;

                var response = await _ViewModel.InsertLocation(location);
                if (!string.IsNullOrEmpty(response.ErrorMessage))
                {
                    _Search.IsLoading = false;
                    this.DisplayCommonError(response.ErrorMessage);
                    return;
                }

                _Search.TriggerRefresh();
                _Search.IsLoading = false; // fail safe

                _IsEditing = false;
            },
            canceled: () =>
            {
                _IsEditing = false;
            }));
    }

    private void EditLocation(Location location)
    {
        _IsEditing = true;
        Navigation.PushModalAsync(PageService.TakeUserInput(
            LanguageService.Instance["Enter the location description below."],
            location.Description,
            Keyboard.Plain,
            submitted: async (loc) =>
            {
                if (string.IsNullOrEmpty(loc)) // canceled or entered empty text, don't do anything.
                    return;

                _Search.IsLoading = true;

                location.Description = loc;

                var response = await _ViewModel.UpdateLocation(location);
                if (!string.IsNullOrEmpty(response.ErrorMessage))
                {
                    _Search.IsLoading = false;
                    this.DisplayCommonError(response.ErrorMessage);
                    return;
                }

                _Search.TriggerRefresh();
                _Search.IsLoading = false; // fail safe

                _IsEditing = false;
            },
            canceled: () =>
            {
                _IsEditing = false;
            }));
    }
}
