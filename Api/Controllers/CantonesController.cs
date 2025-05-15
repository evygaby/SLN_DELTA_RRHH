using System.Collections;
using Api.Modelos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Api.Controllers
{
    [Route("/[controller]/[action]")]

    public class CantonesController : Controller
    {
        private readonly IDeltaContextProcedures _contextp;

        public CantonesController(IDeltaContextProcedures deltaContextProcedures)
        {

            _contextp = deltaContextProcedures;

        }

        // GET: api/<PaisesController>
        [HttpGet]
        public async Task<IActionResult> Get(string usu, string contrasena)
        {
            CANTONES paises = new CANTONES();
            return Ok(_contextp.Consultar<CANTONES>(paises, usu, contrasena));
        }

        // GET api/<PaisesController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<PaisesController>
        [HttpPost]
        public async Task<IActionResult> Post(string values, string usu, string contrasena)
        {
            CANTONES paises = new CANTONES();
            paises = JsonConvert.DeserializeObject<CANTONES>(values);
            var sec = _contextp.ConsultarnumeroUltimoRegistro(paises, "CODCANTON", usu, contrasena);
            paises.CODCANTON = sec;
            var resuy = await _contextp.Guardar(paises, usu, contrasena);
            return Json(new { });

        }

        // PUT api/<PaisesController>/5
        [HttpPut]
        public async Task<IActionResult> Put(string key, string values, string usu, string contrasena)
        {
            try
            {

                CANTONES paises = new CANTONES();
                var Cargos = _contextp.consultaRAW(paises, "select * from CANTONES where CODCANTON=" + key, usu, contrasena);
                PopulateModel(Cargos.First(), JsonConvert.DeserializeObject<IDictionary>(values));
                var resuy = await _contextp.Update(Cargos.First(), "CODCANTON", Cargos.First().CODCANTON.ToString(), usu, contrasena);

                return Ok();
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }

        }

        // DELETE api/<PaisesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        void PopulateModel(CANTONES order, IDictionary values)
        {
            if (values.Contains("CODCANTON"))
                order.CODCANTON = Convert.ToInt16(values["CODCANTON"]);
            if (values.Contains("NOMCANTON"))
                order.NOMCANTON = Convert.ToString(values["NOMCANTON"]);
            if (values.Contains("CODPAIS"))
                order.CODPAIS = Convert.ToString(values["CODPAIS"]);
            if (values.Contains("CODPROV"))
                order.CODPROV = Convert.ToString(values["CODPROV"]);
        }
    }
}
