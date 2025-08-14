using CommunityToolkit.Maui.Views;
using QRCoder;
using Transferencias.Controllers;
using Transferencias.Models;
using Transferencias.Resources.Values;
using Transferencias.Services;
using Transferencias.Views.Assets;

namespace Transferencias;

public abstract class Config
{
    private static readonly DatabaseService DatabaseService = new();
    private static LoadingPopup? _loadingPopup;
    private static bool _isPopupVisible;
    public static HttpClient? SharedClient { get; private set; }

    public static async Task ConfigHttpClient()
    {
        var settings = await DatabaseService.GetAllAsync<ConfigVar>();
        var configVars = settings.ToList();
        var apiUrl = "http://transferencias.escorialsa.com.ar/api";
        SharedClient = new HttpClient
        {
            BaseAddress = new Uri(apiUrl!)
        };
    }

    public static async Task<bool> SetLoggedUser(string loggedUserId)
    {
        await DatabaseService.DeleteItemByKeyAsync<ConfigVar>(AppStrings.LoggedUserIdKey);
        var status = await DatabaseService.AddItemAsync(new ConfigVar { Key = AppStrings.LoggedUserIdKey, Value = loggedUserId });
        return status;
    }

    public static async Task<string?> GetApiUrlAsync()
    {
        // TODO: quitar hardcodeado
        //var settings = await DatabaseService.GetAllAsync<ConfigVar>();
        //var configVars = settings.ToList();
        //var apiUrlSetting = configVars.Find(l => l.Key == AppStrings.ApiUrlKey);
        //if (apiUrlSetting is null)
        //    return null;
        //var apiUrl = apiUrlSetting.Value;
        //return apiUrl ?? null;
        return "http://transferencias.escorialsa.com.ar/api"; // Temporal hardcodeado para pruebas
    }

    public static async Task<bool> SetApiUrl(string apiUrl)
    {
        var dataApiUrl = new ConfigVar()
        {
            Key = AppStrings.ApiUrlKey,
            Value = apiUrl
        };
        await DatabaseService.DeleteItemByKeyAsync<ConfigVar>(AppStrings.ApiUrlKey);
        var isApiUrlSaved = await DatabaseService.AddItemAsync(dataApiUrl);
        return isApiUrlSaved;
    }

    public static async Task<EscoUsuarioApp?> GetLoggedUser()
    {
        var settings = await DatabaseService.GetAllAsync<ConfigVar>();
        var configVars = settings.ToList();
        var loggedUserIdSetting = configVars.Find(l => l.Key == AppStrings.LoggedUserIdKey);
        var loggedUserIdString = loggedUserIdSetting?.Value;
        if (loggedUserIdString is null)
            return null;
        var loggedUserId = int.Parse(loggedUserIdString);
        var loggedUser = await EscoUsuarioAppController.GetByIdAsync(loggedUserId);
        return loggedUser;
    }

    public static void ShowLoadingPopup(Page currentPage)
    {
        if (_loadingPopup != null)
            return;
        _loadingPopup = new LoadingPopup();
        _isPopupVisible = true;
        currentPage.ShowPopup(_loadingPopup);
    }

    public static void CloseLoadingPopup()
    {
        if (_loadingPopup == null || !_isPopupVisible)
            return;
        _loadingPopup.Close();
        _loadingPopup = null;
        _isPopupVisible = false;
    }

    public static ImageSource GenerateQrCode(string text)
    {
        var qrGenerator = new QRCodeGenerator();
        var qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.L);
        var qRCode = new PngByteQRCode(qrCodeData);
        var qrCodeBytes = qRCode.GetGraphic(20);
        var qrImageSource = ImageSource.FromStream(() => new MemoryStream(qrCodeBytes));
        return qrImageSource;
    }

    public static async Task<bool> SetDepos(Deposito origen, Deposito destino)
    {
        var dataOrigen = new ConfigVar()
        {
            Key = AppStrings.OriginDepositIdKey,
            Value = origen.Id.ToString()
        };
        var dataDestino = new ConfigVar()
        {
            Key = AppStrings.DestinationDepositIdKey,
            Value = destino.Id.ToString()
        };
        await DatabaseService.DeleteItemByKeyAsync<ConfigVar>(AppStrings.OriginDepositIdKey);
        await DatabaseService.DeleteItemByKeyAsync<ConfigVar>(AppStrings.DestinationDepositIdKey);
        var isOrigenSaved = await DatabaseService.AddItemAsync(dataOrigen);
        var isDestinoSaved = await DatabaseService.AddItemAsync(dataDestino);
        return isOrigenSaved && isDestinoSaved;
    }

    public static async Task<Deposito?> GetOrigin()
    {
        //var settings = await DatabaseService.GetAllAsync<ConfigVar>();
        //var configVars = settings.ToList();
        //var originIdSetting = configVars.Find(o => o.Key == AppStrings.OriginDepositIdKey);
        //var originId = originIdSetting?.Value;
        //if (originId is null)
        //    return null;
        //var originGuid = new Guid(originId);
        //var origin = await DepositoController.GetByIdAsync(originGuid);
        //return origin ?? null;
        return new Deposito
        {
            Id = Guid.NewGuid(),
            Nombre = "Depósito de Origen",
            Codigo = "ORIGEN"
        }; // Temporal hardcodeado para pruebas
    }

    public static async Task<Deposito?> GetDestination()
    {
        //var settings = await DatabaseService.GetAllAsync<ConfigVar>();
        //var configVars = settings.ToList();
        //var destinationIdSetting = configVars.Find(o => o.Key == AppStrings.DestinationDepositIdKey);
        //var destinationId = destinationIdSetting?.Value;
        //if (destinationId is null)
        //    return null;
        //var destination = await DepositoController.GetByIdAsync(new Guid(destinationId));
        //return destination;
        return new Deposito
        {
            Id = Guid.NewGuid(),
            Nombre = "Depósito de Destino",
            Codigo = "DESTINO"
        }; // Temporal hardcodeado para pruebas
    }
}
