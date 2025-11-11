using System.Data;
using System.Data.OleDb;
using System.Net;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("/[controller]/[action]")]
    public class SftpController : Controller
    {

        private readonly IDeltaContextProcedures _contextp;
        private readonly IWebHostEnvironment _env;

        private readonly ILogger<SftpController> _logger;

        public SftpController(IDeltaContextProcedures deltaContextProcedures,IWebHostEnvironment env, ILogger<SftpController> logger)
        {

            _contextp = deltaContextProcedures;
            _env = env;
            _logger = logger;

        }
              private DataTable EjecutaComando(string ruta, string paraBusq, ref string mensaje)
        {
            DataTable dt = new DataTable();

            // OleDb and its ConnectionString are only supported on Windows.
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                mensaje = "OleDb-based database access is only supported on Windows.";
                return dt;
            }

            using (OleDbConnection conn = new OleDbConnection())
            {
                conn.ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + ruta;

                try
                {
                    using (OleDbCommand comando = new OleDbCommand(paraBusq, conn))
                    {
                        conn.Open();
                        using (System.Data.Common.DbDataReader reader = comando.ExecuteReader())
                        {
                            dt.Load(reader);
                        }
                    }
                    return dt;
                }
                catch (Exception ex)
                {
                    mensaje = ex.Message;
                    return dt;
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                        conn.Close();
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> consultarasistencia(string consulta, int idempresa)

        {
            string carpetaDestino = Path.Combine(_env.WebRootPath, "archivos");
                  string localPath = "";
            if (idempresa == 1)
            {
                   localPath = carpetaDestino + "//Delta.mdb";
            }
            if (idempresa == 2)
            {
                   localPath = carpetaDestino + "//Presco.mdb";
            }
      

          
            string mensaje = "";
            DataTable dt = EjecutaComando(localPath, consulta, ref mensaje);   
            if (mensaje != "")
            {
                _logger.LogError(mensaje);
                return BadRequest(new { message = mensaje });
            }

            return Ok(dt);
        }
        // POST api/<EmpleadosController>
         [HttpPost]
        public async Task<IActionResult> SincronizarAsistencia (int idempresa)

        {

            string carpetaDestino = Path.Combine(_env.WebRootPath, "archivos");
            string ftpUrl = "";
            string localPath = "";
            string usuario = "Administrador";
            string contraseña = "sred%08";
            if (idempresa == 1)
            {   ftpUrl = ClsConfig.AccessDelta!;
                 localPath = carpetaDestino + "//Delta.mdb";
            }
            if (idempresa == 2)
            {
                 ftpUrl = ClsConfig.AccessPresco!;
                 localPath = carpetaDestino + "//Presco.mdb";
            }
            

#pragma warning disable CS0618
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpUrl);
#pragma warning restore CS0618
            request.Credentials = new NetworkCredential(usuario, contraseña);
            request.EnableSsl = false;
            request.UsePassive = true;
            request.UseBinary = true;
            request.Method = WebRequestMethods.Ftp.DownloadFile;

            // Permitir certificados no válidos (solo si confías en el servidor)
            ServicePointManager.ServerCertificateValidationCallback =
                (sender, certificate, chain, sslPolicyErrors) => true;


            if (!Directory.Exists(carpetaDestino))
            {
                Directory.CreateDirectory(carpetaDestino);
            }

            try
            {
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                using (Stream responseStream = response.GetResponseStream())
                using (FileStream outputStream = new FileStream(localPath, FileMode.Create))
                {
                    responseStream.CopyTo(outputStream);
                }

                return Ok("Descarga completada exitosamente.");
            }
            catch (WebException ex)
            {
                _logger.LogError("Error al conectar o listar archivos: {0}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}  