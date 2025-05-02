using System.ComponentModel.DataAnnotations;

namespace Api.Modelos
{
    public class Zonas
    {
        [Key]
        public string? CODZONA { get; set; }
        public string? NOMZONA { get; set; }
        public Decimal? ORDEN_REPORTE { get; set; }
    }
}
