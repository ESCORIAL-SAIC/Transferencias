using Android.Content;
using Android.Net.Wifi;
using System.Collections.ObjectModel;
using System.Net;
using Transferencias.Controllers;
using Transferencias.Models;

namespace Transferencias;

public static class Fun
{
    public static async Task<bool> CheckStockByDepoAsync(Guid? productId, Guid? depositOriginId, decimal requestedQuantity)
    {
        var itemInventario = new ItemInventario()
        {
            ProductoId = productId,
            DepositoId = depositOriginId
        };
        var stock = await ItemInventarioController.GetProductStockByDepoAsync(itemInventario);
        if (stock is null)
            return false;
        return stock.Cantidad2Cantidad >= requestedQuantity;
    }

    public static async Task<ObservableCollection<SolicitudTransferencia>> GetSolicitudesTransferenciaAsyncByStatus(int? idEstado)
    {
        var solicitudesTransferencia = await SolicitudTransferenciaController.GetByStatusIdAsync(idEstado) ??
            throw new InvalidOperationException($"No se encontraron solicitudes de transferencia con el estado ID {idEstado}.");
        foreach (var solicitud in solicitudesTransferencia)
        {
            solicitud.Estado = await EstadoController.GetByIdAsync(solicitud.EstadoId);
            solicitud.Usuario = await EscoUsuarioAppController.GetByIdAsync(solicitud.UsuarioId);
        }
        return solicitudesTransferencia;
    }
    public static async Task<ObservableCollection<Transferencia>> GetTransferenciasAsyncByStatus(int? idEstado)
    {
        var transferencias = await TransferenciaController.GetByStatusIdAsync(idEstado) ??
            throw new InvalidOperationException($"No se encontraron solicitudes transferencias con el estado ID {idEstado}.");
        foreach (var transferencia in transferencias)
        {
            transferencia.Estado = await EstadoController.GetByIdAsync(transferencia.EstadoId);
            transferencia.SolicitudTransferencia = await SolicitudTransferenciaController.GetByIdAsync(transferencia.SolicitudTransferenciaId);
            transferencia.SolicitudTransferencia!.Usuario = await EscoUsuarioAppController.GetByIdAsync(transferencia.SolicitudTransferencia.UsuarioId);
        }
        return transferencias;
    }

    public static async Task<bool> CheckWifiConnection(string expectedSsid)
    {



        var wifiManager = (WifiManager)Android.App.Application.Context.GetSystemService(Context.WifiService);
        var info = wifiManager.ConnectionInfo;

        var ssid = info?.SSID ?? "<unknown ssid>";

        if (ssid != expectedSsid)
            return false;

        return true;
    }

    public static async Task<bool> GetLocationPermissions()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
        return status == PermissionStatus.Granted;
    }

    public static async Task<bool> RequireLocationPermissions()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        }
        return status == PermissionStatus.Granted;
    }

    public static async Task<ObservableCollection<ItemTransferencia>> FilterPickedItemsAsync(SolicitudTransferencia solicitudTransferencia)
    {
        var itemsTransferencia = new ObservableCollection<ItemTransferencia>();
        foreach (var item in solicitudTransferencia.ItemsSolicitudTransferencia
            .Where(x => x.Pickeado is not null && x.Pickeado == true))
        {
            itemsTransferencia.Add(new ItemTransferencia()
            {
                ProductoId = item.ProductoId,
                Cantidad = item.Cantidad,
                UnidadMedidaId = item.UnidadMedidaId,
                EstadoId = Estado.Tipo.PendienteAprobacionProduccion
            });
            item.Pickeado = true;
            await ItemSolicitudTransferenciaController.UpdateAsync(item.Id, item);
        }
        return itemsTransferencia;
    }

    public static async Task<bool> CreateNewTransfer(Transferencia nuevaTransferencia)
    {
        var response = await TransferenciaController.NewAsync(nuevaTransferencia);
        if (response == HttpStatusCode.Created)
            return true;
        return false;
    }

    public static async Task<bool> UpdateOriginIfAllItemsPickedAsync(SolicitudTransferencia solicitudOriginal)
    {
        if (solicitudOriginal is null)
            return false;
        if (solicitudOriginal.ItemsSolicitudTransferencia is null)
            return false;
        if (!solicitudOriginal.ItemsSolicitudTransferencia.Any(i => i.Pickeado == false))
        {
            solicitudOriginal.EstadoId = Estado.Tipo.TransferenciaGenerada;
            await SolicitudTransferenciaController.UpdateStatusAsync(solicitudOriginal.Id, solicitudOriginal);
            return true;
        }
        return false;
    }
}