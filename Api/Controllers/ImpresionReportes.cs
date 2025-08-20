using Api.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Reporting.NETCore;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System.Data;
using System.IO;
using System.Threading.Tasks;
namespace Api.Controllers
{
    [ApiController]
    [Route("/[controller]/[action]")]
    public class ImpresionReportes: ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly IReportesService _reportesService;
        public ImpresionReportes(IWebHostEnvironment env, IReportesService reportesService)
        {
            _env = env;
            _reportesService = reportesService;
        }

        
        [HttpGet("checklist")]
        public async Task<IActionResult> rptPegPreceptora(string usu, string pass, string periodo, [FromQuery] List<string> niveles,Int32 idEmpresa,string mostrarPeg,string mostrarPreceptora)
        {
            // Ruta al archivo RDLC en la carpeta "Reportes"
            string path = Path.Combine(_env.ContentRootPath, "Reportes", "rptPegPrecept.rdlc");

            if (!System.IO.File.Exists(path))
                return NotFound($"No se encontró el archivo RDLC en {path}");

            // Crear reporte
            LocalReport report = new LocalReport();
            report.LoadReportDefinition(System.IO.File.OpenRead(path));

            // Datos de ejemplo
            DataTable pegs  = await _reportesService.ListadoPEGsxSeccion(usu, pass, periodo, niveles);
            DataTable preceptoras = await  _reportesService.ListadoPreceptorasxSeccion(usu, pass, periodo, niveles);
            //parametros
            var reportParams = new List<ReportParameter>
               {
                new ReportParameter("fecha", DateTime.Now.ToString()),
                new ReportParameter("periodo", periodo),
                new ReportParameter("usua", usu),
                new ReportParameter("muestrapeg", "S"),
                new ReportParameter("muestraprecep", "S"),
                new ReportParameter("idEmpresa",idEmpresa == 3 ? "D":"P")
                };

            report.SetParameters(reportParams);
            // Debe coincidir con el nombre del DataSet definido en el RDLC
            report.DataSources.Add(new ReportDataSource("ds_delta", pegs));
            report.DataSources.Add(new ReportDataSource("ds_precep", preceptoras));

            // Exportar a PDF
            byte[] pdf = report.Render("EXCELOPENXML");

            // Devolver archivo PDF al navegador
            return File(pdf, "application/pdf", $"LstPegPrecep_" + DateTime.Now.ToString("ddMMyyyy") + ".xlsx");
        }


        [HttpGet]
        public async Task<IActionResult> rptPegPreceptora23(string usu, string pass, string periodo, [FromQuery] List<string> niveles, Int32 idEmpresa)
        {
            // Ruta al archivo RDLC en la carpeta "Reportes"
            string path = Path.Combine(_env.ContentRootPath, "Reportes", "rptPegPrecep.rdlc");

            if (!System.IO.File.Exists(path))
                return NotFound($"No se encontró el archivo RDLC en {path}");

            // Crear reporte
            LocalReport report = new LocalReport();
            report.LoadReportDefinition(System.IO.File.OpenRead(path));

            // Datos de ejemplo
            DataTable pegs = await _reportesService.ListadoPEGsxSeccion(usu, pass, periodo, niveles);
            DataTable preceptoras = await _reportesService.ListadoPreceptorasxSeccion(usu, pass, periodo, niveles);
            //parametros
            var reportParams = new List<ReportParameter>
               {
                new ReportParameter("fecha", DateTime.Now.ToString()),
                new ReportParameter("periodo", periodo),
                new ReportParameter("usua", usu),
                new ReportParameter("muestrapeg", "S"),
                new ReportParameter("muestraprecep", "S"),
                new ReportParameter("idEmpresa",idEmpresa == 3 ? "D":"P")
                };

            report.SetParameters(reportParams);
            // Debe coincidir con el nombre del DataSet definido en el RDLC
            report.DataSources.Add(new ReportDataSource("ds_delta", pegs));
            report.DataSources.Add(new ReportDataSource("ds_precep", preceptoras));

            // Exportar a PDF
            byte[] pdf = report.Render("PDF");

            // Devolver archivo PDF al navegador
            return File(pdf, "application/pdf", $"LstPegPrecep_" + DateTime.Now.ToString("ddMMyyyy") + ".pdf");
        }
    }
}
