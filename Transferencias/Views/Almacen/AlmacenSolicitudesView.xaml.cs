using System.Collections.ObjectModel;
using Transferencias.Controllers;
using Transferencias.Models;
using Transferencias.Resources.Values;

namespace Transferencias.Views.Almacen;

public partial class AlmacenSolicitudesView
{
    private ObservableCollection<SolicitudTransferencia>? _solicitudesTransferencias = [];
    private ObservableCollection<Estado>? _estados = [];
    public AlmacenSolicitudesView()
    {
        InitializeComponent();
        Appearing += AlmacenSolicitudesView_Appearing;
    }

    private async void AlmacenSolicitudesView_Appearing(object? sender, EventArgs e) => await LoadData();
    private async void EstadosPicker_OnSelectedIndexChanged(object? sender, EventArgs e) => await FilterViewByStatus();
    private async void ItemsListView_ItemTapped(object sender, ItemTappedEventArgs e) => await OpenNewTransferView(e);
    private async void UpdateButton_Pressed(object sender, EventArgs e) => await LoadData();

    private async Task FilterViewByStatus()
    {
        if (_estados is null || EstadosPicker.SelectedIndex < 0 || EstadosPicker.SelectedIndex >= _estados.Count)
            return;
        var estadoSeleccionado = _estados[EstadosPicker.SelectedIndex];
        if (estadoSeleccionado?.Id is null)
            return;
        var idEstado = estadoSeleccionado.Id;
        await ObtenerSolicitudes(idEstado);
    }
    private async Task OpenNewTransferView(ItemTappedEventArgs itemTappedEvent)
    {
        if (itemTappedEvent.Item is not SolicitudTransferencia tappedItem)
            return;
        if (tappedItem.EstadoId != Estado.Tipo.Activo)
        {
            await Message.Error(this, "No se puede procesar una solicitud inactiva");
            return;
        }
        await Navigation.PushModalAsync(new AlmacenNuevaTransferenciaView(tappedItem));
    }
    private async Task LoadData()
    {
        await ObtenerSolicitudes(Estado.Tipo.Activo);
        await ObtenerEstados();
    }
    private async Task ObtenerSolicitudes(int? estadoId)
    {
        try
        {
            Config.ShowLoadingPopup(this);
            _solicitudesTransferencias = await Fun.GetSolicitudesTransferenciaAsyncByStatus(estadoId);
            ItemsListView.ItemsSource = _solicitudesTransferencias;
        }
        catch (InvalidOperationException ioeEx)
        {
            await Message.Error(this, $"Error:\n{ioeEx.Message}");
        }
        catch (Exception ex)
        {
            await Message.Error(this, $"Error inesperado:\n{ex.Message}");
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
            await DisplayAlert(AppStrings.AlertErrorTitle, ex.Message, AppStrings.AlertOkButton);
        }
        finally
        {
            Config.CloseLoadingPopup();
        }
    }
}