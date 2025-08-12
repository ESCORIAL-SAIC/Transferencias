using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace Transferencias.Models;

public class Etiqueta
{
    public int Id { get; set; }
    public string? Codigo { get; set; }
    public Guid? ProductoId { get; set; }
    [JsonIgnore]
    public Producto? Producto { get; set; }
    public int Cantidad { get; set; }
    public DateTime Fecha { get; set; }
    public int EstadoId { get; set; }
    [JsonIgnore]
    public Estado? Estado { get; set; }
}
