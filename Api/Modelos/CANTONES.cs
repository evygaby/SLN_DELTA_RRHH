using System.ComponentModel.DataAnnotations;

namespace Api.Modelos
{
    public class CANTONES
    {
        
        public int  CODCANTON { get; set; }
        public string? CODPROV { get; set; }
        public string? CODPAIS { get; set; }
        public string? NOMCANTON { get; set; }
    }
}
