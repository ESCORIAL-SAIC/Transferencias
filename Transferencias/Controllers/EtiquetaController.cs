using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Transferencias.Models;
using Transferencias.Resources.Values;

namespace Transferencias.Controllers
{
    public class EtiquetaController
    {
        public static async Task<Etiqueta?> GetByCodeAsync(string code)
        {
            var endpoint = string.Format(Endpoints.GetEscoTxEtiquetaCodigo, code);
            var response =
                await Config.SharedClient!.GetFromJsonAsync<Etiqueta>(endpoint);
            return response;
        }
    }
}
