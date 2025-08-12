using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http.Json;
using Transferencias.Models;
using Transferencias.Resources.Values;

namespace Transferencias.Controllers
{
    public static class TransferenciaController
    {
        public static async Task<HttpStatusCode?> NewAsync(Transferencia? datos)
        {
            var endpoint = Endpoints.PostEscoTxTransferencia;
            var response = await Config.SharedClient!.PostAsJsonAsync(endpoint, datos);
            return response.StatusCode;
        }
        public static async Task<ObservableCollection<Transferencia>?> GetAsync()
        {
            var endpoint = Endpoints.GetEscoTxTransferencia;
            var response =
                await Config.SharedClient!.GetFromJsonAsync<ObservableCollection<Transferencia>>(endpoint);
            return response;
        }
        public static async Task<ObservableCollection<Transferencia>?> GetByStatusIdAsync(int? idEstado)
        {
            var endpoint = string.Format(Endpoints.GetEscoTxTransferenciaEstado, idEstado);
            var response =
                await Config.SharedClient!.GetFromJsonAsync<ObservableCollection<Transferencia>>(endpoint);
            return response;
        }
        public static async Task<HttpStatusCode> UpdateStatusAsync(int solicitudId, Transferencia solicitud)
        {
            var endpoint = string.Format(Endpoints.PutEscoTxTransferenciaId, solicitud.Id);
            var response =
                await Config.SharedClient!.PutAsJsonAsync(endpoint, solicitud);
            return response.StatusCode;
        }
        public static async Task<Transferencia?> GetByIdAsync(string id)
        {
            var endpoint = string.Format(Endpoints.GetEscoTxTransferenciaId, id);
            var response =
                await Config.SharedClient!.GetFromJsonAsync<Transferencia>(endpoint);
            return response;
        }
    }
}
