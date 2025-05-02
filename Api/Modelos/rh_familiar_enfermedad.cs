namespace Api.Modelos
{
    public class rh_familiar_enfermedad
    {
        public decimal? IDFAMILIA { get; set; }
        public decimal CODEMP { get; set; }
        public string? NOMBRECOMPLETO { get; set; }
        public string? PARENTESCO { get; set; }
        public string? ESTADO { get; set; }
        public DateTime? FECHA_INGRESO { get; set; }
        public string? USR_INGRESO { get; set; }
        public DateTime? FECHA_ACTUALIZA { get; set; }
        public string? USR_ACTUALIZA { get; set; }
        public Int16? ID_EMPRESA { get; set; }
        public string? RESPONSABILIDAD_ECON { get; set; }
        public string? ENFERMEDAD { get; set; }
    }
}
