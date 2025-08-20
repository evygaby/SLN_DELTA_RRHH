using Api.Services.Interfaces;
using System.Data;

namespace Api.Services.Implementations
{
    public class ReportesService : IReportesService
    {
        private readonly ModelOracleContext _contextp;

        public ReportesService(ModelOracleContext contextp)
        {
            _contextp = contextp;
        }
        public string SeccionesSeleccionadas(List<string> secciones)
        {
            string codigos = "";

            foreach (var item in secciones)
            {
                int niveldesde = int.Parse(item.Split("*")[0]);
                int nivelhasta = int.Parse(item.Split("*")[1]);
                for (var dn = niveldesde; dn <= nivelhasta; dn++)
                    codigos = codigos + dn.ToString() + ",";
            }

            return codigos;
        }
        public  Task<DataTable> ListadoPEGsxSeccion(string usu, string pass, string periodo, List<string> niveles)
        {
            DeltaContextProcedures obj = new DeltaContextProcedures(_contextp);

            // Construir la sentencia PL/SQL
            var sentencia = $"PROC_K_RRHH_WEB.QRY_PegPrecepxSeccion('{periodo}', '{SeccionesSeleccionadas(niveles)}', :1)";

            // Ejecutar procedimiento y obtener DataTable
            DataTable dt = obj.CallProceduresConsulaDT(sentencia, usu, pass);

            // Convertir a lista de diccionarios
            //var lista = dt.AsEnumerable()
            //             .Select(row => dt.Columns
            //                 .Cast<DataColumn>()
            //                 .ToDictionary(col => col.ColumnName, col => row[col]))
            //             .ToList();

            return Task.FromResult(dt); ;
        }

        public  Task<DataTable> ListadoPreceptorasxSeccion(string usu, string pass, string periodo, List<string> niveles)
        {
            DeltaContextProcedures obj = new DeltaContextProcedures(_contextp);

            // Construir la sentencia PL/SQL
            var sentencia = $"PROC_K_RRHH_WEB.QRY_PreceptorasxSeccion('{periodo}', '{SeccionesSeleccionadas(niveles)}', :1)";

            // Ejecutar procedimiento y obtener DataTable
            DataTable dt = obj.CallProceduresConsulaDT(sentencia, usu, pass);

            // Convertir a lista de diccionarios
            //var lista = dt.AsEnumerable()
            //             .Select(row => dt.Columns
            //                 .Cast<DataColumn>()
            //                 .ToDictionary(col => col.ColumnName, col => row[col]))
            //             .ToList();

            return Task.FromResult(dt);
        }
    }
}
