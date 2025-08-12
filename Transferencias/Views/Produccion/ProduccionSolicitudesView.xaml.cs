using System.Collections.ObjectModel;
using System.Net;
using System.Text;
using Transferencias.Controllers;
using Transferencias.Models;

namespace Transferencias.Views.Produccion;

public partial class ProduccionSolicitudesView
{
    private ObservableCollection<SolicitudTransferencia> _solicitudesTransferencias = [];
    private ObservableCollection<Estado>? _estados = [];

    public ProduccionSolicitudesView()
    {
        InitializeComponent();
        Appearing += ProduccionSolicitudesView_Appearing;
    }

    private async void ProduccionSolicitudesView_Appearing(object? sender, EventArgs e) => await LoadData();
    private async void EliminarSwipeItem_OnInvoked(object? sender, EventArgs e) => await MakeItemInactive(sender);
    private async void EstadosPicker_OnSelectedIndexChanged(object? sender, EventArgs e) => await FilterViewBySelectedStatus();
    private async void ItemsListView_ItemTapped(object sender, ItemTappedEventArgs e) => await ViewItemsDetail(e);
    private async void UpdateButton_Pressed(object sender, EventArgs e) => await LoadData();
    private async void NewButton_Pressed(object sender, EventArgs e) => await OpenNewRequest();


    private async Task MakeItemInactive(object? itemEvent)
    {
        try
        {
            Config.ShowLoadingPopup(this);
            if (itemEvent is not SwipeItem item)
                return;
            if (item.BindingContext is not SolicitudTransferencia solicitud)
                return;
            if (solicitud.EstadoId != Estado.Tipo.Activo)
            {
                await Message.Error(this, "No se puede anular una solicitud que no se encuentre activa");
                return;
            }
            var anula = await Message.Confirmation(this, $"¿Desea anular la solicitud de insumos {solicitud.Id}?");
            if (!anula)
                return;
            solicitud.EstadoId = Estado.Tipo.Inactivo;
            var response = await SolicitudTransferenciaController.UpdateStatusAsync(solicitud.Id, solicitud);
            if (response == HttpStatusCode.NoContent)
                _solicitudesTransferencias!.Remove(solicitud);
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
    private async Task FilterViewBySelectedStatus()
    {
        try
        {
            Config.ShowLoadingPopup(this);
            if (_estados is null || EstadosPicker.SelectedIndex < 0 || EstadosPicker.SelectedIndex >= _estados.Count)
                return;
            var estadoSeleccionado = _estados[EstadosPicker.SelectedIndex];
            if (estadoSeleccionado.Id is null)
                return;
            var idEstado = estadoSeleccionado.Id;
            await ObtenerSolicitudes(idEstado);
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
    private async Task ViewItemsDetail(ItemTappedEventArgs itemTappedEvent)
    {
        try
        {
            Config.ShowLoadingPopup(this);
            if (itemTappedEvent.Item is not SolicitudTransferencia tappedItem)
                return;
            var detailTextBuilder = new StringBuilder();
            foreach (var item in tappedItem.ItemsSolicitudTransferencia!)
            {
                item.Producto = await ProductoController.GetByIdAsync(item.ProductoId);
                var pickeado = (bool)item.Pickeado! ? "si" : "no";
                detailTextBuilder.AppendLine($"Producto: {item.Producto!.Descripcion} - Cantidad: {item.Cantidad} - Pickeado: {pickeado}");
            }
            await Message.Info(this, detailTextBuilder.ToString());
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
    private async Task LoadData()
    {
        await ObtenerSolicitudes(Estado.Tipo.Activo);
        await ObtenerEstados();
    }
    private async Task ObtenerSolicitudes(int? idEstado)
    {
        try
        {
            Config.ShowLoadingPopup(this);
            _solicitudesTransferencias = await Fun.GetSolicitudesTransferenciaAsyncByStatus(idEstado);
            ItemsListView.ItemsSource = _solicitudesTransferencias;
        }
        catch (InvalidOperationException ioeEx)
        {
            await Message.Error(this, $"Error:\n{ioeEx.Message}");
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
    private async Task ObtenerEstados()
    {
        try
        {
            Config.ShowLoadingPopup(this);
            _estados = await EstadoController.GetAsync();
            EstadosPicker.ItemsSource = _estados;
            EstadosPicker.SelectedItem = _estados!.FirstOrDefault(estado => estado.Id == Estado.Tipo.Activo);
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
    private async Task OpenNewRequest()
    {
        await Navigation.PushModalAsync(new ProduccionCargaSolicitudView());
    }
}
