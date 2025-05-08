using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Runtime.Intrinsics.Arm;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Modelos
{
    public class EMP
    {
        [Key]
        public int CODEMP { get; set; }
        public string? APELLIDO_PAT { get; set; }
        public string? APELLIDO_MAT { get; set; }
        public string? APELLIDO_CAS { get; set; }
        public string? NOMBRES { get; set; }
        public string? RAZONSOCIAL { get; set; }
        public string? SEXO { get; set; }
        public string? TIPO { get; set; }
        public string? RUC { get; set; }
        public string? DIRECCION { get; set; }
        public string? DIRECCION_CSV { get; set; }
        public string? CODZONA { get; set; }
        public string? TLF1 { get; set; }
        public string? TLF2 { get; set; }
        public string? PAIS { get; set; }
        public string? PROVINCIA { get; set; }
        public string? CIUDAD { get; set; }
        public DateTime? FECINGRESO { get; set; }
        public DateTime? FECSALIDA { get; set; }
        public string? SEGSOCIAL { get; set; }
        public string? NUMCEDULA { get; set; }
        public string? ESTCIVIL { get; set; }
        public string? PROFESION { get; set; }
        public string? TIPCONTRATO { get; set; }
        public double SUELDO { get; set; }
        public string? CODCCOSTO { get; set; }
        public DateTime? FECNAC { get; set; }
        public int NUMEXT { get; set; }
        public int NUMHIJOS { get; set; }
        public int NUMDIAS { get; set; }
        public string? ACTIVO { get; set; }
        public string? SEGURO { get; set; }
        public string? CODPERSONA { get; set; }
        public string? CELULAR { get; set; }
        public decimal? MNT_DIC { get; set; }
        public decimal? HOR_DIC { get; set; }
        public decimal? BENEFICIO { get; set; }
        public Int16? CODGRUPO { get; set; }
        public decimal? MNT_ABRIL { get; set; }
        public decimal? MNT_ABRIL1 { get; set; }
        public decimal? MNT_ABRIL2 { get; set; }
        public decimal? MNT_DIC_2000 { get; set; }
        public string? CODCATEGORIA { get; set; }
        public string? ACTIVO_REPORTES_AUMENTOS { get; set; }
        public decimal? DEDUCCION { get; set; }
        public string? LIC_MATERNIDAD { get; set; }
        public DateTime? FEC_MATERNIDADI { get; set; }
        public DateTime? FEC_MATERNIDADF { get; set; }
        public string? LIC_ENFERMEDAD { get; set; }
        public DateTime? FEC_ENFERMEDADI { get; set; }
        public DateTime? FEC_ENFERMEDADF { get; set; }
        public string? NOMBRE_INVITACION { get; set; }
        public string? REQUIERE_TRANSP { get; set; }
        public string? CALLE_SRI { get; set; }
        public decimal? NUMERO_SRI { get; set; }
        public string? ASISTE_L { get; set; }
        public string? ASISTE_M { get; set; }
        public string? ASISTE_C { get; set; }
        public string? ASISTE_J { get; set; }
        public string? ASISTE_V { get; set; }
        public decimal? SUELDO_DIC { get; set; }
        public decimal? ANIOS_DOCENCIA { get; set; }
        public int CANTON { get; set; }
        public string? OTRAS_ACTIVIDADES { get; set; }
        public decimal? BENEFICIO2 { get; set; }
        public string? MAIL { get; set; }
        public string? ID_INSTITUCION { get; set; }
        public string? PRESCOLAR { get; set; }
        public DateTime? FEC_INGPRESCOLAR { get; set; }
        public DateTime? FEC_SALPRESCOLAR { get; set; }
        public decimal? ID_CLIENTE { get; set; }
        public DateTime? FECHA_DIGITACION { get; set; }
        public decimal? ID_EMPRESA { get; set; }
        public string? TIPO_DOCUMENTO { get; set; }
        public string? OBSERVACION { get; set; }
        public decimal? CODIGO_IESS { get; set; }
        public string? DISCAPACIDAD { get; set; }
        public decimal? PORC_DISCAPACIDAD { get; set; }
        public string? COND_DISCAPACIDAD { get; set; }
        public string? TIPO_DOC_DISCAPACIDAD { get; set; }
        public string? ID_DISCAPACIDAD { get; set; }
        public string? NIVELESTUDIO { get; set; }
        public string? NOMBRECONYUGE { get; set; }
        public DateTime? FECHAMATRICIVIL { get; set; }
        public DateTime? FECHAMATRIECLE { get; set; }
        public string? TELEFONOCONYUGE { get; set; }
        public string? EMAILPERSONAL { get; set; }
        public string? CONTACTOEMERGENCIA { get; set; }
        public string? PARENTESCOEMERGENCIA { get; set; }
        public string? OTROPARENTEMERGENCIA { get; set; }
        public string? TELEFONOEMERGENCIA { get; set; }
        public string? SEGUROPARTICULAR { get; set; }
        public string? DISCAP_AUDITIVA { get; set; }
        public string? DISCAP_VISUAL { get; set; }
        public string? DISCAP_FISICA { get; set; }
        public string? DISCAP_INTELECTUAL { get; set; }
        public DateTime? FECHAACTUALIZA { get; set; }
        public string? USRACTUALIZA { get; set; }
      
        public string? REFERENCIADOMICILIO { get; set; }
        public string? CONTACTOEMERGENCIA2 { get; set; }
        public string? PARENTESCOEMERGENCIA2 { get; set; }
        public string? OTROPARENTEMERGENCIA2 { get; set; }
        public string? TELEFONOEMERGENCIA2 { get; set; }
        public string? ACTUALIZAWEB { get; set; }
        public string? UNIFICADO { get; set; }
        public string? CODCCOSTO_MINIS { get; set; }
        public decimal? CODJEFA { get; set; }
        public string? LIC_SINSUELDO { get; set; }
        public DateTime? FEC_SINSUELDOINI { get; set; }
        public DateTime? FEC_SINSUELDOFIN { get; set; }
        public string? MOTIVO_SALIDA { get; set; }
        public DateTime? FEC_AIESS { get; set; }
        public DateTime? FEC_INI_JUBILACION { get; set; }
        public decimal? SUELDO_JUBILADO { get; set; }
        public string? LIC_SINSUELDO_EMPR { get; set; }
        public DateTime? FEC_SINSUELDOEMPR_INI { get; set; }
        public DateTime? FEC_SINSUELDOEMPR_FIN { get; set; }
        public decimal? DIAS_PAG_SINSUELDOEMPR { get; set; }
        public string? PRIMER_NOMBRE { get; set; }
        public string? SEGUNDO_NOMBRE { get; set; }
        public decimal? CODIGO_IESS_JUB { get; set; }
        public string? NACIONALIDAD { get; set; }
        public decimal? ANTIGUEDAD { get; set; }
        public string? PERTENECE_OBRA { get; set; }
        [NotMapped]
        public List<rh_familiar_discapacidad>? FamiliarDiscapicidad { get; set; }
        [NotMapped]
        public List<rh_cargas_empleados>? FamiliarCargas { get; set; }
        public List<rh_familiar_enfermedad>? FamiliarEnfermedad { get; set; }
        public List<detalle_grupocentrocosto>? CentroCosto { get; set; }
        public List<crgemp>? Cargos { get; set; }
        public List<grupo_centrocosto>? Grupo { get; set; }
        public List<crgdep>? Departamentos { get; set; }
        public List<CGTEMPCTAS>? CuentasContables { get; set; }    
        public List<NUMCTABCO>? CuentasBancos { get; set; }
        public List<titulosacademicos_emp>? Titulos { get; set; }
        public List<SUELDOS>? Sueldos { get; set; }


    }
}
