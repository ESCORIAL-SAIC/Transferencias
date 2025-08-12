using System.Net.Http.Json;
using Transferencias.Models;
using Transferencias.Resources.Values;

namespace Transferencias.Controllers;

public static class ItemInventarioController
{
    public static async Task<ItemInventario?> GetProductStockByDepoAsync(ItemInventario itemInventario)
    {
        if (Config.SharedClient == null)
            throw new InvalidOperationException("SharedClient no está configurado.");
        var endpoint = Endpoints.PostItemInventario;
        try
        {
            var response = await Config.SharedClient.PostAsJsonAsync(endpoint, itemInventario);
            if (!response.IsSuccessStatusCode)
            {
                await Console.Error.WriteLineAsync($"Error en la solicitud: {response.StatusCode} - {response.ReasonPhrase}");
                return null;
            }
            var itemRetrieved = await response.Content.ReadFromJsonAsync<ItemInventario>();
            return itemRetrieved;
        }
        catch (Exception ex)
        {
            await Console.Error.WriteLineAsync($"Excepción al realizar la solicitud: {ex.Message}");
            return null;
        }
    }

}