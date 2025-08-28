using Api.Services.Interfaces;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Api.Services.Implementations
{
    public class ReportesService : IReportesService
    {
        private readonly ModelOracleContext _contextp;
        private static TripleDESCryptoServiceProvider DES = new TripleDESCryptoServiceProvider();
        private static MD5CryptoServiceProvider MD5 = new MD5CryptoServiceProvider();

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
            return Task.FromResult(dt); ;
        }

        public  Task<DataTable> ListadoPreceptorasxSeccion(string usu, string pass, string periodo, List<string> niveles)
        {
            DeltaContextProcedures obj = new DeltaContextProcedures(_contextp);

            // Construir la sentencia PL/SQL
            var sentencia = $"PROC_K_RRHH_WEB.QRY_PreceptorasxSeccion('{periodo}', '{SeccionesSeleccionadas(niveles)}', :1)";

            // Ejecutar procedimiento y obtener DataTable
            DataTable dt = obj.CallProceduresConsulaDT(sentencia, usu, pass);
            return Task.FromResult(dt);
        }
        public async Task<DataSet> ActualizaDatosEmpleado(string usuario, string pass, int codigo)
        {
            var ds = new DataSet();
            try
            {
                using (var connection = new OracleConnection(new DBOracle().crearcadena(ClsConfig.DATA_SOURCE, usuario, pass)))
                {
                    connection.FetchSize = 10 * 1024 * 1024; // 10 MB
                    await connection.OpenAsync();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "PROC_K_ACTUALIZA_DATOS.RPT_FICHASOCIAL";
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("PN_CODEMP", OracleDbType.Varchar2, 50).Value = codigo;
                        command.Parameters.Add("lista", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                        command.Parameters.Add("titu", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                        command.Parameters.Add("estruc", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                        command.Parameters.Add("disc", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                        command.Parameters.Add("enfermedad", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                        command.InitialLOBFetchSize = 10 * 1024 * 1024; // 10 MB
                        OracleDataAdapter da = new OracleDataAdapter();
                        da.SelectCommand = command;
                        da.Fill(ds);
                        ds.Tables[0].Columns.Add("TIPO_VI");
                        ds.Tables[0].Columns.Add("URB");
                        ds.Tables[0].Columns.Add("MZ");
                        ds.Tables[0].Columns.Add("CALLE_PRIN");
                        ds.Tables[0].Columns.Add("NO_CASA");
                        ds.Tables[0].Columns.Add("CALLE_SEC");
                        ds.Tables[0].Columns.Add("COND");
                        ds.Tables[0].Columns.Add("NO_DEPA");
                        ds.Tables[0].Columns.Add("KM");
                        ds.Tables[0].Columns.Add("VIA");
                        foreach (DataRow i in ds.Tables[0].Rows)
                        {
                            string[] c = i["DIRECCION_CSV"].ToString()
                               .Split(new string[] { "," }, StringSplitOptions.None);
                            i["URB"] = c[0];
                            i["MZ"] = c[1];
                            i["CALLE_PRIN"] = c[2];
                            i["NO_CASA"] = c[3];
                            i["CALLE_SEC"] = c[4];
                            i["COND"] = c[5];
                            i["NO_DEPA"] = c[6];
                            i["KM"] = c[7];
                            i["VIA"] = c[8];
                            if (c.Length == 10)
                                i["TIPO_VI"] = c[9];
                        }

                    }
                }
            }
            catch (Exception)
            {
                ds = null;
                throw;
            }
            return ds;
        }
        public Task<DataTable> Prestamos(string usu, string pass, string empresa, string? saldo,DateTime? desde,DateTime? hasta)
        {
            DeltaContextProcedures obj = new DeltaContextProcedures(_contextp);
            var fdesde = desde.HasValue ? "'" + desde.Value.ToString("dd/MM/yyyy") + "'" : "null";
            var fhasta = hasta.HasValue ? "'" + hasta.Value.ToString("dd/MM/yyyy") + "'" : "null";



            // Construir la sentencia PL/SQL
            var sentencia = $"select e.razonsocial,p.codprestamo,P.TIPO,P.OBSERVACION,P.FECHAINI,P.NUMCUOTAS," +
                "P.VALOR_CUOTA,P.FECHAFIN,P.SALDO from developer1.emp e inner join developer1.prestamos p on "+
                "P.CODEMP=E.CODEMP and p.estado=1 where e.activo='S' AND E.ID_EMPRESA=" + empresa + " ";
            if (saldo != "")
            {
                sentencia += " AND p.saldo > " + saldo;
            }
            if (desde.HasValue)
            {
                sentencia += " and p.fechaini between " + fdesde + " and " + fhasta ;
            }

            // Ejecutar procedimiento y obtener DataTable
            DataTable dt = obj.consultaSimple(sentencia, usu, pass);
            return Task.FromResult(dt);
        }
        private static byte[] MD5Hash(string value)
        {
            return MD5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(value));
        }
        public string Encrypt(string dataToEncrypt, string password)
        {
            DES.Key = MD5Hash(password);
            DES.Mode = CipherMode.ECB;
            byte[] Buffer = ASCIIEncoding.ASCII.GetBytes(dataToEncrypt);
            return Convert.ToBase64String(DES.CreateEncryptor().TransformFinalBlock(Buffer, 0, Buffer.Length));
        }
    }
}
