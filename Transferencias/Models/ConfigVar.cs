using SQLite;

namespace Transferencias.Models
{
    [Table("configVars")]
    public class ConfigVar
    {
        [PrimaryKey]
        public string? Key {  get; set; }
        public string? Value {  get; set; }
    }
}
