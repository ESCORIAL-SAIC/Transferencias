using System.Text.Json.Serialization;

namespace Transferencias.Models;

public class ItemTransferencia
{
    public int Id { get; set; }
    public Guid? ProductoId { get; set; }
    [JsonIgnore]
    public Producto? Producto { get; set; }
    public int Cantidad { get; set; }
    public Guid? UnidadMedidaId { get; set; }
    [JsonIgnore]
    public UnidadMedida? UnidadMedida { get; set; }
    [JsonIgnore]
    public Transferencia? Transferencia { get; set; }
    public int EstadoId { get; set; }
    [JsonIgnore]
    public Estado? Estado { get; set; }
}