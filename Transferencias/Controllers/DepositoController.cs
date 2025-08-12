using System.Net.Http.Json;
using Transferencias.Models;
using Transferencias.Resources.Values;

namespace Transferencias.Controllers
{
    public static class DepositoController
    {
        public static async Task<List<Deposito>?> GetAsync()
        {
            var endpoint = Endpoints.GetDeposito;
            var response =
                await Config.SharedClient!.GetFromJsonAsync<List<Deposito>>(endpoint);
            return response;
        }
        public static async Task<Deposito?> GetByIdAsync(Guid id)
        {
            var endpoint = string.Format(Endpoints.GetDepositoId, id);
            var response =
                await Config.SharedClient!.GetFromJsonAsync<Deposito>(endpoint);
            return response;
        }
    }
}
