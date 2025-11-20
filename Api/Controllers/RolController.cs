using Api.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Api.Controllers
{
    [Route("/[controller]/[action]")]
    [ApiController]
    public class RolController : Controller
    {
        private readonly IDeltaContextProcedures _contextp;
        private readonly IReportesService _reportesService;
        public RolController(IDeltaContextProcedures deltaContextProcedures, IReportesService reportesService)
        {

            _contextp = deltaContextProcedures;
            _reportesService = reportesService;
        }
        [HttpGet]
        public async Task<IActionResult> ListadoRolGenerado(string usu, string pass,Int32 empresa, DateTime fecha,string ccosto,string tipo_rol)

        {
            DataTable dt = await _reportesService.ListaRol(usu, pass,empresa,fecha,ccosto,tipo_rol);
            var lista = dt.AsEnumerable()
                     .Select(row => dt.Columns
                         .Cast<DataColumn>()
                         .ToDictionary(col => col.ColumnName, col => row[col]))
                     .ToList();
            return Json(lista);
        }
        [HttpGet]
        public async Task<IActionResult> ComparativoRolesMensual(string usu, string pass, Int32 empresa, DateTime? fecha)
        {
            var ffecha = fecha.HasValue ? "'" + fecha.Value.ToString("dd/MM/yyyy") + "'" : "null";
            var contextoOracle = new ModelOracleContext();
            DeltaContextProcedures obj = new DeltaContextProcedures(contextoOracle);
            var sentencia = "DEVELOPER1.prock_personal_web.QRY_COMPARAMENSUAL(" + empresa + "," + ffecha + ",:1)";
            DataTable dt = obj.CallProceduresConsulaDT(sentencia, usu, pass);
            var lista = dt.AsEnumerable()
                     .Select(row => dt.Columns
                         .Cast<DataColumn>()
                         .ToDictionary(col => col.ColumnName, col => row[col]))
                     .ToList();
            return Json(lista);
        }
        [HttpGet]
        public async Task<IActionResult> ObtenerDecimos(string usu, string pass, Int32 empresa,DateTime fecha)
        {
            var contextoOracle = new ModelOracleContext();
            DeltaContextProcedures obj = new DeltaContextProcedures(contextoOracle);
            var sentencia = "DEVELOPER1.RH_PROCESOS.QRY_ASIGNADECIMOS(" + empresa + "," + fecha.ToString("yyyy") + ",:1)";
            DataTable dt = obj.CallProceduresConsulaDT(sentencia, usu, pass);
            var lista = dt.AsEnumerable()
                     .Select(row => dt.Columns
                         .Cast<DataColumn>()
                         .ToDictionary(col => col.ColumnName, col => row[col]))
                     .ToList();
            return Json(lista);
        }
    }
}
