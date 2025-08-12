using System.Collections.ObjectModel;
using System.Net.Http.Json;
using Transferencias.Models;
using Transferencias.Resources.Values;

namespace Transferencias.Controllers
{
    public static class EstadoController
    {
        public static async Task<ObservableCollection<Estado>?> GetAsync()
        {
            var endpoint = Endpoints.GetEscoTxEstado;
            var response =
                await Config.SharedClient!.GetFromJsonAsync<ObservableCollection<Estado>?>(endpoint);
            return response;
        }
        public static async Task<Estado?> GetByIdAsync(int id)
        {
            var endpoint = string.Format(Endpoints.GetEscoTxEstadoId, id.ToString());
            var response =
                await Config.SharedClient!.GetFromJsonAsync<Estado?>(endpoint);
            return response;
        }
    }
}
