using Camera.MAUI.ZXingHelper;
using Transferencias.Controllers;
using Timer = System.Timers.Timer;

namespace Transferencias.Views.Produccion;

public partial class ProduccionAutorizarTransferenciaView
{
    private Timer _debounceTimer = new();
    private bool _isReading;
    public ProduccionAutorizarTransferenciaView()
    {
        InitializeComponent();
        Appearing += ProduccionAutorizarTransferenciaView_Appearing;
    }

    private async void ProduccionAutorizarTransferenciaView_Appearing(object? sender, EventArgs e) => await LoadCameras();

    private async void CameraView_CamerasLoaded(object sender, EventArgs e) => await LoadCameras();
    private void CameraView_BarcodeDetected(object sender, BarcodeEventArgs args) => OnBarcodeDetected(args);

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
            try
            {
                Config.ShowLoadingPopup(this);
                var idTransferencia = args.Result[0].Text;
                BarcodeResult.Text = $"Transferencia Nro.: {args.Result[0].Text}";
                var aprueba = await Message.Confirmation(this, $"Confirma la aprobacion de la transferencia {BarcodeResult.Text}");
                if (!aprueba)
                {
                    await Message.Info(this, "No se confirmó la aprobación de la transacción");
                    return;
                }
                var transferencia = await TransferenciaController.GetByIdAsync(idTransferencia);
                if (transferencia is null)
                {
                    await Message.Error(this, "No se encontró la transferencia para aprobar");
                    return;
                }
                transferencia.EstadoId = Models.Estado.Tipo.Aprobado;
                var response = await TransferenciaController.UpdateStatusAsync(transferencia.Id, transferencia);
                if (response == System.Net.HttpStatusCode.NoContent)
                {
                    await Message.Info(this, "Transferencia aprobada correctamente");
                    return;
                }
                await Message.Error(this, response.ToString());
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
}