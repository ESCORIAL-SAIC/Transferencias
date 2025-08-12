using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using Transferencias.Controllers;
using Transferencias.Models;
using Transferencias.Resources.Values;

namespace Transferencias.Views;

[ObservableObject]
public partial class SettingsView
{
    [ObservableProperty]
    private ObservableCollection<Deposito>? _depositos = [];
    [ObservableProperty]
    private string _apiUrlLabel = SettingsViewStrings.ApiUrlLabel;
    [ObservableProperty]
    private string _apiUrlPlaceholderEntry = SettingsViewStrings.ApiUrlPlaceholderEntry;
    [ObservableProperty]
    private string _destinationDepoTitle = SettingsViewStrings.DestinationDepoTitle;
    [ObservableProperty]
    private string _loggedUserIdLabel = SettingsViewStrings.LoggedUserIdLabel;
    [ObservableProperty]
    private string _loggedUserIdPlaceholderEntry = SettingsViewStrings.LoggedUserIdPlaceholderEntry;
    [ObservableProperty]
    private string _originDepoTitle = SettingsViewStrings.OriginDepoTitle;
    [ObservableProperty]
    private string _saveButtonText = SettingsViewStrings.SaveButtonText;
    [ObservableProperty]
    private string _appStringVersionLabel = SettingsViewStrings.AppVersionLabel;

    public SettingsView()
    {
        InitializeComponent();
        BindingContext = this;
        Appearing += SettingsView_Appearing;
    }

    private async void SettingsView_Appearing(object? sender, EventArgs e) => await LoadSavedSettings();
    private async void SaveSettingsButton_OnClicked(object? sender, EventArgs e) => await SaveSettings();

    private async Task LoadSavedSettings()
    {
        try
        {
            Config.ShowLoadingPopup(this);
            var displayVersion = AppInfo.Current.VersionString;
            var buildVersion = AppInfo.Current.BuildString;
            AppStringVersionLabel = string.Format(AppStringVersionLabel, displayVersion, buildVersion);
            var apiUrl = await Config.GetApiUrlAsync() ?? string.Empty;
            var loggedUser = await Config.GetLoggedUser();
            var loggedUserId = loggedUser?.Id.ToString() ?? string.Empty;
            await LoadDepos(apiUrl);
            if (string.IsNullOrEmpty(loggedUserId))
                LoggedUserIdEntry.Placeholder = loggedUserId;
            else
                LoggedUserIdEntry.Text = loggedUserId;
        }
        catch (Exception ex)
        {
            await Message.Error(this, ex.Message);
        }
        finally
        {
            Config.CloseLoadingPopup();
        }
    }
    private async Task SaveSettings()
    {
        try
        {
            Config.ShowLoadingPopup(this);
            var apiUrl = ApiUrlEntry.Text;
            if (string.IsNullOrEmpty(apiUrl))
            {
                await DisplayAlert(AppStrings.AlertErrorTitle, AppStrings.AlertErrorBlankField,
                    AppStrings.AlertOkButton);
                return;
            }
            var isApiUrlSaved = await Config.SetApiUrl(apiUrl);
            var origen = (Deposito)DepositoOrigenPicker.SelectedItem;
            var destino = (Deposito)DepositoDestinoPicker.SelectedItem;
            if (origen is not null && destino is not null)
                await Config.SetDepos(origen, destino);
            if (isApiUrlSaved)
            {
                await DisplayAlert(AppStrings.AlertSuccessTitle, AppStrings.AlertSuccessConfigSavedMessage,
                    AppStrings.AlertOkButton);
                await Navigation.PopModalAsync();
                return;
            }
            await DisplayAlert(AppStrings.AlertErrorTitle, AppStrings.AlertErrorSavingSettings,
                AppStrings.AlertOkButton);
        }
        catch (Exception ex)
        {
            await Message.Error(this, ex.Message);
        }
        finally
        {
            Config.CloseLoadingPopup();
        }
    }


    private async Task LoadDepos(string apiUrl)
    {
        if (string.IsNullOrEmpty(apiUrl))
        {
            ApiUrlEntry.Placeholder = apiUrl;
            DepositoOrigenPicker.IsEnabled = false;
        }
        else
        {
            ApiUrlEntry.Text = apiUrl;
            DepositoOrigenPicker.IsEnabled = true;
            var depositosList = await DepositoController.GetAsync();
            if (depositosList is null)
            {
                await Message.Error(this, AppStrings.AlertErrorDepoLoad);
                return;
            }
            depositosList = depositosList.OrderBy(deposito => deposito.Nombre).ToList();
            Depositos = new ObservableCollection<Deposito>(depositosList);
            DepositoDestinoPicker.ItemsSource = Depositos;
            DepositoOrigenPicker.ItemsSource = Depositos;
            var origin = await Config.GetOrigin();
            var destination = await Config.GetDestination();
            if (destination is null || origin is null)
            {
                await Message.Error(this, AppStrings.AlertErrorDepoGet);
                return;
            }
            DepositoDestinoPicker.SelectedItem = Depositos.FirstOrDefault(d => d.Id == destination.Id);
            DepositoOrigenPicker.SelectedItem = Depositos.FirstOrDefault(d => d.Id == origin.Id);
        }
    }
}