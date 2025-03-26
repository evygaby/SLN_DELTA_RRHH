using Api.Modelos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Api.Controllers
{
    [Route("/[controller]/[action]")]
    [ApiController]
    public class UsuariosController : Controller
    {
        private readonly IDeltaContextProcedures _contextp;
        public UsuariosController(IDeltaContextProcedures deltaContextProcedures)
        {

            _contextp = deltaContextProcedures;

        }
        // GET: api/<UsuariosController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<UsuariosController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<UsuariosController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<UsuariosController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<UsuariosController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        [HttpPost()]
        public async Task<ActionResult<Login>> Login(string username, string password)
        {

            try
            {
                DBOracle dB1 = new DBOracle();
                ClsConfig.cadenaoracle = dB1.crearcadena(ClsConfig.DATA_SOURCE, username, password);
              var login = dB1.login("proc_k_academico_web.qryloginpass", username, password);
               
                return Ok(login);
            }
            catch (Exception ex)
            {
               
                return BadRequest(ClsConfig.MensajeErrorGenerico);
            }

        }


    }
}
