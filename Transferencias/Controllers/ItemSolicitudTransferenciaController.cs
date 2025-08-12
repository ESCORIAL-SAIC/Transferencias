using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Transferencias.Models;
using Transferencias.Resources.Values;

namespace Transferencias.Controllers
{
    public class ItemSolicitudTransferenciaController
    {
        public static async Task<HttpStatusCode> UpdateAsync(int itemSolicitudId, ItemSolicitudTransferencia item)
        {
            var endpoint = string.Format(Endpoints.PutItemSolicitudTransferenciaId, itemSolicitudId.ToString());
            var response =
                await Config.SharedClient!.PutAsJsonAsync(endpoint, item);
            return response.StatusCode;
        }
    }
}
