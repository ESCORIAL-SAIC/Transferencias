using System.Net.Http.Json;
using Transferencias.Models;
using Transferencias.Resources.Values;

namespace Transferencias.Controllers
{
    public static class EscoUsuarioAppController
    {
        public static async Task<EscoUsuarioApp?> GetByUserCodeAsync(string userCode)
        {
            var endpoint = string.Format(Endpoints.GetEscoUsuarioAppCodigo, userCode);
            var response =
                await Config.SharedClient!.GetFromJsonAsync<EscoUsuarioApp>(endpoint);
            return response;
        }
        public static async Task<EscoUsuarioApp?> GetByIdAsync(int id)
        {
            var endpoint = string.Format(Endpoints.GetEscoUsuarioAppId, id.ToString());
            var response =
                await Config.SharedClient!.GetFromJsonAsync<EscoUsuarioApp>(endpoint);
            return response;
        }
        public static async Task<bool> IsLoggedAsync(EscoUsuarioApp? user)
        {
            var endpoint = Endpoints.PostEscoUsuarioAppLogin;
            var response =
                await Config.SharedClient!.PostAsJsonAsync(endpoint, user);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return true;
            return false;
        }
    }
}
