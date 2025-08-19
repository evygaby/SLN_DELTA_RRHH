using Microsoft.AspNetCore.Mvc;
using Microsoft.Reporting.NETCore;
using System.IO;
using Microsoft.AspNetCore.Hosting;
namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImpresionReportes(IWebHostEnvironment env) : ControllerBase
    {
        private readonly IWebHostEnvironment _env = env;

        [HttpGet("rdlc")]
        public IActionResult GenerarReporte(string periodo)
        {
            // Ruta al archivo RDLC en la carpeta "Reportes"
            string path = Path.Combine(_env.ContentRootPath, "Reportes", "rptPegPrecep.rdlc");

            if (!System.IO.File.Exists(path))
                return NotFound($"No se encontró el archivo RDLC en {path}");

            // Crear reporte
            LocalReport report = new LocalReport();
            report.LoadReportDefinition(System.IO.File.OpenRead(path));

            // Datos de ejemplo
            var datos = new[]
            {
                new { Id = 1, Nombre = "Evelyn", Periodo = periodo },
                new { Id = 2, Nombre = "Carlos", Periodo = periodo }
            };

            // Debe coincidir con el nombre del DataSet definido en el RDLC
            report.DataSources.Add(new ReportDataSource("MiDataSet", datos));

            // Exportar a PDF
            byte[] pdf = report.Render("PDF");

            // Devolver archivo PDF al navegador
            return File(pdf, "application/pdf", $"Reporte_{periodo}.pdf");
        }
    }
}
