using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Transferencias.Models;

public class ItemSolicitudTransferencia : INotifyPropertyChanged
{
    public int Id { get; set; }
    public Guid? ProductoId { get; set; }
    [JsonIgnore]
    public Producto? Producto { get; set; }
    public int Cantidad { get; set; }
    public Guid? UnidadMedidaId { get; set; }
    [JsonIgnore]
    public UnidadMedida? UnidadMedida { get; set; }
    public int EstadoId { get; set; }
    [JsonIgnore]
    public Estado? Estado { get; set; }
    private bool? _pickeado;
    public bool? Pickeado
    {
        get => _pickeado;
        set
        {
            if (_pickeado != value)
            {
                _pickeado = value;
                OnPropertyChanged(nameof(Pickeado));
            }
        }
    }
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}