using System.ComponentModel.DataAnnotations.Schema;

namespace Transferencias.Models;

public class ItemInventario
{
    public Guid? DepositoId { get; set; }
    [NotMapped]
    public Deposito? Deposito { get; set; }
    public Guid? ProductoId { get; set; }
    [NotMapped]
    public Producto? Producto { get; set; }
    public decimal? Cantidad2Cantidad { get; set; }
}