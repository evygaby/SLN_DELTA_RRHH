namespace Api.Modelos
{
    public class SUELDOS
    {
        public Int16? CODEMP { get; set; }
        public DateTime? FECHA { get; set; }
        public string? TIPCONTRATO { get; set; }
        public string? CENTRO_COSTO { get; set; }
        public decimal? A_PAGAR { get; set; }
        public decimal? SUELDO { get; set; }
        public decimal? EXTRAS { get; set; }

        public decimal? OTROS { get; set; }
        public decimal? INGRESOS { get; set; }
        public decimal? EGRESOS { get; set; }
        public decimal? DIAS_ENF { get; set; }
        public decimal? DIAS_MAT { get; set; }
        public decimal? SINSUELDO { get; set; }
        
    }
}
