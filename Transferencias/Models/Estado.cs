using Newtonsoft.Json;

namespace Transferencias.Models;

public class Estado
{
    public int? Id { get; set; }
    public string? Codigo { get; set; }
    public string? Descripcion { get; set; }
    [JsonIgnore]
    public Color Semaforo
    {
        get
        {
            return Id switch
            {
                Tipo.Activo => Colors.Green,
                Tipo.Inactivo => Colors.Red,
                Tipo.Baja => Colors.Red,
                Tipo.Pendiente => Colors.Orange,
                Tipo.PendienteAprobacionProduccion => Colors.Yellow,
                _ => (Color)Application.Current!.Resources["Primary"],
            };
        }
    }
    public static class Tipo
    {
        public const int Activo = 5;
        public const int Inactivo = 6;
        public const int Baja = 7;
        public const int Pendiente = 8;
        public const int PendienteAprobacionProduccion = 9;
        public const int Finalizado = 10;
        public const int Aprobado = 11;
        public const int TransferenciaGenerada = 12;
    }
}
