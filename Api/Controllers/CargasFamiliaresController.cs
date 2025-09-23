using Api.Modelos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;

namespace Api.Controllers
{
    [Route("/[controller]/[action]")]
    [ApiController]
    public class CargasFamiliaresController : Controller
    {
        private readonly IDeltaContextProcedures _contextp;

        public CargasFamiliaresController(IDeltaContextProcedures deltaContextProcedures)
        {
            _contextp = deltaContextProcedures;
        }
        [HttpGet]
        public async Task<IActionResult> ConsultaDatos(string usu, string pass, int idempresa,int anio)

        {
            CargasFamiliares Lista = new CargasFamiliares();
            return Ok(_contextp.CallProceduresConsula(Lista, "PROCK_PERSONAL_WEB.QRY_CARGAS_FAMILIARES(" + idempresa + "," +anio +  ",:1)", usu, pass));
        }
        [HttpPost("batch")]
        public IActionResult Batch(string usu, string pass, [FromBody] List<BatchChange> changes)
        {
            bool ok = true;
            DBOracle DB = new DBOracle();
            using (var conn = new OracleConnection(DB.crearcadena(ClsConfig.DATA_SOURCE, usu, pass)))
            {
                conn.Open();
                foreach (var change in changes)
                {
                    using (var cmd = new OracleCommand("PROCK_PERSONAL_WEB.UPD_CARGAS_FAMILIARES", conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add("PN_CODEMP", OracleDbType.Int32).Value = change.Key ?? change.Data.CODEMP;
                        cmd.Parameters.Add("pv_anio", OracleDbType.Int32).Value = change.Data.ANIO;
                        cmd.Parameters.Add("NUM_CARGAS", OracleDbType.Decimal).Value = change.Data.CARGAS_FAMILIARES;
                        cmd.Parameters.Add("pv_vivienda", OracleDbType.Decimal).Value = change.Data.GASTO_VIVIENDA;
                        cmd.Parameters.Add("pv_educ", OracleDbType.Decimal).Value = change.Data.GASTO_EDUC;
                        cmd.Parameters.Add("pv_salud", OracleDbType.Decimal).Value = change.Data.GASTO_SALUD;
                        cmd.Parameters.Add("pv_vestimenta", OracleDbType.Decimal).Value = change.Data.GASTO_VESTIMENTA;
                        cmd.Parameters.Add("pv_alimentacion", OracleDbType.Decimal).Value = change.Data.GASTO_ALIMENTA;
                        cmd.Parameters.Add("pv_turismo", OracleDbType.Decimal).Value = change.Data.GASTO_TURISMO;

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            return Ok(new { success = true });
        }
    }
    public class BatchChange
    {
        public int? Key { get; set; }
        public CargasFamiliares Data { get; set; }
    }
}
