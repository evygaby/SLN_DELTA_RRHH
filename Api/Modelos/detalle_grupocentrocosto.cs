using System.ComponentModel.DataAnnotations;

namespace Api.Modelos
{
    public class detalle_grupocentrocosto
    {
        [Key]
        public string? GCODCCOSTO { get; set; }
        public int? CODEMPLEADO { get; set; }
        public int? IDEMPRESA { get; set; }
        public string? ESTADO_GR { get; set; }

    }
}
