using System.Collections;
using Api.Modelos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Api.Controllers
{
    [Route("/[controller]/[action]")]

    public class ProvinciasController : Controller
    {
        private readonly IDeltaContextProcedures _contextp;

        public ProvinciasController(IDeltaContextProcedures deltaContextProcedures)
        {

            _contextp = deltaContextProcedures;

        }

        // GET: api/<PaisesController>
        [HttpGet]
        public async Task<IActionResult> Get(string usu, string contrasena)
        {
            Provincias paises = new Provincias();
            return Ok(_contextp.Consultar<Provincias>(paises, usu, contrasena));
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
            Provincias paises = new Provincias();
            paises = JsonConvert.DeserializeObject<Provincias>(values);
            
            var resuy = await _contextp.Guardar(paises, usu, contrasena);
            return Json(new { });

        }

        // PUT api/<PaisesController>/5
        [HttpPut]
        public async Task<IActionResult> Put(string key, string values, string usu, string contrasena)
        {
            try
            {

                Provincias paises = new Provincias();
                var Cargos = _contextp.consultaRAW(paises, "select * from Provincias where CODPROV='" + key + "'", usu, contrasena);
                PopulateModel(Cargos.First(), JsonConvert.DeserializeObject<IDictionary>(values));
                var resuy = await _contextp.Updatecodigostring(Cargos.First(), "CODPROV", Cargos.First().CODPROV.ToString(), usu, contrasena);

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

        void PopulateModel(Provincias order, IDictionary values)
        {
            if (values.Contains("CODPROV"))
                order.CODPROV = Convert.ToString(values["CODPROV"]);
            if (values.Contains("CODPAIS"))
                order.CODPAIS = Convert.ToString(values["CODPAIS"]);
            if (values.Contains("NOMPROV"))
                order.NOMPROV = Convert.ToString(values["NOMPROV"]);
            if (values.Contains("COD_SRI"))
                order.COD_SRI = Convert.ToInt32(values["COD_SRI"]);
        }
    }
}
