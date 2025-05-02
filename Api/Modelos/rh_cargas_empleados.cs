namespace Api.Modelos
{
    public class rh_cargas_empleados
    {
        public decimal? ID_HIJO { get; set; }
       public Int16? CODEMP { get; set; }
        public string? TIPO_CARGA { get; set; }
        public string? NOMBRE_CARGA { get; set; }
        public DateTime? FECHA_NACIMIENTO { get; set; }
        public string? SEXO { get; set; }
       public   Int16? ID_EMPRESA { get; set; }
        public DateTime? FECHA_INGRESO { get; set; }
        public string? USUARIO_INGRESO { get; set; }
        public string? OTROTIPO { get; set; }
        public string? INSTITUCION { get; set; }
        public DateTime? FECHA_MODIFICA { get; set; }
        public string? USUARIO_MODIFICA { get; set; }
        public string? GRADOCURSO { get; set; }
        public string? OTRAINSTITUCION { get; set; }
    }
}
