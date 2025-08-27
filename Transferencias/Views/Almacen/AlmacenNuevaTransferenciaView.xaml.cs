using Camera.MAUI.ZXingHelper;
using System.Collections.ObjectModel;
using Transferencias.Controllers;
using Transferencias.Models;
using Transferencias.Resources.Values;
using Transferencias.Services;
using Timer = System.Timers.Timer;

namespace Transferencias.Views.Almacen;

public partial class AlmacenNuevaTransferenciaView
{
    private readonly EtiquetaService _etiquetaService = new();
    private SolicitudTransferencia? SolicitudTransferencia { get; set; }
    private Timer _debounceTimer = new();
    private bool _isReading;
    public bool IsRefreshing { get; set; }

    public AlmacenNuevaTransferenciaView(SolicitudTransferencia solicitudTransferencia)
    {
        InitializeComponent();
        var itemsFiltrados =
            new ObservableCollection<ItemSolicitudTransferencia>(solicitudTransferencia.ItemsSolicitudTransferencia!.Where(i => i.Pickeado == false));
        solicitudTransferencia.ItemsSolicitudTransferencia = itemsFiltrados;
        SolicitudTransferencia = solicitudTransferencia;
        Appearing += AlmacenNuevaTransferenciaView_Appearing;
    }

    private async void AlmacenNuevaTransferenciaView_Appearing(object? sender, EventArgs e) => await LoadData();
    private async void CameraView_CamerasLoaded(object sender, EventArgs e) => await LoadCameras();
    private void CameraView_BarcodeDetected(object sender, BarcodeEventArgs args) => OnBarcodeDetected(args);
    private async void ConfirmarButton_OnPressed(object? sender, EventArgs e) => await NewTransfer();

    private async Task LoadData()
    {
        if (SolicitudTransferencia is null)
            return;
        await CargarSolicitud();
    }
    private async Task NewTransfer()
    {
        try
        {
            Config.ShowLoadingPopup(this);
            if (SolicitudTransferencia?.ItemsSolicitudTransferencia is null)
                return;
            if (!SolicitudTransferencia.ItemsSolicitudTransferencia.Any(x => x.Pickeado == true))
            {
                await DisplayAlert(AppStrings.AlertErrorTitle, "No se encontraron items pickeados para a�adir a la transaccion", AppStrings.AlertOkButton);
                return;
            }

            var itemsTransferencia = await Fun.FilterPickedItemsAsync(SolicitudTransferencia);

            var loggedUser = await Config.GetLoggedUser();
            if (loggedUser is null)
            {
                await DisplayAlert(AppStrings.AlertErrorTitle, AppStrings.AlertErrorLoggedUserNullMessage, AppStrings.AlertOkButton);
                return;
            }

            var origin = await Config.GetOrigin();
            var destination = await Config.GetDestination();
            if (origin is null || destination is null)
            {
                await Message.Error(this, "No se encontraron los dep�sitos de origen o destino");
                return;
            }

            var nuevaTransferencia = new Transferencia()
            {
                SolicitudTransferenciaId = SolicitudTransferencia.Id,
                EstadoId = Estado.Tipo.PendienteAprobacionProduccion,
                DepositoOriId = origin.Id,
                DepositoDesId = destination.Id,
                UsuarioId = loggedUser.Id,
                ItemsTransferencia = itemsTransferencia
            };

            var solicitudOriginal = await SolicitudTransferenciaController.GetByIdAsync(SolicitudTransferencia.Id);
            var isCreated = await Fun.CreateNewTransfer(nuevaTransferencia);
            if (isCreated)
            {
                await Fun.UpdateOriginIfAllItemsPickedAsync(solicitudOriginal!);
                await DisplayAlert(AppStrings.AlertSuccessTitle, "Transferencia cargada correctamente.\nQueda pendiente de aceptacion", AppStrings.AlertOkButton);
                await Navigation.PopModalAsync();
                return;
            }
            await DisplayAlert(AppStrings.AlertErrorTitle, "No se carg� la transferencia", AppStrings.AlertOkButton);

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
    private async Task CargarSolicitud()
    {
        try
        {
            Config.ShowLoadingPopup(this);
            if (SolicitudTransferencia is null)
                return;
            if (SolicitudTransferencia.ItemsSolicitudTransferencia is null)
                return;
            foreach (var item in SolicitudTransferencia.ItemsSolicitudTransferencia)
                item.Producto = await ProductoController.GetByIdAsync(item.ProductoId);
            MainThread.BeginInvokeOnMainThread(() =>
            {
                NumeroSolicitudLabel.Text = $"{SolicitudTransferencia.NombreSolicitud} - {SolicitudTransferencia.Usuario!.NombreCompleto}";
                ItemsListView.ItemsSource = SolicitudTransferencia.ItemsSolicitudTransferencia
                    .Where(x => x.Pickeado == false)
                    .ToList();
            });
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
            try
            {
                Config.ShowLoadingPopup(this);
                await ProcessBarcode(args.Result[0].Text);
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
    }
    private async Task ProcessBarcode(string barcodeText)
    {
        try
        {
            var etiqueta = await EtiquetaController.GetByCodeAsync(barcodeText);
            if (etiqueta?.Id is null)
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await DisplayAlert(AppStrings.AlertErrorTitle, "Etiqueta no encontrada", AppStrings.AlertOkButton);
                });
                return;
            }
            etiqueta.Producto = await ProductoController.GetByIdAsync(etiqueta.ProductoId);
            if (etiqueta.Producto is null)
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await DisplayAlert(AppStrings.AlertErrorTitle, "Producto no encontrado en la etiqueta", AppStrings.AlertOkButton);
                });
                return;
            }
            if (SolicitudTransferencia?.ItemsSolicitudTransferencia is null)
                return;
            var item = SolicitudTransferencia.ItemsSolicitudTransferencia
                .FirstOrDefault(x =>
                    x is { Producto.Codigo: not null } &&
                    x.Producto.Codigo.Equals(etiqueta.Producto.Codigo) &&
                    x.Cantidad.Equals(etiqueta.Cantidad) &&
                    x.Pickeado == false
                );
            if (item is null)
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await DisplayAlert(AppStrings.AlertErrorTitle, "Item no encontrado en la solicitud de transferencia", AppStrings.AlertOkButton);
                });
                return;
            }
            var origin = await Config.GetOrigin();
            var hasStock = await Fun.CheckStockByDepoAsync(etiqueta.Producto!.Id, origin!.Id, etiqueta.Cantidad);
            if (!hasStock)
            {
                await Message.Error(this, "No hay stock disponible del producto pickeado.");
                return;
            }
            SolicitudTransferencia.ItemsSolicitudTransferencia
                .FirstOrDefault(x => x.Id.ToString() is not null && x.Id.Equals(item.Id))!.Pickeado = true;
        }
        catch (Exception ex)
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await DisplayAlert(AppStrings.AlertErrorTitle, $"Error procesando el c�digo de barras: {ex.Message}", AppStrings.AlertOkButton);
            });
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
        try { await CameraView.StopCameraAsync(); } catch { /* ignora si ya est� detenida */ }
        CameraLayer.IsVisible = false;
        ManualLayer.IsVisible = true;
        BarcodeEntry.Text = string.Empty;
        BarcodeEntry.Focus();
    }

    private async Task SwitchToCameraAsync()
    {
        ManualLayer.IsVisible = false;
        CameraLayer.IsVisible = true;
        try { await LoadCameras(); } catch { /* maneja si no hay permiso/c�mara */ }
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
            try
            {
                Config.ShowLoadingPopup(this);
                await ProcessBarcode(entry.Text.Trim());
            }
            catch (Exception ex)
            {
                await Message.Error(this, ex.Message);
            }
            finally
            {
                entry.Text = string.Empty;
                entry.Focus();
                Config.CloseLoadingPopup();
            }
        }
    }
}