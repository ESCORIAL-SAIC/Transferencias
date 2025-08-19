using Camera.MAUI.ZXingHelper;
using System.Collections.ObjectModel;
using System.Net;
using System.Threading.Tasks;
using Transferencias.Controllers;
using Transferencias.Models;
using Transferencias.Resources.Values;
using Transferencias.Services;
using Timer = System.Timers.Timer;

namespace Transferencias.Views.Produccion;

public partial class ProduccionCargaSolicitudView
{
    private readonly EtiquetaService _etiquetaService = new();
    private readonly ObservableCollection<Etiqueta> _etiquetasLeidas = [];
    private Timer _debounceTimer = new();
    private bool _isReading;
    public ProduccionCargaSolicitudView()
    {
        InitializeComponent();
        ItemsListView.ItemsSource = _etiquetasLeidas;
    }

    private async void CameraView_CamerasLoaded(object sender, EventArgs e) => await LoadCameras();
    private void CameraView_BarcodeDetected(object sender, BarcodeEventArgs args) => OnBarcodeDetected(args);
    private async void ConfirmarButton_OnPressed(object? sender, EventArgs e) => await NewRequest();
    private async void EliminarSwipeItem_OnInvoked(object? sender, EventArgs e) => await MakeItemInactive(sender);

    private async Task NewRequest()
    {
        try
        {
            Config.ShowLoadingPopup(this);
            if (!_etiquetasLeidas.Any())
                return;
            var items = new ObservableCollection<ItemSolicitudTransferencia>(
                _etiquetasLeidas.Select(
                    etiqueta => new ItemSolicitudTransferencia()
                    {
                        ProductoId = etiqueta.Producto!.Id,
                        Cantidad = etiqueta.Cantidad,
                        UnidadMedidaId = new Guid(AppStrings.UMUnId),
                        Pickeado = false,
                        EstadoId = Estado.Tipo.Activo
                    }).ToList()
                );
            var loggedUser = await Config.GetLoggedUser();
            if (loggedUser is null)
            {
                await DisplayAlert(AppStrings.AlertErrorTitle, AppStrings.AlertErrorLoggedUserNullMessage, AppStrings.AlertOkButton);
                return;
            }
            var solicitud = new SolicitudTransferencia()
            {
                EstadoId = Estado.Tipo.Activo,
                UsuarioId = loggedUser.Id,
                ItemsSolicitudTransferencia = items
            };

            var statusCode = await SolicitudTransferenciaController.NewAsync(solicitud);
            if (statusCode == HttpStatusCode.Created)
            {
                await DisplayAlert(AppStrings.AlertSuccessTitle,
                    AppStrings.AlertSuccessTransferenceMessage, AppStrings.AlertOkButton);
                await Navigation.PopModalAsync();
            }
            else
            {
                await DisplayAlert(AppStrings.AlertErrorTitle, statusCode.Value.ToString(), AppStrings.AlertOkButton);
            }
        }
        catch (Exception exception)
        {
            await DisplayAlert(AppStrings.AlertErrorTitle, exception.Message, AppStrings.AlertOkButton);
        }
        finally
        {
            Config.CloseLoadingPopup();
        }
    }
    private async Task MakeItemInactive(object? sender)
    {
        try
        {
            Config.ShowLoadingPopup(this);
            if (sender is not SwipeItem item)
                return;
            if (item.BindingContext is not Etiqueta etiqueta)
                return;
            _etiquetasLeidas.Remove(etiqueta);
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
    private async Task LoadCameras()
    {
        try
        {
            if (CameraView.Cameras.Count <= 0)
                return;
            CameraView.Camera = CameraView.Cameras[0];
            MainThread.BeginInvokeOnMainThread(Action);

            async void Action()
            {
                await CameraView.StopCameraAsync();
                await CameraView.StartCameraAsync();
            }
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
    private void OnBarcodeDetected(BarcodeEventArgs args)
    {
        if (_isReading)
            return;
        _isReading = true;
        MainThread.BeginInvokeOnMainThread(Action);
        _debounceTimer = new Timer(5000);
        _debounceTimer.Elapsed += (_, _) =>
        {
            _isReading = false;
        };
        _debounceTimer.AutoReset = false;
        _debounceTimer.Enabled = true;
        async void Action()
        {
            var etiqueta = await ReadBarcodeUIAsync(args.Result[0].Text);
            if (etiqueta != null)
                _etiquetasLeidas.Add(etiqueta);
        }
    }

    private async Task<Etiqueta?> ReadBarcodeUIAsync(string barcode)
    {
        Config.ShowLoadingPopup(this);
        try
        {
            var (etiqueta, error, stockBajo) = await _etiquetaService.ReadBarcodeAsync(barcode);

            if (!string.IsNullOrEmpty(error))
            {
                await DisplayAlert(AppStrings.AlertErrorTitle, error, AppStrings.AlertOkButton);
                return null;
            }

            if (stockBajo)
            {
                var continua = await Message.Confirmation(this,
                    "El stock contabilizado en el almacén de origen es menor al solicitado, lo cual podría demorar la transferencia. Desea continuar de todas formas?");
                if (!continua)
                    return null;
            }

            return etiqueta;
        }
        finally
        {
            Config.CloseLoadingPopup();
        }
    }

    private async void OnCameraTapped(object sender, TappedEventArgs e)
    {
        await SwitchToManualAsync();
    }

    private async void OnBackToCameraClicked(object sender, EventArgs e)
    {
        await SwitchToCameraAsync();
    }

    private async Task SwitchToManualAsync()
    {
        try { await CameraView.StopCameraAsync(); } catch { /* ignora si ya está detenida */ }
        CameraLayer.IsVisible = false;
        ManualLayer.IsVisible = true;
        Codigo.Focus();
    }

    private async Task SwitchToCameraAsync()
    {
        ManualLayer.IsVisible = false;
        CameraLayer.IsVisible = true;
        try { await CameraView.StartCameraAsync(); } catch { /* maneja si no hay permiso/cámara */ }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _ = CameraView.StartCameraAsync();
    }

    protected override void OnDisappearing()
    {
        _ = CameraView.StopCameraAsync();
        base.OnDisappearing();
    }

    private async void Codigo_Completed(object sender, EventArgs e)
    {
        if (sender is Entry entry && !string.IsNullOrWhiteSpace(entry.Text))
        {
            var barcode = entry.Text.Trim();
            entry.Text = string.Empty;

            var etiqueta = await ReadBarcodeUIAsync(barcode);
            if (etiqueta != null)
            {
                _etiquetasLeidas.Add(etiqueta);
            }
        }
    }
}