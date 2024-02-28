using Maui.Components;
using Maui.Components.Controls;
using Maui.Components.Pages;
using Maui.Inventory.Models;
using Maui.Inventory.ViewModels;

namespace Maui.Inventory.Pages;

public class InventoryPage : BasePage
{
	#region Private Properties
	private InventoryViewModel _viewModel => (InventoryViewModel) BindingContext;
	private readonly ILanguageService _languageService;
	private readonly MaterialList<Models.Inventory> _Search;
    #endregion

    #region Constructor
    public InventoryPage(
		ILanguageService languageService,
		InventoryViewModel viewModel) : base(languageService)
	{
		_languageService = languageService;
		BindingContext = viewModel;

		Title = _languageService.StringForKey("Inventory");

		_Search = new(_languageService.StringForKey("No Inventory."), MaterialIcon.Inventory_2, new DataTemplate(() =>
		{
			var view = new MaterialArticleCardView();
			view.SetBinding(MaterialArticleCardView.BindingContextProperty, ".");
			view.SetBinding(MaterialArticleCardView.ArticleProperty, "Description");
            view.SetBinding(MaterialArticleCardView.MainSupportOneProperty, "Location");
            view.SetBinding(MaterialArticleCardView.MainSupportTwoProperty, "QuantityStr");
            view.SetBinding(MaterialArticleCardView.TitleProperty, "Status");
            view.SetDynamicResource(MaterialCardView.BackgroundColorProperty, "CardColor");
            view.TextColor = Colors.White;

            view.Clicked += EditInventory;

			return view;
		}), viewModel, isEditable: false);

		Content = _Search;

        _Search.AddItemClicked += AddInventory;
	}
	~InventoryPage()
	{
		_Search.AddItemClicked -= AddInventory;	
	}
    #endregion

    #region Overrides
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _Search.FetchPublic();
        GetPermissions();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
    }
    #endregion

    #region Helpers
    private async void GetPermissions()
    {
        AccessControl.EditInventoryPermissions = await _viewModel.GetPermissions();
        int canAddPermission = AccessControl.EditInventoryPermissions & (int)EditInventoryPerms.CanAddInventory;
        if (canAddPermission == (int)EditInventoryPerms.CanAddInventory)
        {
            _Search.ToggleEditable(AccessControl.IsLicenseValid);
        }
        else
        {
            _Search.ToggleEditable(false);
        }
    }

    private void AddInventory(object sender, ClickedEventArgs e)
    {
        _viewModel.SelectedInventory = null;
        _viewModel.SelectedCached = null;
        _viewModel.Clean();
        Navigation.PushAsync(new EditInventoryPage(_languageService, _viewModel));
    }

    private void EditInventory(object sender, EventArgs e)
    {
        if (sender is MaterialArticleCardView card && card.BindingContext is Models.Inventory inventory)
        {
            _viewModel.SelectedInventory = inventory;
            _viewModel.SelectedCached = new()
            {
                Id = inventory.Id,
                AdminId = inventory.AdminId,
                Description = inventory.Description,
                Status = inventory.Status,
                Quantity = inventory.Quantity,
                QuantityType = inventory.QuantityType,
                Barcode = inventory.Barcode,
                Location = inventory.Location,
                LastEditedOn = inventory.LastEditedOn,
                CreatedOn = inventory.CreatedOn,
            };
            _viewModel.Clean();
            Navigation.PushAsync(new EditInventoryPage(_languageService, _viewModel));
        }
    }
    #endregion
}