using Api.Modelos;
using Api.Services.Interfaces;
using ExcelDataReader;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace Api.Controllers

{
    [Route("/[controller]/[action]")]
    [ApiController]
    public class VariosReportesController : Controller
    {
        private readonly IDeltaContextProcedures _contextp;
        private readonly IReportesService _reportesService;
        public VariosReportesController(IDeltaContextProcedures deltaContextProcedures, IReportesService reportesService)
        {

            _contextp = deltaContextProcedures;
            _reportesService = reportesService;
        }

        [HttpGet]
        public async Task<IActionResult> ListaPeriodo(string usu, string pass)

        {
            var contextoOracle = new ModelOracleContext();
            DeltaContextProcedures obj = new DeltaContextProcedures(contextoOracle);
            var sentencia = "PROC_K_ACAD_WEB.QRY_TodosPeriodos(get_pensum_presente,:1)";
            DataTable dt = obj.CallProceduresConsulaDT(sentencia, usu, pass);

            var lista = dt.AsEnumerable()
                     .Select(row => dt.Columns
                         .Cast<DataColumn>()
                         .ToDictionary(col => col.ColumnName, col => row[col]))
                     .OrderByDescending(dict => dict["PER_PERIODO"].ToString())
                     .ToList();
            return Json(lista);
        }
        [HttpGet]
        public async Task<IActionResult> ListaSecciones(string usu, string pass, int idempresa, string periodo)

        {
            TextoValor Lista = new TextoValor();
            return Ok(_contextp.CallProceduresConsula(Lista, "rh_mantenimientos.QRY_SECCIONES_X_EMP('" + periodo + "','" + idempresa + "',:1)", usu, pass));
        }
        [HttpGet]
        public async Task<IActionResult> Orlas(string usu, string pass, int idempresa)

        {
            OrlasPersonal Orla = new OrlasPersonal();
            return Ok(_contextp.CallProceduresConsula(Orla, "prock_personal_web.qry_empleadosxcc(null," + idempresa + ",:1)", usu, pass));
        }
        [HttpGet]
        public async Task<IActionResult> Cumpleanios(string usu, string pass, int idempresa)

        {
            CumpleaniosPersonal Lista = new CumpleaniosPersonal();
            return Ok(_contextp.CallProceduresConsula(Lista, "prock_personal_web.QRY_CUMPLE_EMPLEADOS(" + idempresa + ",null,null,:1)", usu, pass));
        }

        [HttpGet]
        [Route("api/checklist")]
        public async Task<IActionResult> NumAlumnos(string usu, string pass, string periodo, [FromQuery] List<string> niveles, string paralelo, string tipoRep)

        {

            NumAlumnosxSeccion Lista = new NumAlumnosxSeccion();
            return Ok(_contextp.CallProceduresConsula(Lista, "PROC_K_REPORTES_ACADWEB.QRY_ReporteTotalAlumnas('" + periodo + "','" + _contextp.SeccionesSeleccionadas(niveles) + "',null,'" + tipoRep + "',:1)", usu, pass));
        }
        [HttpGet]
        public async Task<IActionResult> ConsultaCapacitaciones(string usu, string pass, Int32 empresa, DateTime? desde, DateTime? hasta)

        {
            var contextoOracle = new ModelOracleContext();
            DeltaContextProcedures obj = new DeltaContextProcedures(contextoOracle);
            ///formato fechas
            var fdesde = desde.HasValue ? "'" + desde.Value.ToString("dd/MM/yyyy") + "'" : "null";
            var fhasta = hasta.HasValue ? "'" + hasta.Value.ToString("dd/MM/yyyy") + "'" : "null";
            var sentencia = "proc_k_rrhh_web.rpt_capacitaciones(" + empresa + "," + fdesde + "," + fhasta + ",:1)";
            DataTable dt = obj.CallProceduresConsulaDT(sentencia, usu, pass);
            var lista = dt.AsEnumerable()
                     .Select(row => dt.Columns
                         .Cast<DataColumn>()
                         .ToDictionary(col => col.ColumnName, col => row[col]))
                     .ToList();
            return Json(lista); ;
        }
        [HttpGet]
        public async Task<IActionResult> ConsultarActas(string usu, string pass, string periodo, DateTime? desde, DateTime? hasta, string? vfiltro, Int32 codemp)

        {
            var contextoOracle = new ModelOracleContext();
            DeltaContextProcedures obj = new DeltaContextProcedures(contextoOracle);
            ///formato fechas
            var fdesde = desde.HasValue ? "'" + desde.Value.ToString("dd/MM/yyyy") + "'" : "null";
            var fhasta = hasta.HasValue ? "'" + hasta.Value.ToString("dd/MM/yyyy") + "'" : "null";
            var ffiltro = !string.IsNullOrWhiteSpace(vfiltro) ? "'" + vfiltro + "'" : "null";


            var sentencia = "PROC_K_OTRASACTAS_WEB.QRY_Reporte_ActaRGP('" + periodo + "',12," + fdesde + "," + fhasta + "," + ffiltro + "," + codemp + ",'" + usu + "',:1)";
            DataTable dt = obj.CallProceduresConsulaDT(sentencia, usu, pass);
            foreach (DataRow i in dt.Rows)
            {
                if (!i.IsNull("ADJUNTO") && !i.IsNull("SEC_CABRG"))
                {
                    i["ADJUNTO"] = _contextp.ConvertirURL(i["ADJUNTO"].ToString(), int.Parse(i["SEC_CABRG"].ToString()));
                }
            }
            var lista = dt.AsEnumerable()
                     .Select(row => dt.Columns
                         .Cast<DataColumn>()
                         .ToDictionary(col => col.ColumnName, col => row[col]))
                     .ToList();
            return Json(lista); ;
        }
        [HttpGet]
        public async Task<IActionResult> ActadeReunion(string usu, string pass, string periodo, int codigo)

        {
            // return Ok(_contextp.ConsultarActa(usu, pass,periodo,codigo));
            //await new ("miUsuario", "miPass", "2025-08", 123);
            var contextoOracle = new ModelOracleContext();
            DeltaContextProcedures obj = new DeltaContextProcedures(contextoOracle);
            DataSet dt = await obj.ConsultarActa(usu, pass, periodo, codigo);
            List<object> listaDeListas = new List<object>();
            var Cabecera = dt.Tables[0].AsEnumerable()
                     .Select(row => dt.Tables[0].Columns
                         .Cast<DataColumn>()
                         .ToDictionary(col => col.ColumnName, col => row[col]))
                     .ToList();
            listaDeListas.Add(Cabecera);
            var Asistentes = dt.Tables[1].AsEnumerable()
                     .Select(row => dt.Tables[1].Columns
                         .Cast<DataColumn>()
                         .ToDictionary(col => col.ColumnName, col => row[col]))
                     .ToList();
            listaDeListas.Add(Asistentes);
            var OrdenDia = dt.Tables[2].AsEnumerable()
                     .Select(row => dt.Tables[2].Columns
                         .Cast<DataColumn>()
                         .ToDictionary(col => col.ColumnName, col => row[col]))
                     .ToList();
            listaDeListas.Add(OrdenDia);


            var Adjuntos = dt.Tables[3].AsEnumerable()
                     .Select(row => dt.Tables[3].Columns
                         .Cast<DataColumn>()
                         .ToDictionary(col => col.ColumnName, col => row[col]))
                     .ToList();
            listaDeListas.Add(Adjuntos);
            return Json(listaDeListas);
        }
        [HttpGet]
        public async Task<IActionResult> DatosExcel(string usu, string pass, Int32 empresa)

        {
            var contextoOracle = new ModelOracleContext();
            DeltaContextProcedures obj = new DeltaContextProcedures(contextoOracle);
            var sentencia = "prock_personal_web.QRY_DATOS_EMPLEADO(" + empresa + ",:1)";
            DataTable dt = obj.CallProceduresConsulaDT(sentencia, usu, pass);
            var lista = dt.AsEnumerable()
                     .Select(row => dt.Columns
                         .Cast<DataColumn>()
                         .ToDictionary(col => col.ColumnName, col => row[col]))
                     .ToList();
            return Json(lista); ;
        }
        [HttpGet]
        public async Task<IActionResult> JefasArea(string usu, string pass, Int32 empresa)

        {
            var contextoOracle = new ModelOracleContext();
            DeltaContextProcedures obj = new DeltaContextProcedures(contextoOracle);
            var sentencia = "PROC_K_RRHH_WEB.QRY_JefesArea(" + empresa + ",:1)";
            DataTable dt = obj.CallProceduresConsulaDT(sentencia, usu, pass);
            var lista = dt.AsEnumerable()
                     .Select(row => dt.Columns
                         .Cast<DataColumn>()
                         .ToDictionary(col => col.ColumnName, col => row[col]))
                     .ToList();
            return Json(lista);
        }
        [HttpGet]
        [Route("api/checklist")]
        public async Task<IActionResult> ListadoPEGsxSeccion(string usu, string pass, string periodo, [FromQuery] List<string> niveles)
        {
            DataTable dt = await _reportesService.ListadoPEGsxSeccion(usu, pass, periodo, niveles);
            var lista = dt.AsEnumerable()
                     .Select(row => dt.Columns
                         .Cast<DataColumn>()
                         .ToDictionary(col => col.ColumnName, col => row[col]))
                     .ToList();
            return Json(lista);
        }
        [HttpGet]
        [Route("api/checklist")]
        public async Task<IActionResult> ListadoPreceptorasxSeccion(string usu, string pass, string periodo, [FromQuery] List<string> niveles)

        {
            DataTable dt = await _reportesService.ListadoPreceptorasxSeccion(usu, pass, periodo, niveles);
            var lista = dt.AsEnumerable()
                     .Select(row => dt.Columns
                         .Cast<DataColumn>()
                         .ToDictionary(col => col.ColumnName, col => row[col]))
                     .ToList();
            return Json(lista);
        }
        [HttpGet]
        public async Task<IActionResult> ComparativoRoles(string usu, string pass, Int32 empresa, DateTime? desde, DateTime? hasta)
        {
            var fdesde = desde.HasValue ? "'" + desde.Value.ToString("dd/MM/yyyy") + "'" : "null";
            var fhasta = hasta.HasValue ? "'" + hasta.Value.ToString("dd/MM/yyyy") + "'" : "null";
            var contextoOracle = new ModelOracleContext();
            DeltaContextProcedures obj = new DeltaContextProcedures(contextoOracle);
            var sentencia = "DEVELOPER1.prock_personal_web.arcComparaExcel(" + empresa + "," + fdesde + "," + fhasta + ",:1)";
            DataTable dt = obj.CallProceduresConsulaDT(sentencia, usu, pass);
            var lista = dt.AsEnumerable()
                     .Select(row => dt.Columns
                         .Cast<DataColumn>()
                         .ToDictionary(col => col.ColumnName, col => row[col]))
                     .ToList();
            return Json(lista);
        }
        [HttpGet]
        public async Task<IActionResult> ListaActualizaDatos(string usu, string pass, Int32 empresa)
        {
            var contextoOracle = new ModelOracleContext();
            DeltaContextProcedures obj = new DeltaContextProcedures(contextoOracle);
            var sentencia = "DEVELOPER1.prock_personal_web.QRY_LISTAACTUALIZADATOS(" + empresa + ",:1)";
            DataTable dt = obj.CallProceduresConsulaDT(sentencia, usu, pass);
            dt.Columns.Add("CODIGO");
            dt.Columns.Add("URL");
            foreach (DataRow i in dt.Rows)
            {
                i["CODIGO"] = _reportesService.Encrypt(i["CODEMP"] + "|" + i["ID_EMPRESA"],"d3lt@_act_emp_2024");
                i["URL"] = "https://actualizaciondatos.uedelta.k12.ec/wfrmfichasocial.aspx?a=" + i["CODIGO"];
            }
            var lista = dt.AsEnumerable()
                     .Select(row => dt.Columns
                         .Cast<DataColumn>()
                         .ToDictionary(col => col.ColumnName, col => row[col]))
                     .ToList();
            return Json(lista);
        }
        [HttpGet]
        public async Task<IActionResult> ListaTitulos(string usu, string pass, Int32 empresa)
        {
            var contextoOracle = new ModelOracleContext();
            DeltaContextProcedures obj = new DeltaContextProcedures(contextoOracle);
            var sentencia = "DEVELOPER1.PROC_K_RRHH_WEB.QRY_LISTA_TITULOS(" + empresa + ",:1)";
            DataTable dt = obj.CallProceduresConsulaDT(sentencia, usu, pass);
            var lista = dt.AsEnumerable()
                     .Select(row => dt.Columns
                         .Cast<DataColumn>()
                         .ToDictionary(col => col.ColumnName, col => row[col]))
                     .ToList();
            return Json(lista);
        }
        [HttpGet]
        public async Task<IActionResult> ListaEncargos(string usu, string pass, Int32 empresa)
        {
            var contextoOracle = new ModelOracleContext();
            DeltaContextProcedures obj = new DeltaContextProcedures(contextoOracle);
            var sentencia = "DEVELOPER1.PROC_K_RRHH_WEB.RPT_LISTA_ENCARGOS(" + empresa + ",:1)";
            DataTable dt = obj.CallProceduresConsulaDT(sentencia, usu, pass);
            var lista = dt.AsEnumerable()
                     .Select(row => dt.Columns
                         .Cast<DataColumn>()
                         .ToDictionary(col => col.ColumnName, col => row[col]))
                     .ToList();
            return Json(lista);
        }
        [HttpGet]
        [Route("api/checklist")]
        public async Task<IActionResult> DistributivoMaestras(string usu, string pass,string periodo,  [FromQuery] List<string> niveles)
        {
            var contextoOracle = new ModelOracleContext();
            DeltaContextProcedures obj = new DeltaContextProcedures(contextoOracle);
            var sentencia = "PROC_K_REPORTES_ACADWEB.QRYDistribxMaestra('" + periodo + "','" + _contextp.SeccionesSeleccionadas(niveles) + "',NULL,:1)";
            DataTable dt = obj.CallProceduresConsulaDT(sentencia, usu, pass);
            dt.Columns.Add("HORASCLASE_NUM", typeof(int));
            foreach (DataRow i in dt.Rows)
            {
                if (!Information.IsDBNull(i["PROF"]))
                {
                    object result = dt.Compute("Sum(NUMHORAS)", $"PROF = '{i["PROF"]}'");
                    int sum = result != DBNull.Value ? Convert.ToInt32(result) : 0;
                    i["HORASCLASE_NUM"] = sum;
                    i["CARGOS"] = i["CARGOS"] == DBNull.Value? "": i["CARGOS"].ToString().Replace("        ", "<br>");
                }
            }


            var lista = dt.AsEnumerable()
                     .Select(row => dt.Columns
                         .Cast<DataColumn>()
                         .ToDictionary(col => col.ColumnName, col => row[col]))
                     .ToList();
            return Json(lista);
        }

        [HttpPost]
        public IActionResult CompareData(IFormFile file,  string usu,string pass,DateTime? Fecha,Int32 empresa)
        {
            DataTable excelData = new DataTable();
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (var ms = new MemoryStream())
            {
                 file.CopyTo(ms);
                ms.Seek(0, SeekOrigin.Begin);


                using (var reader = ExcelReaderFactory.CreateReader(ms))
                {
                    var conf = new ExcelDataSetConfiguration
                    {
                        ConfigureDataTable = _ => new ExcelDataTableConfiguration
                        {
                            UseHeaderRow = false
                        }
                    };
                    // Read the Excel w  ss  
                    var dataSet = reader.AsDataSet(conf);

                    // Agregar columnas
                    for (int j = 0; j <= 8 ; j++)
                    {
                        excelData!.Columns.Add(dataSet.Tables[0].Rows[1][j].ToString());
                    }

                    for (int i = 2; i < dataSet.Tables[0].Rows.Count; i++)
                    {
                        if (dataSet.Tables[0].Rows[i][3].ToString()!= ""){ 
                        DataRow dr = excelData.NewRow();
                        for (int j = 0; j <= 8 ; j++)
                        {
                            dr[j - 0] = dataSet.Tables[0].Rows[i][j];
                        }
                        excelData.Rows.Add(dr);
                        }
                    }

                }
            }



            //var excelData = request.ExcelData; // Array de objetos desde Angular
            string Busq = "";
            var fdesde = Fecha.HasValue ?  Fecha.Value.ToString("MM-yyyy") : "null";
            if (Fecha.HasValue)
            {
                Busq = string.Join(";", excelData.AsEnumerable().Select(r => $"{fdesde}|{r["Cédula"]}"));
            }
            else
            {
                StringBuilder buscar = new StringBuilder();
                foreach (DataRow row in excelData.Rows)
                {
                    string[] parts = row["Periodo"].ToString().Split('-');
                    if (parts.Length < 2) continue; // seguridad

                    int year = int.Parse(parts[0]);
                    int month = int.Parse(parts[1]);

                    // Aseguramos que el mes tenga 2 dígitos
                    string periodo = $"{month:D2}-{year}";

                    buscar.Append(periodo)
                          .Append("|")
                          .Append(row["Cédula"])
                          .Append(";");
                }
                Busq= buscar.ToString().TrimEnd(';');
            }

            //Datos desde Oracle
            var contextoOracle = new ModelOracleContext();
            DeltaContextProcedures obj = new DeltaContextProcedures(contextoOracle);
            var sentencia = "developer1.PROCK_PERSONAL_WEB.QRY_COMPARA_IESS('" + Busq + "',"+ empresa +",:1)";
            DataTable dt = obj.CallProceduresConsulaDT(sentencia, usu, pass);

            //Comparar los datos
            foreach (DataRow i in dt.Rows)
            {
                string[] periodoParts = i["PERIODO"].ToString().Split('-');
                string mes = periodoParts[0];
                string anio = periodoParts[1];
                // Quitar cero inicial si mes < 10
                if (int.TryParse(mes, out int mesInt) && mesInt < 10)
                {
                    mes = mesInt.ToString();
                }
                string periodo = anio + "-" + mes;
                // Buscar fila correspondiente en dt usando NUMCEDULA
                DataRow dr = excelData.AsEnumerable().FirstOrDefault(r => r.Field<string>("Cédula") == i.Field<string>("NUMCEDULA"));
                if (dr != null)
                {
                    i["SUELDO_IESS"] = dr["Sueldo"];
                    i["DIAS_IESS"] = dr["Días"];
                    // Marcar diferencias
                    if (!i["SUELDO"].Equals(i["SUELDO_IESS"]))
                    {
                        i["DIF_SUELDO"] = 1;
                    }
                    if (!i["DIAS"].Equals(i["DIAS_IESS"]))
                    {
                        i["DIF_DIAS"] = 1;
                    }
                }
            }

            //Agregar registros que están en Oracle pero no en Excel
            foreach (DataRow i in excelData.Rows)
            {
                int x = 0;
                string cedula = i["Cédula"].ToString();
                string[] partes = i["Periodo"].ToString().Split('-');

                // Convierte la primera parte a int y la formatea con 2 dígitos
                string parte1 = int.Parse(partes[1]).ToString("00");
                string parte2 = partes[0]; // tal cual

                string period = parte1 + "-" + parte2;
                DataRow dr = dt.AsEnumerable()
                    .FirstOrDefault(r => r.Field<string>("NUMCEDULA") == cedula
                                      && r.Field<string>("PERIODO") == period);

                if (dr == null)
                {
                    DataRow dw = dt.NewRow();
                    dw["CODEMP"] = x;
                    dw["PERIODO"] = period;
                    dw["NUMCEDULA"] = i["Cédula"];
                    dw["RAZONSOCIAL"] = i["Nombre"];
                    dw["SUELDO"] = DBNull.Value;
                    dw["DIAS"] = DBNull.Value;
                    dw["SUELDO_IESS"] = i["Sueldo"];
                    dw["DIAS_IESS"] = i["Días"];
                    dw["DIF_SUELDO"] = 1;
                    dw["DIF_DIAS"] = 1;

                    dt.Rows.Add(dw);
                }
            }
            // 1️⃣ Crear DataTable de periodos
            DataTable dtPeriodo = new DataTable();
            dtPeriodo.Columns.Add("Periodo", typeof(string));
            dtPeriodo.Columns.Add("Periodoc", typeof(long));
            String fecha1 = "";
            String fecha2 ="";
            if (Fecha.HasValue)
            {
                DateTime fechab = (DateTime)Fecha; // suponiendo que dtCompara es un DateTimePicker
                fecha1 = new DateTime(fechab.Year, fechab.Month, 1).ToString("dd/MM/yyyy");
                fecha2 = new DateTime(fechab.Year, fechab.Month + 1, 1).ToString("dd/MM/yyyy");
            }
            else
            {
                foreach (DataRow i in excelData.Rows)
                {
                    DataRow dr = dtPeriodo.NewRow();
                    dr["Periodo"] = i["Periodo"];
                    dr["Periodoc"] = i["Periodo"].ToString().Replace("-", "");
                    dtPeriodo.Rows.Add(dr);
                }
                // Obtener periodo mínimo y máximo
                string minPeriodoc = dtPeriodo.Compute("MIN(Periodoc)", string.Empty).ToString();
                string maxPeriodoc = dtPeriodo.Compute("MAX(Periodoc)", string.Empty).ToString();

                DataRow rowMin = dtPeriodo.Select($"Periodoc='{minPeriodoc}'").FirstOrDefault();
                DataRow rowMax = dtPeriodo.Select($"Periodoc='{maxPeriodoc}'").FirstOrDefault();

                if (rowMin != null)
                {
                    string[] parts = rowMin[0].ToString().Split('-');
                    int year = int.Parse(parts[0]);
                    int month = int.Parse(parts[1]);
                    fecha1 = new DateTime(year, month, 1).ToString("dd/MM/yyyy");
                }

                if (rowMax != null)
                {
                    string[] parts = rowMax[0].ToString().Split('-');
                    int year = int.Parse(parts[0]);
                    int month = int.Parse(parts[1]);
                    fecha2 = new DateTime(year, month, 1).ToString("dd/MM/yyyy");
                }
            }
            // 3️⃣ Armar cadena de cédulas
            string busq = "";
            foreach (DataRow i in excelData.Rows)
            {
                busq += i["Cédula"].ToString() + ";";
            }
            busq = busq.TrimEnd(';');

            //Datos desde Oracle1
             contextoOracle = new ModelOracleContext();
             obj = new DeltaContextProcedures(contextoOracle);
             sentencia = "developer1.PROCK_PERSONAL_WEB.QRY_COMPARA_IESS_EXCEL('" + fecha1 + "','" + fecha2 + "','" + busq + "'," + empresa + ",:1)";
            DataTable dtBase = obj.CallProceduresConsulaDT(sentencia, usu, pass);

            foreach (DataRow i in dtBase.Rows)
            {
                DataRow dw = dt.NewRow();
                dw["CODEMP"] = i["CODEMP"];
                dw["PERIODO"] = i["PERIODO"];
                dw["NUMCEDULA"] = i["NUMCEDULA"];
                dw["RAZONSOCIAL"] = i["RAZONSOCIAL"];
                dw["SUELDO"] = i["SUELDO"];
                dw["DIAS"] = i["DIAS"];
                dw["SUELDO_IESS"] = DBNull.Value;
                dw["DIAS_IESS"] = DBNull.Value;
                dw["DIF_SUELDO"] = 1;
                dw["DIF_DIAS"] = 1;

                dt.Rows.Add(dw);
            }

            var lista = dt.AsEnumerable()
                     .Select(row => dt.Columns
                         .Cast<DataColumn>()
                         .ToDictionary(col => col.ColumnName, col => row[col]))
                     .ToList();
            return Json(lista);

            
        }
        
    }


}
