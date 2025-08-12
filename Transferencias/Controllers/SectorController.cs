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
    public class SectorController
    {
        public static async Task<Sector?> GetByIdAsync(int id)
        {
            var endpoint = string.Format(Endpoints.GetEscoSectorId, id.ToString());
            var response =
                await Config.SharedClient!.GetFromJsonAsync<Sector>(endpoint);
            return response;
        }
    }
}
