using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http.Json;
using Transferencias.Models;
using Transferencias.Resources.Values;

namespace Transferencias.Controllers
{
    public static class SolicitudTransferenciaController
    {
        public static async Task<HttpStatusCode?> NewAsync(SolicitudTransferencia? solicitud)
        {
            var endpoint = Endpoints.PostEscoTxSolicitudTransferencia;
            var response = await Config.SharedClient!.PostAsJsonAsync(endpoint, solicitud);
            return response.StatusCode;
        }
        public static async Task<ObservableCollection<SolicitudTransferencia>?> GetAsync()
        {
            var endpoint = Endpoints.GetEscoTxSolicitudTransferencia;
            var response =
                await Config.SharedClient!.GetFromJsonAsync<ObservableCollection<SolicitudTransferencia>>(endpoint);
            return response;
        }
        public static async Task<ObservableCollection<SolicitudTransferencia>?> GetByStatusIdAsync(int? idEstado)
        {
            var endpoint = string.Format(Endpoints.GetEscoTxSolicitudTransferenciaEstado, idEstado);
            var response =
                await Config.SharedClient!.GetFromJsonAsync<ObservableCollection<SolicitudTransferencia>>(endpoint);
            return response;
        }
        public static async Task<HttpStatusCode> UpdateStatusAsync(int solicitudId, SolicitudTransferencia solicitud)
        {
            var endpoint = string.Format(Endpoints.PutEscoTxSolicitudTransferenciaId, solicitudId);
            var response =
                await Config.SharedClient!.PutAsJsonAsync(endpoint, solicitud);
            return response.StatusCode;
        }
        public static async Task<SolicitudTransferencia?> GetByIdAsync(int id)
        {
            var endpoint = string.Format(Endpoints.GetEscoTxSolicitudTransferenciaId, id);
            var response =
                await Config.SharedClient!.GetFromJsonAsync<SolicitudTransferencia>(endpoint);
            return response;
        }
    }
}
