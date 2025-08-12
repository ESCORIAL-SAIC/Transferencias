using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace Transferencias.Models;

public class SolicitudTransferencia
{
    public int Id { get; set; }
    public int EstadoId { get; set; }
    [JsonIgnore]
    public Estado? Estado { get; set; }
    public DateTime? Fecha { get; set; }
    public DateTime? FechaModificacion { get; set; }
    public int UsuarioId { get; set; }
    [JsonIgnore]
    public EscoUsuarioApp? Usuario { get; set; }
    public ObservableCollection<ItemSolicitudTransferencia>? ItemsSolicitudTransferencia { get; set; } = [];
    [JsonIgnore]
    public string NombreSolicitud { get => $"Solicitud N° {Id}"; }

}