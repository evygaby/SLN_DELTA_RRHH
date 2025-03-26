using System.ComponentModel.DataAnnotations;

namespace Api.Modelos
{
    public class EMP_FICHA_SOCIAL
    {
        [Key]
        public int? CODEMP { get; set; }
        public string? SEGURO_PRIVADO { get; set; }
        public string? TIPO_SEGURO { get; set; }
        public string? OTRAS_ACTIVIDADES { get; set; }
        public string? CUAL_OTRA_ACTIVIDAD { get; set; }
        public string? PRACTICA_DEPORTES { get; set; }
        public string? INDIQUE_DEPORTE { get; set; }
        public string? TRASLADO_EXPRESO { get; set; }
        public string? TRASLADO_TAXI { get; set; }
        public string? TRASLADO_VEHICULO_PROP { get; set; }
        public string? TRASLADO_VEHICULO_FAM { get; set; }
        public string? TRASLADO_BUS { get; set; }
        public string? TRASLADO_MOTO { get; set; }
        public string? TRASLADO_BICI { get; set; }
        public string? TRASLADO_CAMINA { get; set; }
        public string? ES_EMPRENDEDOR { get; set; }
        public string? EXPLIQUE_EMPRENDIMIENTO { get; set; }
        public string? TIPO_VIVIENDA { get; set; }
        public string? TENENCIA_VIVIENDA { get; set; }
        public string? TIEMPO_VIVIENDA { get; set; }
        public string? MATERIAL_PAREDES { get; set; }
        public string? MATERIAL_PISO { get; set; }
        public string? SB_ENERGIA { get; set; }
        public string? SB_AGUA { get; set; }
        public string? SB_ALCANTARILLADO { get; set; }
        public string? SB_TELEFONO { get; set; }
        public string? SB_INTERNET { get; set; }
        public string? MIEMBRO_FAM_DISCAP { get; set; }
        public string? MIEMBRO_FAM_ENFCATAS { get; set; }
        public string? USUARIO_INGRESA { get; set; }
        public DateTime? FECHA_INGRESA { get; set; }
        public string? USUARIO_MODIFICA { get; set; }
        public DateTime? FECHA_MODIFICA { get; set; }
    }
}
