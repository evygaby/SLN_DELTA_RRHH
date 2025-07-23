using Api.Modelos;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> Orlas(string usu, string pass, int idempresa)

        {
            OrlasPersonal Orla = new OrlasPersonal();
            return Ok(_contextp.CallProceduresConsula(Orla, "prock_personal_web.qry_empleadosxcc(null,"+ idempresa + ",:1)", usu, pass));
        }
    }
}
