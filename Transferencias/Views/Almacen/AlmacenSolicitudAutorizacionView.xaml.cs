using Transferencias.Controllers;
using Transferencias.Models;

namespace Transferencias.Views.Almacen;

public partial class AlmacenSolicitudAutorizacionView : ContentPage
{
    private Transferencia? Transferencia { get; set; }

    public AlmacenSolicitudAutorizacionView(Transferencia transferencia)
    {
        InitializeComponent();
        Transferencia = transferencia;
        Appearing += AlmacenSolicitudAutorizacionView_Appearing;
    }

    private async void AlmacenSolicitudAutorizacionView_Appearing(object? sender, EventArgs e) => await LoadData();

    private async Task LoadData()
    {
        if (Transferencia is null)
            return;
        QrCodeImage.Source = Config.GenerateQrCode(Transferencia.Id.ToString());
        await CargarTransferencia(Transferencia);
        await WaitApprovalAsync(Transferencia);
    }
    private async Task CargarTransferencia(Transferencia transferencia)
    {
        try
        {
            Config.ShowLoadingPopup(this);
            if (transferencia.Id.ToString() is null)
                return;
            foreach (var item in Transferencia!.ItemsTransferencia!)
                item.Producto = await ProductoController.GetByIdAsync(item.ProductoId);
            MainThread.BeginInvokeOnMainThread(() =>
            {
                TransferenciaNombreLabel.Text = $"{Transferencia.NombreTransferencia} - {Transferencia.SolicitudTransferencia.Usuario.NombreCompleto}";
                ItemsListView.ItemsSource = Transferencia.ItemsTransferencia
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
    private async Task WaitApprovalAsync(Transferencia transferencia)
    {
        if (transferencia is null)
            return;
        while (transferencia.EstadoId != Estado.Tipo.Aprobado)
            transferencia = await TransferenciaController.GetByIdAsync(Transferencia.Id.ToString());
        await Message.Info(this, "Se aprobó correctamente la transferencia.");
        await Navigation.PopModalAsync();
    }
}