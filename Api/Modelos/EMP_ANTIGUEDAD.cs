namespace Api.Modelos
{
    public class EMP_ANTIGUEDAD
    {
        public int? ID_CONTRATO { get; set; }
        public string? IDENTIFICACION { get; set; }
        public DateTime FECHA_DESDE { get; set; }
        public DateTime? FECHA_HASTA { get; set; }
        public int ID_EMPRESA { get; set; }
        public string? OBSERVACION { get; set; }
        public string? TIPCONTRATO { get; set; }
        public string? TERM_CONTRATO { get; set; }
        public string? ESTADO_REGISTRO { get; set; }
        public string? DETALLE_TERM { get; set; }
    }
}
