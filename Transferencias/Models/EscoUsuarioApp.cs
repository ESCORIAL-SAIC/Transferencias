# nullable disable

using Newtonsoft.Json;

namespace Transferencias.Models;

public class EscoUsuarioApp
{
    public int Id { get; set; }
    public string Codigo { get; set; }
    public string NombreCompleto { get; set; }
    public string Contrasena { get; set; }
    public int SectorId { get; set; }
    [JsonIgnore]
    public Sector? Sector { get; set; }
}