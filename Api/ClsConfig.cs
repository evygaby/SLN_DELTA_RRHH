
using System;

namespace Api
{
    public sealed class LogDiskSettings
    {
        public string Ruta { get; set; }
        public string Layout { get; set; }
        public string FileName { get; set; }
    }
    public sealed class LogSettings
    {
        public bool Habilitar { get; set; }
        public short Destino { get; set; }
        public LogDiskSettings Disk { get; set; }


    }
    public sealed class ClsConfig
    {
         public static string? UrlFotos { get; set; }
        public static string? UrlServeEncuesta { get; set; }
        public static string? UrlServeEncuesta360 { get; set; }
        public static string? UrlServeEncuestaclima { get; set; }
        public static string? UrlServesinc { get; set; }

        
        public static int? minutosTokenExpira { get; set; }
        
        public static LogSettings LOG { get; set; }

        public static string? MensajeErrorGenerico { get; set; }



        public static string? DATA_SOURCE { get; set; }
        public static string? USER_ID { get; set; }
        public static string? PASSWORD { get; set; }


        public static string? cadenaoracle { get; set; }

        private static string _BaseDatos;

        private static string _nombreServicio;
        private static string _PatchLog;
        private static string _nombreApp;
        private static string _GuardarLog;


        public static string NombreApp
        {
            get
            {
                return _nombreApp;
            }

            set
            {
                if ((_nombreApp ?? "") == (value ?? ""))
                {
                    return;
                }

                _nombreApp = value;
            }
        }
        public static string BaseDatos
        {
            get
            {
                return _BaseDatos;
            }

            set
            {
                if ((_BaseDatos ?? "") == (value ?? ""))
                {
                    return;
                }

                _BaseDatos = value;
            }
        }
        public static string PatchLog
        {
            get
            {
                return _PatchLog;
            }

            set
            {
                if ((_PatchLog ?? "") == (value ?? ""))
                {
                    return;
                }

                _PatchLog = value;
            }
        }
        public static string GuardarLog
        {
            get
            {
                return _GuardarLog;
            }

            set
            {
                if ((_GuardarLog ?? "") == (value ?? ""))
                {
                    return;
                }

                _GuardarLog = value;
            }
        }


        public static string NombreServicio
        {
            get
            {
                return _nombreServicio;
            }

            set
            {
                if ((_nombreServicio ?? "") == (value ?? ""))
                {
                    return;
                }

                _nombreServicio = value;
            }
        }



    }

    public class Constants
    {

        public class EstadosPlantillas
        {
            public const short Table = 1001;

            public const bool   Activo = true;

            public const string Aprovado = "AP";

            public const bool  Inactivo = false;

            public const string Pendiente = "P";
        }

        public class TipoProfesor
        {
            public const short Table = 1004;

            public const string Docente = "D";

           
        }

    }
    public enum TipoRegistro
    {
        Nuevo = 0,
        Editar = 1
    }
}