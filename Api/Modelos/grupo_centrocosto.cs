using System.ComponentModel.DataAnnotations;

namespace Api.Modelos
{
    public class grupo_centrocosto
    {

        [Key]
        public string? GCODCCOSTO { get; set; }
        public string? NOMGRUPOCCOSTO { get; set; }
        public decimal? ORDEN_REPORTE { get; set; }
    }
}
