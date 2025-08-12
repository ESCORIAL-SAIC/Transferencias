using System.Collections.ObjectModel;
using Transferencias.Controllers;
using Transferencias.Models;
using Transferencias.Resources.Values;

namespace Transferencias.Views.Almacen;

public partial class AlmacenTransferenciasView : ContentPage
{
    private ObservableCollection<Transferencia>? Transferencias = [];
    private ObservableCollection<Estado>? Estados = [];

    public AlmacenTransferenciasView()
    {
        InitializeComponent();
        Appearing += AlmacenTransferenciasView_Appearing;
    }

    private async void AlmacenTransferenciasView_Appearing(object? sender, EventArgs e) => await LoadData();
    private async void EstadosPicker_OnSelectedIndexChanged(object? sender, EventArgs e) => await FilterViewByStatus();
    private async void ItemsListView_ItemTapped(object sender, ItemTappedEventArgs e) => await OpenAuthView(e);
    private async void UpdateButton_Pressed(object sender, EventArgs e) => await LoadData();

    private async Task LoadData()
    {
        await ObtenerTransferencias(Estado.Tipo.PendienteAprobacionProduccion);
        await ObtenerEstados();
    }
    private async Task OpenAuthView(ItemTappedEventArgs tappedEventItem)
    {
        if (tappedEventItem.Item is not Transferencia tappedItem)
            return;
        if (tappedItem.EstadoId == Estado.Tipo.PendienteAprobacionProduccion)
            await Navigation.PushModalAsync(new AlmacenSolicitudAutorizacionView(tappedItem));
    }
    private async Task FilterViewByStatus()
    {
        if (Estados is null || EstadosPicker.SelectedIndex < 0 || EstadosPicker.SelectedIndex >= Estados.Count)
            return;
        var estadoSeleccionado = Estados[EstadosPicker.SelectedIndex];
        if (estadoSeleccionado?.Id is null)
            return;
        var idEstado = estadoSeleccionado.Id;
        await ObtenerTransferencias(idEstado);
    }

    private async Task ObtenerTransferencias(int? estadoId)
    {
        try
        {
            Config.ShowLoadingPopup(this);
            Transferencias = await Fun.GetTransferenciasAsyncByStatus(estadoId);
            ItemsListView.ItemsSource = Transferencias;
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
            Estados = await EstadoController.GetAsync();
            EstadosPicker.ItemsSource = Estados;
            EstadosPicker.SelectedItem = Estados!.FirstOrDefault(estado => estado.Id == Estado.Tipo.PendienteAprobacionProduccion);
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