using System.Collections;
using Api.Modelos;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Api.Controllers
{
    [Route("/[controller]/[action]")]
    public class PaisesController : Controller
    {  
        private readonly IDeltaContextProcedures _contextp;

        public PaisesController(IDeltaContextProcedures deltaContextProcedures)
        {

            _contextp = deltaContextProcedures;

        }

        // GET: api/<PaisesController>
        [HttpGet]
        public async Task<IActionResult> Get(string usu, string contrasena)
        {
            Paises paises = new Paises();
            return Ok(_contextp.Consultar<Paises>(paises, usu, contrasena));
        }

        // GET api/<PaisesController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<PaisesController>
        [HttpPost]
        public async Task<IActionResult> Post( string values, string usu, string contrasena)
        {
            Paises paises = new Paises();
            paises = JsonConvert.DeserializeObject<Paises>(values);
            var resuy = await _contextp.Guardar(paises, usu, contrasena);
            return Json(new {  });

        }

        // PUT api/<PaisesController>/5
        [HttpPut]
        public async Task<IActionResult> Put(string key, string values, string usu, string contrasena)
        {
            try
            {

            Paises paises = new Paises();
            var Cargos = _contextp.consultaRAW(paises, "select * from Paises where CODPAIS='" + key+"'", usu, contrasena);
            PopulateModel(Cargos.First(), JsonConvert.DeserializeObject<IDictionary>(values));
            var resuy = await _contextp.Updatecodigostring(Cargos.First(), "CODPAIS", Cargos.First().CODPAIS.ToString(), usu, contrasena);

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

        void PopulateModel(Paises order, IDictionary values)
        {
            if (values.Contains("NACIONALIDAD"))
                order.NACIONALIDAD = Convert.ToString(values["NACIONALIDAD"]);
            if (values.Contains("NOMPAIS"))
                order.NOMPAIS = Convert.ToString(values["NOMPAIS"]);
            if (values.Contains("TIPO_NACIONALIDAD"))
                order.TIPO_NACIONALIDAD = Convert.ToString(values["TIPO_NACIONALIDAD"]);
            if (values.Contains("CODPAIS"))
                order.CODPAIS = Convert.ToString(values["CODPAIS"]);
        }

    }
}
