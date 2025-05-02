using System.ComponentModel.DataAnnotations;

namespace Api.Modelos
{
    public class Paises
    {

        [Key]
        public string CODPAIS { get; set; }
        public string? NOMPAIS { get; set; }
        public string? NACIONALIDAD { get; set; }
        public string? TIPO_NACIONALIDAD { get; set; }

    }
}
