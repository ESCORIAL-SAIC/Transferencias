using CommunityToolkit.Mvvm.ComponentModel;
using Transferencias.Controllers;
using Transferencias.Models;
using Transferencias.Resources.Values;

namespace Transferencias.Views;

[ObservableObject]
public partial class LoginView
{
    [ObservableProperty]
    private string _loginEntryText = LoginViewStrings.LoginLabelText;
    [ObservableProperty]
    private string _placeholderUserEntryText = LoginViewStrings.UserEntryPlaceholder;
    [ObservableProperty]
    private string _placeholderPasswordEntryText = LoginViewStrings.PasswordEntryPlaceholder;
    [ObservableProperty]
    private string _loginButtonText = LoginViewStrings.LoginButtonText;
    [ObservableProperty]
    private string _appVersionText = LoginViewStrings.AppVersionText;
    [ObservableProperty]
    private string? _displayVersion;
    [ObservableProperty]
    private string? _buildVersion;


    private Sector sector;

    public LoginView()
    {
        InitializeComponent();
        BindingContext = this;
        Appearing += LoginView_Appearing;
    }

    private async void LoginView_Appearing(object? sender, EventArgs e) => await StartSettings();
    private async void SubmitButton_Clicked(object sender, EventArgs e) => await Submit();
    private async void SettingsButton_OnClicked(object? sender, EventArgs e) => await OpenSettings();

    private async Task StartSettings()
    {
        DisplayVersion = AppInfo.Current.VersionString;
        BuildVersion = AppInfo.Current.BuildString;
        await MainThread.InvokeOnMainThreadAsync(() =>
            AppVersionText = string.Format(AppVersionText, DisplayVersion, BuildVersion)
        );
        try
        {
            Config.ShowLoadingPopup(this);
            await Config.ConfigHttpClient();

            var loggedUser = await Config.GetLoggedUser();
            UserEntry.Text = loggedUser?.Codigo ?? string.Empty;

            // en desarrollo se omiten estas verificaciones
            // TODO: descomentar en producción

            //bool hasPermission = await Fun.GetLocationPermissions();
            //if (!hasPermission)
            //{
            //    await Message.Info(this, "A continuación se solicitará acceso a la ubicación para obtener el nombre de la red Wi-Fi. Este permiso es necesario para continuar y se solicitará una única vez.");
            //    hasPermission = await Fun.RequireLocationPermissions();
            //    if (!hasPermission)
            //        throw new PermissionException("Se necesitan permisos de ubicación para obtener el nombre de la red.");
            //}

            //var isWifiCorrect = await Fun.CheckWifiConnection("\"escorial-terminales\"");
            //if (!isWifiCorrect)
            //{
            //    await Message.Error(this, "Debe estar conectado a la red escorial-terminales para el correcto funcionamiento de la app.");
            //    Application.Current!.Quit();
            //}

            //var isCorrectVersion = await CheckAppVersionAsync(DisplayVersion);
            //if (!isCorrectVersion)
            //{
            //    await Message.Error(this, LoginViewStrings.OutdatedApp);
            //    Application.Current!.Quit();
            //}
        }
        catch (PermissionException pEx)
        {
            await Message.Error(this, pEx.Message);
        }
        catch (Exception ex)
        {
            await Message.Error(this, $"{AppStrings.AlertErrorInitialSettings}\n{ex.Message}\n{ex.InnerException}");
        }
        finally
        {
            Config.CloseLoadingPopup();
        }
    }

    private async Task Submit()
    {
        if (string.IsNullOrEmpty(UserEntry.Text))
        {
            await Message.Error(this, AppStrings.AlertErrorLoginBlankUser);
            return;
        }
        if (string.IsNullOrEmpty(PasswordEntry.Text))
        {
            await Message.Error(this, AppStrings.AlertErrorLoginBlankPassword);
            return;
        }
        if (string.IsNullOrEmpty(await Config.GetApiUrlAsync()))
        {
            await Message.Error(this, AppStrings.AlertErrorApiUrlConfig);
            return;
        }
        if (await Config.GetOrigin() is null || await Config.GetDestination() is null)
        {
            await Message.Error(this, AppStrings.AlertErrorDepoConfig);
            return;
        }
        try
        {
            Config.ShowLoadingPopup(this);
            var requestedUser = new EscoUsuarioApp()
            {
                Codigo = UserEntry.Text,
                Contrasena = PasswordEntry.Text
            };
            var isLoggedIn = await EscoUsuarioAppController.IsLoggedAsync(requestedUser);
            if (!isLoggedIn)
            {
                await Message.Error(this, AppStrings.AlertErrorLoginInvalidPassword);
                return;
            }
            var user = await EscoUsuarioAppController.GetByUserCodeAsync(UserEntry.Text);
            if (user is null)
            {
                await Message.Error(this, "Usuario inválido. No se encontró en la base de datos.");
                return;
            }
            user.Sector = await SectorController.GetByIdAsync(user.SectorId);
            if (user.Sector is null)
            {
                await Message.Error(this, "Sector inválido. No se encontró en la base de datos.");
                return;
            }
            await DisplayAlert(AppStrings.AlertSuccessTitle, string.Format(AppStrings.AlertSuccessLoginMessage, user.Codigo, user.NombreCompleto), AppStrings.AlertOkButton);
            await Config.SetLoggedUser(user.Id.ToString());
            if (user.Sector.Codigo.Equals(AppStrings.SectorCodAlm25M) && Shell.Current != null)
            {
                await Shell.Current.GoToAsync(AppStrings.ViewAdrAlm25M);
                return;
            }
            if (user.Sector.Codigo.Equals(AppStrings.SectorCodPrd25M) && Shell.Current != null)
            {
                await Shell.Current.GoToAsync(AppStrings.ViewAdrPrd25M);
                return;
            }
            await Message.Error(this, AppStrings.AlertErrorLoginSector);
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

    private async Task OpenSettings()
    {
        try
        {
            var password = await DisplayPromptAsync(AppStrings.AlertRestrictedAccessTitle, AppStrings.AlertRestrictedAccessPrompt);
            if (string.IsNullOrEmpty(password))
                return;
            if (password.Equals(AppStrings.ConfigurationViewPassword))
            {
                await Navigation.PushModalAsync(new SettingsView());
                return;
            }
            await Message.Error(this, AppStrings.AlertErrorLoginInvalidPassword);
        }
        catch (Exception ex)
        {
            await Message.Error(this, ex.Message);
        }
    }

    private static async Task<bool> CheckAppVersionAsync(string currentVersion)
    {
        var expectedVersion = await EscoTxConfigController.GetAppVersionAsync();
        if (expectedVersion is null)
            return false;
        return currentVersion == expectedVersion.Valor;
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        var appInfo = string.Format(LoginViewStrings.AppInfoText, DisplayVersion, BuildVersion, DateTimeOffset.FromUnixTimeSeconds(long.Parse(BuildVersion)));
        await Message.Info(this, appInfo);
    }
}