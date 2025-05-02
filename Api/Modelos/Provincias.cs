using System.ComponentModel.DataAnnotations;

namespace Api.Modelos
{
    public class Provincias
    {
        [Key]
        public string? CODPROV { get; set; }
        public string? NOMPROV { get; set; }
        public string? CODPAIS { get; set; }
        public int COD_SRI { get; set; }
    }
}
