using Api.Modelos;
using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Data;
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
            PeriodoLectivo Lista = new PeriodoLectivo();
            return Ok(_contextp.CallProceduresConsula(Lista, "PROC_K_ACAD_WEB.QRY_TodosPeriodos(get_pensum_presente,:1)", usu, pass));
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
            return Ok(_contextp.CallProceduresConsula(Orla, "prock_personal_web.qry_empleadosxcc(null,"+ idempresa + ",:1)", usu, pass));
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
            return Ok(_contextp.CallProceduresConsula(Lista, "PROC_K_REPORTES_ACADWEB.QRY_ReporteTotalAlumnas('" + periodo + "','"+ _contextp.SeccionesSeleccionadas( niveles) + "','"+ paralelo +"','"+ tipoRep +"',:1)", usu, pass));
        }
        
    }


}
