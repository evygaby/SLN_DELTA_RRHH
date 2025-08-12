using Api.Modelos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
namespace Api.Controllers

{
    [Route("/[controller]/[action]")]
    [ApiController]
    public class VariosReportesController:Controller
    {
        private readonly IDeltaContextProcedures _contextp;
       
        public VariosReportesController(IDeltaContextProcedures deltaContextProcedures)
        {

            _contextp = deltaContextProcedures;

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
        public async Task<IActionResult> ListaSecciones(string usu, string pass,int idempresa,string periodo)

        {
            TextoValor Lista = new TextoValor();
            return Ok(_contextp.CallProceduresConsula(Lista, "rh_mantenimientos.QRY_SECCIONES_X_EMP('" + periodo + "','" + idempresa +"',:1)", usu, pass));
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
            return Ok(_contextp.CallProceduresConsula(Lista, "prock_personal_web.QRY_CUMPLE_EMPLEADOS("+ idempresa + ",null,null,:1)", usu, pass));
       }

        [HttpGet]
        [Route("api/checklist")]
        public async Task<IActionResult> NumAlumnos(string usu, string pass, string periodo, [FromQuery] List<string> niveles,string paralelo,string tipoRep)

        {
            
            NumAlumnosxSeccion Lista = new NumAlumnosxSeccion();
            return Ok(_contextp.CallProceduresConsula(Lista, "PROC_K_REPORTES_ACADWEB.QRY_ReporteTotalAlumnas('" + periodo + "','"+ _contextp.SeccionesSeleccionadas( niveles) + "',null,'"+ tipoRep +"',:1)", usu, pass));
        }
        [HttpGet]
        public async Task<IActionResult> ConsultarActas(string usu, string pass, string periodo, DateTime? desde,DateTime? hasta,string? vfiltro,Int32 codemp)

        {
            var contextoOracle = new ModelOracleContext(); 
            DeltaContextProcedures obj = new DeltaContextProcedures(contextoOracle);
            ///formato fechas
            var fdesde = desde.HasValue ? "'" + desde.Value.ToString("dd/MM/yyyy") + "'" : "null";
            var fhasta = hasta.HasValue ? "'" + hasta.Value.ToString("dd/MM/yyyy") + "'" : "null";
            var ffiltro = ! string.IsNullOrWhiteSpace(vfiltro) ?  "'" + vfiltro + "'" : "null";
            

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
        public async Task<IActionResult> ActadeReunion(string usu, string pass,string periodo,int codigo)

        {
           // return Ok(_contextp.ConsultarActa(usu, pass,periodo,codigo));
            //await new ("miUsuario", "miPass", "2025-08", 123);
            var contextoOracle = new ModelOracleContext();
            DeltaContextProcedures obj = new DeltaContextProcedures(contextoOracle);
            DataSet dt = await obj.ConsultarActa(usu, pass,periodo,codigo);
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
    }

    public class OrdenDia
    {

        public Int16? SEC_DETRG { get; set; }
        public string? ORDENDIA { get; set; }
        public string? DESARROLLO { get; set; }
        public string? ACUERDOS { get; set; }
        public Int16? SEC_CABRG { get; set; }
    }
}
