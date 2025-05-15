using System.ComponentModel.DataAnnotations;

namespace Api.Modelos
{
    public class IESS
    {
        [Key]
        public decimal? CODIGO_IESS { get; set; }
        public string? CARGO_ACTIVIDAD { get; set; }
    }
}
