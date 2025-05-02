using System.ComponentModel.DataAnnotations;

namespace Api.Modelos
{
    public class detalle_grupocentrocosto
    {
        [Key]
        public string? GCODCCOSTO { get; set; }
        public decimal? CODEMPLEADO { get; set; }
        public decimal? IDEMPRESA { get; set; }
        public string? ESTADO_GR { get; set; }

    }
}
