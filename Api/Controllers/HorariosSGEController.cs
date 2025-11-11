using Api.Modelos;
using Api.Services.Interfaces;
using ExcelDataReader;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Text;
namespace Api.Controllers

{
    [Route("/[controller]/[action]")]
    [ApiController]
    public class HorariosSGEController : Controller
    {
        private readonly IDeltaContextProcedures _contextp;
        private readonly IReportesService _reportesService;
        public HorariosSGEController(IDeltaContextProcedures deltaContextProcedures, IReportesService reportesService)
        {

            _contextp = deltaContextProcedures;
            _reportesService = reportesService;
        }

        [HttpGet]
        public async Task<IActionResult> DatosHorarios(string usu, string pass, int idempresa )

        {
            var contextoOracle = new ModelOracleContext();
            DeltaContextProcedures obj = new DeltaContextProcedures(contextoOracle);
            var sentencia = "PROCK_PERSONAL_WEB.QRY_HORARIO_PERSONAL(" + idempresa + ",:1)";
            DataTable dt = obj.CallProceduresConsulaDT(sentencia, usu, pass);

            var lista = dt.AsEnumerable()
                     .Select(row => dt.Columns
                         .Cast<DataColumn>()
                         .ToDictionary(col => col.ColumnName, col => row[col]))
                     .OrderBy(dict => dict["RAZONSOCIAL"].ToString())
                     .ToList();
            return Json(lista);
        }
        [HttpPost("batch")]
        public async Task<IActionResult> Batch(string usu, string pass,int id_empresa, [FromBody] List<BatchChangeHE> changes)
        {
            DBOracle DB = new DBOracle();
            using (var conn = new OracleConnection(DB.crearcadena(ClsConfig.DATA_SOURCE, usu, pass)))
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (var change in changes)
                        {
                            using (var cmd = new OracleCommand("PROCK_PERSONAL_WEB.OP_ModificaHorario", conn))
                            {
                                string entrada = change.Data.HOR_ENTRADA.ToString("dd/MM/yyyy HH:mm");
                                string salida = change.Data.HOR_SALIDA.ToString("dd/MM/yyyy HH:mm");

                                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                                cmd.Transaction = transaction;

                                cmd.Parameters.Add("PN_EMPRESA", OracleDbType.Int32).Value = id_empresa;
                                cmd.Parameters.Add("PN_EMP", OracleDbType.Int32).Value = change.Key ?? change.Data.CODEMP;
                                cmd.Parameters.Add("PN_DIA", OracleDbType.Int32).Value = change.Data.HOR_DIA;
                                cmd.Parameters.Add("PV_ENTRADA", OracleDbType.Varchar2).Value = entrada;
                                cmd.Parameters.Add("PV_SALIDA", OracleDbType.Varchar2).Value = salida;
                                cmd.Parameters.Add("PV_REFERENCIA", OracleDbType.Varchar2).Value = change.Data.HOR_REFERENCIA;

                                cmd.ExecuteNonQuery();
                            }
                        }

                        // si todo salió bien
                        transaction.Commit();
                        return Ok(new { success = true });
                    }
                    catch (Exception ex)
                    {
                        // algo falló → rollback
                        transaction.Rollback();
                        return BadRequest(new { success = false, message = ex.Message });
                    }
                }
            }
        }
    }
    public class BatchChangeHE
    {
        public int? Key { get; set; }
        public HORARIOSGE Data { get; set; }
    }
}
