using Api.Modelos;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace Api.Controllers
{
    [Route("/[controller]/[action]")]
    [ApiController]
    public class AntiguedadController : Controller
    {
        private readonly IDeltaContextProcedures _contextp;

        public AntiguedadController(IDeltaContextProcedures deltaContextProcedures)
        {

            _contextp = deltaContextProcedures;

        }
        [HttpGet]
        public async Task<IActionResult> DatosContratos(string usu, string pass)

        {
            var contextoOracle = new ModelOracleContext();
            DeltaContextProcedures obj = new DeltaContextProcedures(contextoOracle);
            var sentencia = "PROCK_PERSONAL_WEB.qryContratos(:1)";
            DataTable dt = obj.CallProceduresConsulaDT(sentencia, usu, pass);

            var lista = dt.AsEnumerable()
                     .Select(row => dt.Columns
                         .Cast<DataColumn>()
                         .ToDictionary(col => col.ColumnName, col => row[col]))
                     .OrderBy(dict => dict["RAZONSOCIAL"].ToString())
                     .ToList();
            return Json(lista);
        }
        [HttpPost]
        public async Task<IActionResult> Update([FromQuery] string usu, [FromQuery] string pass, [FromBody] EMP_ANTIGUEDAD datos)
        {
            DBOracle DB = new DBOracle();
            using (var conn = new OracleConnection(DB.crearcadena(ClsConfig.DATA_SOURCE, usu, pass)))
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {

                        using (var cmd = new OracleCommand("PROCK_PERSONAL_WEB.upd_contrato", conn))
                        {
                            var fHasta = datos.FECHA_HASTA.HasValue ?datos.FECHA_HASTA.Value.ToString("dd/MM/yyyy") : "";
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.Transaction = transaction;

                            cmd.Parameters.Add("pn_id", OracleDbType.Int32).Value = datos.ID_CONTRATO;
                            cmd.Parameters.Add("pv_cedula", OracleDbType.Varchar2).Value = datos.IDENTIFICACION;
                            cmd.Parameters.Add("pd_desde", OracleDbType.Varchar2).Value = datos.FECHA_DESDE.ToString("dd/MM/yyyy");
                            if (datos.FECHA_HASTA.HasValue)
                                cmd.Parameters.Add("pd_hasta", OracleDbType.Varchar2).Value =fHasta;
                            else
                                cmd.Parameters.Add("pd_hasta", OracleDbType.Varchar2).Value = DBNull.Value;
                            cmd.Parameters.Add("pn_empresa", OracleDbType.Varchar2).Value = datos.ID_EMPRESA;
                            cmd.Parameters.Add("pv_observa", OracleDbType.Varchar2).Value = datos.OBSERVACION;
                            cmd.Parameters.Add("pv_tipContrato", OracleDbType.Varchar2).Value = datos.TIPCONTRATO;
                            cmd.Parameters.Add("pv_finContrato", OracleDbType.Varchar2).Value = datos.TERM_CONTRATO;
                            cmd.Parameters.Add("pv_detalle", OracleDbType.Varchar2).Value = datos.DETALLE_TERM;

                            cmd.ExecuteNonQuery();
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
        [HttpPost]
        public async Task<IActionResult> Insert([FromQuery] string usu, [FromQuery] string pass, [FromBody] EMP_ANTIGUEDAD datos)
        {
            DBOracle DB = new DBOracle();
            using (var conn = new OracleConnection(DB.crearcadena(ClsConfig.DATA_SOURCE, usu, pass)))
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {

                        using (var cmd = new OracleCommand("PROCK_PERSONAL_WEB.ins_contrato", conn))
                        {
                            var fHasta = datos.FECHA_HASTA.HasValue ? datos.FECHA_HASTA.Value.ToString("dd/MM/yyyy") : "";
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.Transaction = transaction;
                            cmd.Parameters.Add("pv_cedula", OracleDbType.Varchar2).Value = datos.IDENTIFICACION;
                            cmd.Parameters.Add("pv_desde", OracleDbType.Varchar2).Value = datos.FECHA_DESDE.ToString("dd/MM/yyyy");
                            if (datos.FECHA_HASTA.HasValue)
                                cmd.Parameters.Add("pv_hasta", OracleDbType.Varchar2).Value = fHasta;
                            else
                                cmd.Parameters.Add("pv_hasta", OracleDbType.Varchar2).Value = DBNull.Value;
                            cmd.Parameters.Add("pn_empresa", OracleDbType.Varchar2).Value = datos.ID_EMPRESA;
                            cmd.Parameters.Add("pv_observa", OracleDbType.Varchar2).Value = datos.OBSERVACION;
                            cmd.Parameters.Add("pv_tipContrato", OracleDbType.Varchar2).Value = datos.TIPCONTRATO;
                            cmd.Parameters.Add("pv_finContrato", OracleDbType.Varchar2).Value = datos.TERM_CONTRATO;
                            cmd.Parameters.Add("pv_detalle", OracleDbType.Varchar2).Value = datos.DETALLE_TERM;

                            cmd.ExecuteNonQuery();
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
    
    [HttpPost]
        public async Task<IActionResult> Delete([FromQuery] string usu, [FromQuery] string pass, [FromQuery] int idContrato)
        {
            DBOracle DB = new DBOracle();
            using (var conn = new OracleConnection(DB.crearcadena(ClsConfig.DATA_SOURCE, usu, pass)))
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {

                        using (var cmd = new OracleCommand("PROCK_PERSONAL_WEB.DEL_CONTRATO", conn))
                        {
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.Transaction = transaction;
                            cmd.Parameters.Add("PN_ID", OracleDbType.Int32).Value = idContrato;
                            cmd.ExecuteNonQuery();
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
}