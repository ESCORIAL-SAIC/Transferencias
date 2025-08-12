using System.Net.Http.Json;
using Transferencias.Models;
using Transferencias.Resources.Values;

namespace Transferencias.Controllers;

public class EscoTxConfigController
{
    public static async Task<EscoTxConfig?> GetAppVersionAsync()
    {
        var endpoint = Endpoints.GetAppVersion;
        var response =
            await Config.SharedClient!.GetFromJsonAsync<EscoTxConfig>(endpoint);
        return response;
    }}