using Api.Modelos;
using Api.Services.Interfaces;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Reporting.NETCore;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System.Data;
using System.IO;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
namespace Api.Controllers
{
    [ApiController]
    [Route("/[controller]/[action]")]
    public class ImpresionReportes : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly IReportesService _reportesService;
        public ImpresionReportes(IWebHostEnvironment env, IReportesService reportesService)
        {
            _env = env;
            _reportesService = reportesService;
        }


        [HttpGet("checklist")]
        public async Task<IActionResult> rptPegPreceptora(string usu, string pass, string periodo, [FromQuery] List<string> niveles, Int32 idEmpresa, string mostrarPeg, string mostrarPreceptora)
        {
            // Ruta al archivo RDLC en la carpeta "Reportes"
            string path = Path.Combine(_env.ContentRootPath, "Reportes", "rptPegPrecept.rdlc");

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
            byte[] pdf = report.Render("EXCELOPENXML");

            // Devolver archivo PDF al navegador
            return File(pdf, "application/pdf", $"LstPegPrecep_" + DateTime.Now.ToString("ddMMyyyy") + ".xlsx");
        }


        [HttpGet]
        public async Task<IActionResult> DocActualizaDatos(string usu, string pass, Int32 codigo)
        {
            // Ruta al archivo RDLC en la carpeta "Reportes"
            string path = Path.Combine(_env.ContentRootPath, "Reportes", "rptActualizaFicha.rdlc");

            if (!System.IO.File.Exists(path))
                return NotFound($"No se encontró el archivo RDLC en {path}");

            // Crear reporte
            LocalReport report = new LocalReport();
            report.LoadReportDefinition(System.IO.File.OpenRead(path));

            // Datos de ejemplo
            System.Data.DataSet ds = await _reportesService.ActualizaDatosEmpleado(usu, pass, codigo);
            // Debe coincidir con el nombre del DataSet definido en el RDLC
            report.DataSources.Add(new ReportDataSource("dsEmpleado", ds.Tables[0]));
            report.DataSources.Add(new ReportDataSource("dsTitulos", ds.Tables[1]));
            report.DataSources.Add(new ReportDataSource("dsFamiliares", ds.Tables[2]));
            report.DataSources.Add(new ReportDataSource("dsDiscapa", ds.Tables[3]));
            report.DataSources.Add(new ReportDataSource("dsEnfermedad", ds.Tables[4]));

            // Exportar a PDF
            byte[] pdf = report.Render("PDF");

            // Devolver archivo PDF al navegador
            return File(pdf, "application/pdf", $"Actualizadatos_" + codigo + DateTime.Now.ToString("ddMMyyyy") + ".pdf");
        }
        [HttpGet]
        public async Task<IActionResult> Prestamos(string usu, string pass, Int32 empresa, Int32? saldo, DateTime? desde, DateTime? hasta)
        {
            // Ruta al archivo RDLC en la carpeta "Reportes"
            string path = Path.Combine(_env.ContentRootPath, "Reportes", "rptPrestamos.rdlc");

            if (!System.IO.File.Exists(path))
                return NotFound($"No se encontró el archivo RDLC en {path}");

            // Crear reporte
            LocalReport report = new LocalReport();
            report.LoadReportDefinition(System.IO.File.OpenRead(path));
            //parametros
            var reportParams = new List<ReportParameter>
               {
                new ReportParameter("desde", desde.HasValue ?  desde.Value.ToString("dd/MM/yyyy") : ""),
                new ReportParameter("hasta",hasta.HasValue ?  hasta.Value.ToString("dd/MM/yyyy") : ""),
                new ReportParameter("empresa", empresa.ToString()),
                new ReportParameter("saldo", saldo.ToString())
                };
            report.SetParameters(reportParams);
            // Datos de ejemplo
            System.Data.DataTable dt = await _reportesService.Prestamos(usu, pass, empresa, saldo, desde, hasta);
            // Debe coincidir con el nombre del DataSet definido en el RDLC
            report.DataSources.Add(new ReportDataSource("ds_prestamos", dt));

            // Exportar a PDF
            byte[] pdf = report.Render("PDF");

            // Devolver archivo PDF al navegador
            return File(pdf, "application/pdf", $"Prestamos_" + DateTime.Now.ToString("ddMMyyyy") + ".pdf");
        }
        [HttpGet]
        public async Task<IActionResult> CrearListaPDF(string usu, string pass, Int32 empresa, DateTime fecha, string ccosto, string trol)
        {
            System.Data.DataTable filas = await _reportesService.ListaRol(usu, pass, empresa, fecha, ccosto, trol);

            using (var ms = new MemoryStream())
            {
                Document doc = new Document();
                PdfWriter writer = PdfWriter.GetInstance(doc, ms);
                doc.Open();

                foreach (DataRow j in filas.Rows)
                {

                    int key = Convert.ToInt32(j["CODEMP"]);
                    System.Data.DataTable dt = await _reportesService.RolIndividual(usu, pass, empresa, fecha, key);

                    if (dt.Rows.Count > 0)
                    {
                        try
                        {
                            string imagePath = "";
                            if (empresa == 3)
                                imagePath = Path.Combine(_env.WebRootPath, "Images", "logo_solo.png");
                            else
                                imagePath = Path.Combine(_env.WebRootPath, "images", "logo_prescolar.jpg");
                            Image img = Image.GetInstance(imagePath);
                            img.ScaleToFit(75f, 75f);
                            doc.Add(img);

                            float alturalinea = 800f;
                            PdfContentByte cb = writer.DirectContent;

                            BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                            BaseFont bfn = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                            iTextSharp.text.Font boldFont = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.BOLD);

                            cb.SetFontAndSize(bf, 12);
                            cb.BeginText();
                            alturalinea -= 20;
                            cb.ShowTextAligned(Element.ALIGN_CENTER, "COPECE", 300, alturalinea, 0);
                            alturalinea -= 20;
                            if (empresa == 3)
                                cb.ShowTextAligned(Element.ALIGN_CENTER, "Unidad Educativa Bilingüe Delta", 300, alturalinea, 0);
                            else
                                cb.ShowTextAligned(Element.ALIGN_CENTER, "Presco DeltaTorremar", 300, alturalinea, 0);

                            string texto = "ROL DE PAGOS " + dt.Rows[0]["TIPO_ROL"].ToString();
                            alturalinea -= 30;
                            cb.ShowTextAligned(Element.ALIGN_CENTER, texto, 300, alturalinea, 0);
                            cb.EndText();

                            cb.SetFontAndSize(bf, 12);
                            cb.BeginText();
                            alturalinea -= 20;
                            cb.ShowTextAligned(Element.ALIGN_LEFT, "Nombre:", 40, alturalinea, 0);
                            cb.ShowTextAligned(Element.ALIGN_LEFT, dt.Rows[0]["RAZONSOCIAL"].ToString(), 100, alturalinea, 0);
                            alturalinea -= 20;
                            cb.ShowTextAligned(Element.ALIGN_LEFT, "Mes:", 40, alturalinea, 0);
                            cb.SetFontAndSize(bfn, 12);
                            cb.ShowTextAligned(Element.ALIGN_LEFT, dt.Rows[0]["MES"].ToString(), 100, alturalinea, 0);
                            cb.EndText();

                            alturalinea -= 10;
                            cb.MoveTo(30f, alturalinea);
                            cb.LineTo(560f, alturalinea);
                            cb.Stroke();
                            alturalinea -= 10;
                            // INGRESOS
                            int ingresos = dt.Select("TIPO='Ingresos'", "COL, ORD,ORDEN,RUBRO").Length;
                            if (ingresos > 0)
                            {
                                DataTable dtIngresos = dt.Select("TIPO='Ingresos'", "COL, ORD,ORDEN,RUBRO").CopyToDataTable();
                                PdfPTable table = new PdfPTable(3);
                                table.TotalWidth = 250f;
                                PdfPCell header = new PdfPCell(new Phrase("INGRESOS", boldFont))
                                {
                                    Colspan = 3,
                                    HorizontalAlignment = Element.ALIGN_CENTER,
                                    FixedHeight = 20
                                };
                                table.AddCell(header);

                                foreach (DataRow i in dtIngresos.Rows)
                                {
                                    PdfPCell nombreCompleto = new PdfPCell(new Phrase(i["RUBRO"].ToString()))
                                    {
                                        Colspan = 2,
                                        HorizontalAlignment = Element.ALIGN_LEFT,
                                        FixedHeight = 17
                                    };
                                    table.AddCell(nombreCompleto);

                                    string s = string.Format("{0:N2}", i["MONTO"]);
                                    PdfPCell monto = new PdfPCell(new Phrase(s))
                                    {
                                        HorizontalAlignment = Element.ALIGN_RIGHT,
                                        FixedHeight = 17
                                    };
                                    table.AddCell(monto);
                                }

                                PdfPCell total = new PdfPCell(new Phrase("Ingreso Mensual", boldFont))
                                {
                                    Colspan = 2,
                                    HorizontalAlignment = Element.ALIGN_LEFT,
                                    FixedHeight = 20
                                };
                                table.AddCell(total);

                                string mt = string.Format("{0:N2}", dtIngresos.Compute("Sum(MONTO)", ""));
                                PdfPCell montot = new PdfPCell(new Phrase(mt, boldFont))
                                {
                                    HorizontalAlignment = Element.ALIGN_RIGHT
                                };
                                table.AddCell(montot);

                                table.WriteSelectedRows(0, -1, 30f, alturalinea, cb);
                            }

                            // EGRESOS
                            int egresos = dt.Select("TIPO='Egresos'", "COL, ORD,ORDEN,RUBRO").Length;
                            if (egresos > 0)
                            {
                                DataTable dtEgresos = dt.Select("TIPO='Egresos'", "COL, ORD,ORDEN,RUBRO").CopyToDataTable();
                                PdfPTable table = new PdfPTable(3);
                                table.TotalWidth = 250f;
                                PdfPCell header = new PdfPCell(new Phrase("EGRESOS", boldFont))
                                {
                                    Colspan = 3,
                                    HorizontalAlignment = Element.ALIGN_CENTER,
                                    FixedHeight = 20
                                };
                                table.AddCell(header);

                                foreach (DataRow i in dtEgresos.Rows)
                                {
                                    PdfPCell nombreCompleto = new PdfPCell(new Phrase(i["RUBRO"].ToString()))
                                    {
                                        Colspan = 2,
                                        HorizontalAlignment = Element.ALIGN_LEFT,
                                        FixedHeight = 17
                                    };
                                    table.AddCell(nombreCompleto);

                                    string s = string.Format("{0:N2}", i["MONTO"]);
                                    PdfPCell monto = new PdfPCell(new Phrase(s))
                                    {
                                        HorizontalAlignment = Element.ALIGN_RIGHT
                                    };
                                    table.AddCell(monto);
                                }

                                PdfPCell total = new PdfPCell(new Phrase("Egreso Mensual", boldFont))
                                {
                                    Colspan = 2,
                                    HorizontalAlignment = Element.ALIGN_LEFT,
                                    FixedHeight = 20
                                };
                                table.AddCell(total);

                                string mt = string.Format("{0:N2}", dtEgresos.Compute("Sum(MONTO)", ""));
                                PdfPCell montot = new PdfPCell(new Phrase(mt, boldFont))
                                {
                                    HorizontalAlignment = Element.ALIGN_RIGHT
                                };
                                table.AddCell(montot);

                                table.WriteSelectedRows(0, -1, 310f, alturalinea, cb);
                            }
                            //Altura de la tabla
                            Single altura_tabla;
                            if (ingresos > egresos)
                            {
                                altura_tabla = (ingresos * 17) + 50;
                                if ((ingresos - egresos) <= 3)
                                    altura_tabla += 30;
                            }
                            else
                            {
                                altura_tabla = (egresos * 17) + 50;
                                if ((ingresos - egresos) <= 3)
                                    altura_tabla += 30;
                            }
                            alturalinea -= altura_tabla;
                            // NETO A PAGAR
                            PdfPTable netoTable = new PdfPTable(3);
                            netoTable.TotalWidth = 250f;
                            PdfPCell neto = new PdfPCell(new Phrase("NETO A RECIBIR", boldFont))
                            {
                                Colspan = 2,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                            netoTable.AddCell(neto);

                            string apagar = string.Format("{0:N2}", dt.Rows[0]["A_PAGAR"]);
                            PdfPCell montoPagar = new PdfPCell(new Phrase(apagar, boldFont))
                            {
                                HorizontalAlignment = Element.ALIGN_RIGHT
                            };
                            netoTable.AddCell(montoPagar);

                            netoTable.WriteSelectedRows(0, -1, 310f, alturalinea, cb);

                            // Firma
                            PdfPTable firma = new PdfPTable(1);
                            firma.TotalWidth = 150f;
                            PdfPCell recibi = new PdfPCell(new Phrase("Recibí Conforme", boldFont))
                            {
                                HorizontalAlignment = Element.ALIGN_CENTER,
                                BorderWidthLeft = 0,
                                BorderWidthRight = 0,
                                BorderWidthBottom = 0
                            };
                            firma.AddCell(recibi);
                            firma.WriteSelectedRows(0, -1, 70f, alturalinea, cb);

                            alturalinea -= 40;
                            cb.BeginText();
                            cb.SetFontAndSize(bfn, 8);
                            cb.ShowTextAligned(Element.ALIGN_LEFT, "Recibí conforme la suma anotada de acuerdo al concepto mencionado.", 30, alturalinea, 0);
                            cb.EndText();
                        }
                        catch (Exception)
                        {
                            doc.Close();
                        }
                    }
                    doc.NewPage();
                }

                if (doc.IsOpen())
                    doc.Close();

                byte[] pdfBytes = ms.ToArray();
                return File(pdfBytes, "application/pdf", "Rol.pdf");
            }
        }
        [HttpGet]
        public async Task<IActionResult> CrearRolIndividual(string usu, string pass, Int32 empresa, DateTime fecha, Int32 codemp)
        {
            using (var ms = new MemoryStream())
            {
                Document doc = new Document();
                PdfWriter writer = PdfWriter.GetInstance(doc, ms);
                doc.Open();
                System.Data.DataTable dt = await _reportesService.RolIndividual(usu, pass, empresa, fecha, codemp);

                if (dt.Rows.Count > 0)
                {
                    try
                    {
                        string imagePath = "";
                        if (empresa == 3)
                            imagePath = Path.Combine(_env.WebRootPath, "Images", "logo_solo.png");
                        else
                            imagePath = Path.Combine(_env.WebRootPath, "images", "logo_prescolar.jpg");
                        Image img = Image.GetInstance(imagePath);
                        img.ScaleToFit(75f, 75f);
                        doc.Add(img);

                        float alturalinea = 800f;
                        PdfContentByte cb = writer.DirectContent;

                        BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                        BaseFont bfn = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                        iTextSharp.text.Font boldFont = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.BOLD);

                        cb.SetFontAndSize(bf, 12);
                        cb.BeginText();
                        alturalinea -= 20;
                        cb.ShowTextAligned(Element.ALIGN_CENTER, "COPECE", 300, alturalinea, 0);
                        alturalinea -= 20;
                        if (empresa == 3)
                            cb.ShowTextAligned(Element.ALIGN_CENTER, "Unidad Educativa Bilingüe Delta", 300, alturalinea, 0);
                        else
                            cb.ShowTextAligned(Element.ALIGN_CENTER, "Presco DeltaTorremar", 300, alturalinea, 0);

                        string texto = "ROL DE PAGOS " + dt.Rows[0]["TIPO_ROL"].ToString();
                        alturalinea -= 30;
                        cb.ShowTextAligned(Element.ALIGN_CENTER, texto, 300, alturalinea, 0);
                        cb.EndText();

                        cb.SetFontAndSize(bf, 12);
                        cb.BeginText();
                        alturalinea -= 20;
                        cb.ShowTextAligned(Element.ALIGN_LEFT, "Nombre:", 40, alturalinea, 0);
                        cb.ShowTextAligned(Element.ALIGN_LEFT, dt.Rows[0]["RAZONSOCIAL"].ToString(), 100, alturalinea, 0);
                        alturalinea -= 20;
                        cb.ShowTextAligned(Element.ALIGN_LEFT, "Mes:", 40, alturalinea, 0);
                        cb.SetFontAndSize(bfn, 12);
                        cb.ShowTextAligned(Element.ALIGN_LEFT, dt.Rows[0]["MES"].ToString(), 100, alturalinea, 0);
                        cb.EndText();

                        alturalinea -= 10;
                        cb.MoveTo(30f, alturalinea);
                        cb.LineTo(560f, alturalinea);
                        cb.Stroke();
                        alturalinea -= 10;
                        // INGRESOS
                        int ingresos = dt.Select("TIPO='Ingresos'", "COL, ORD,ORDEN,RUBRO").Length;
                        if (ingresos > 0)
                        {
                            DataTable dtIngresos = dt.Select("TIPO='Ingresos'", "COL, ORD,ORDEN,RUBRO").CopyToDataTable();
                            PdfPTable table = new PdfPTable(3);
                            table.TotalWidth = 250f;
                            PdfPCell header = new PdfPCell(new Phrase("INGRESOS", boldFont))
                            {
                                Colspan = 3,
                                HorizontalAlignment = Element.ALIGN_CENTER,
                                FixedHeight = 20
                            };
                            table.AddCell(header);

                            foreach (DataRow i in dtIngresos.Rows)
                            {
                                PdfPCell nombreCompleto = new PdfPCell(new Phrase(i["RUBRO"].ToString()))
                                {
                                    Colspan = 2,
                                    HorizontalAlignment = Element.ALIGN_LEFT,
                                    FixedHeight = 17
                                };
                                table.AddCell(nombreCompleto);

                                string s = string.Format("{0:N2}", i["MONTO"]);
                                PdfPCell monto = new PdfPCell(new Phrase(s))
                                {
                                    HorizontalAlignment = Element.ALIGN_RIGHT,
                                    FixedHeight = 17
                                };
                                table.AddCell(monto);
                            }

                            PdfPCell total = new PdfPCell(new Phrase("Ingreso Mensual", boldFont))
                            {
                                Colspan = 2,
                                HorizontalAlignment = Element.ALIGN_LEFT,
                                FixedHeight = 20
                            };
                            table.AddCell(total);

                            string mt = string.Format("{0:N2}", dtIngresos.Compute("Sum(MONTO)", ""));
                            PdfPCell montot = new PdfPCell(new Phrase(mt, boldFont))
                            {
                                HorizontalAlignment = Element.ALIGN_RIGHT
                            };
                            table.AddCell(montot);

                            table.WriteSelectedRows(0, -1, 30f, alturalinea, cb);
                        }

                        // EGRESOS
                        int egresos = dt.Select("TIPO='Egresos'", "COL, ORD,ORDEN,RUBRO").Length;
                        if (egresos > 0)
                        {
                            DataTable dtEgresos = dt.Select("TIPO='Egresos'", "COL, ORD,ORDEN,RUBRO").CopyToDataTable();
                            PdfPTable table = new PdfPTable(3);
                            table.TotalWidth = 250f;
                            PdfPCell header = new PdfPCell(new Phrase("EGRESOS", boldFont))
                            {
                                Colspan = 3,
                                HorizontalAlignment = Element.ALIGN_CENTER,
                                FixedHeight = 20
                            };
                            table.AddCell(header);

                            foreach (DataRow i in dtEgresos.Rows)
                            {
                                PdfPCell nombreCompleto = new PdfPCell(new Phrase(i["RUBRO"].ToString()))
                                {
                                    Colspan = 2,
                                    HorizontalAlignment = Element.ALIGN_LEFT,
                                    FixedHeight = 17
                                };
                                table.AddCell(nombreCompleto);

                                string s = string.Format("{0:N2}", i["MONTO"]);
                                PdfPCell monto = new PdfPCell(new Phrase(s))
                                {
                                    HorizontalAlignment = Element.ALIGN_RIGHT
                                };
                                table.AddCell(monto);
                            }

                            PdfPCell total = new PdfPCell(new Phrase("Egreso Mensual", boldFont))
                            {
                                Colspan = 2,
                                HorizontalAlignment = Element.ALIGN_LEFT,
                                FixedHeight = 20
                            };
                            table.AddCell(total);

                            string mt = string.Format("{0:N2}", dtEgresos.Compute("Sum(MONTO)", ""));
                            PdfPCell montot = new PdfPCell(new Phrase(mt, boldFont))
                            {
                                HorizontalAlignment = Element.ALIGN_RIGHT
                            };
                            table.AddCell(montot);

                            table.WriteSelectedRows(0, -1, 310f, alturalinea, cb);
                        }
                        //Altura de la tabla
                        Single altura_tabla;
                        if (ingresos > egresos)
                        {
                            altura_tabla = (ingresos * 17) + 50;
                            if ((ingresos - egresos) <= 3)
                                altura_tabla += 30;
                        }
                        else
                        {
                            altura_tabla = (egresos * 17) + 50;
                            if ((ingresos - egresos) <= 3)
                                altura_tabla += 30;
                        }
                        alturalinea -= altura_tabla;
                        // NETO A PAGAR
                        PdfPTable netoTable = new PdfPTable(3);
                        netoTable.TotalWidth = 250f;
                        PdfPCell neto = new PdfPCell(new Phrase("NETO A RECIBIR", boldFont))
                        {
                            Colspan = 2,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        netoTable.AddCell(neto);

                        string apagar = string.Format("{0:N2}", dt.Rows[0]["A_PAGAR"]);
                        PdfPCell montoPagar = new PdfPCell(new Phrase(apagar, boldFont))
                        {
                            HorizontalAlignment = Element.ALIGN_RIGHT
                        };
                        netoTable.AddCell(montoPagar);

                        netoTable.WriteSelectedRows(0, -1, 310f, alturalinea, cb);

                        // Firma
                        PdfPTable firma = new PdfPTable(1);
                        firma.TotalWidth = 150f;
                        PdfPCell recibi = new PdfPCell(new Phrase("Recibí Conforme", boldFont))
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            BorderWidthLeft = 0,
                            BorderWidthRight = 0,
                            BorderWidthBottom = 0
                        };
                        firma.AddCell(recibi);
                        firma.WriteSelectedRows(0, -1, 70f, alturalinea, cb);

                        alturalinea -= 40;
                        cb.BeginText();
                        cb.SetFontAndSize(bfn, 8);
                        cb.ShowTextAligned(Element.ALIGN_LEFT, "Recibí conforme la suma anotada de acuerdo al concepto mencionado.", 30, alturalinea, 0);
                        cb.EndText();
                    }
                    catch (Exception)
                    {
                        doc.Close();
                    }

                }

                if (doc.IsOpen())
                    doc.Close();

                byte[] pdfBytes = ms.ToArray();
                return File(pdfBytes, "application/pdf", "Rol.pdf");
            }
        }
    }
}
