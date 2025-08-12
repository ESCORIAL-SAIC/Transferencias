using System.Net.Http.Json;
using Transferencias.Models;
using Transferencias.Resources.Values;

namespace Transferencias.Controllers
{
    public class ProductoController
    {
        public static async Task<Producto?> GetByIdAsync(Guid? id)
        {
            var endpoint = string.Format(Endpoints.GetProductoId, id.ToString());
            var response =
                await Config.SharedClient!.GetFromJsonAsync<Producto?>(endpoint);
            return response;
        }
    }
}
