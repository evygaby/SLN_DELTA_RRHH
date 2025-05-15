using System.ComponentModel.DataAnnotations;

namespace Api.Modelos
{
    public class SEGUROS
    {
        [Key]
        public string? Cadena    { get; set; }
        public string? Descripcion { get; set; }
    }
}
