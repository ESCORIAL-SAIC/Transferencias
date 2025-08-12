using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace Transferencias.Models;

public class Transferencia
{
    public int Id { get; set; }
    public int SolicitudTransferenciaId { get; set; }
    [JsonIgnore]
    public SolicitudTransferencia? SolicitudTransferencia { get; set; }
    public int EstadoId { get; set; }
    [JsonIgnore]
    public Estado? Estado { get; set; }
    public DateTime? Fecha { get; set; }
    public DateTime? FechaModificacion { get; set; }
    public bool? ProcesadoTransactor { get; set; }
    public string? Referencia { get; set; }
    public DateTime? FechaProcesado { get; set; }
    public Guid? DepositoOriId { get; set; }
    [JsonIgnore]
    public Deposito? DepositoOrigen { get; set; }
    public Guid? DepositoDesId { get; set; }
    [JsonIgnore]
    public Deposito? DepositoDestino { get; set; }
    public int UsuarioId { get; set; }
    [JsonIgnore]
    public EscoUsuarioApp? Usuario { get; set; }
    public ObservableCollection<ItemTransferencia>? ItemsTransferencia { get; set; }
    [JsonIgnore]
    public string NombreTransferencia { get => $"Transferencia N° {Id}"; }
}