using System.ComponentModel.DataAnnotations;

namespace Api.Modelos
{
    public class CargasFamiliares
    {
        [Key]
        public int CODEMP { get; set; }
        public string? RAZONSOCIAL { get; set; }
        public decimal GASTOS { get; set; }
        public int ANIO { get; set; }
        public string? USR_ING { get; set; }
        public DateTime FEC_ING { get; set; }
        public string? ESTADO { get; set; }
        public decimal GASTO_VIVIENDA { get; set; }
        public decimal GASTO_SALUD { get; set; }
        public decimal GASTO_EDUC { get; set; }
        public decimal GASTO_ALIMENTA { get; set; }
        public decimal GASTO_VESTIMENTA { get; set; }
        public int ID_EMPRESA { get; set; }
        public decimal GASTO_TURISMO { get; set; }
        public int CARGAS_FAMILIARES { get; set; }
    }
}
