using System.ComponentModel.DataAnnotations;

namespace Api.Modelos
{
    public class EXT
    {
        [Key]
        public int NUMEXT { get; set; }
        public string? DSCEXT { get; set; }
    }
}
