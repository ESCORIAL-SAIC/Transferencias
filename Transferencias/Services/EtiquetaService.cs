using Transferencias.Controllers;
using Transferencias.Models;
using Transferencias.Resources.Values;

namespace Transferencias.Services
{
    public class EtiquetaService
    {
        public async Task<(Etiqueta? Etiqueta, string? Error, bool StockBajo)> ReadBarcodeAsync(string barcode)
        {
            try
            {
                var etiqueta = await EtiquetaController.GetByCodeAsync(barcode);
                if (etiqueta is null)
                    return (null, AppStrings.AlertErrorLabelNotFoundMessage, false);

                etiqueta.Producto = await ProductoController.GetByIdAsync(etiqueta.ProductoId);

                var origin = await Config.GetOrigin();
                var hasStock = await Fun.CheckStockByDepoAsync(
                    etiqueta.Producto!.Id,
                    origin!.Id,
                    etiqueta.Cantidad
                );

                return (etiqueta, null, !hasStock);
            }
            catch (HttpRequestException)
            {
                return (null, AppStrings.AlertErrorLabelNotFoundMessage, false);
            }
            catch (Exception ex)
            {
                return (null, ex.Message, false);
            }
        }
    }
}