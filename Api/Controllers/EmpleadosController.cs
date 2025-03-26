using Api.Modelos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Api.Controllers
{
    [Route("/[controller]/[action]")]
    [ApiController]
    public class EmpleadosController : Controller
    {
        private readonly IDeltaContextProcedures _contextp;

        public EmpleadosController(IDeltaContextProcedures deltaContextProcedures)
        {
           
            _contextp = deltaContextProcedures;
           
        }

        // GET: api/<EmpleadosController>
        [HttpGet]
        public async Task<IActionResult> Get(string usu,string contrasena)
        {
            return Ok(_contextp.consultarEMP(usu,contrasena));
        }

        // GET api/<EmpleadosController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            return Ok(_contextp.consultarEMPID(id));
        }

        // POST api/<EmpleadosController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] EMP value)
        {
            EMP mP = new EMP();
            mP.MAIL = "hhgeovanny@gmail.com";
            mP.CODEMP = 1327;
            mP.CODGRUPO = 7;
            mP.NOMBRES = "Hector Montero";
            mP.RAZONSOCIAL = "Hector Montero";
            var resuy = await _contextp.GuardarTabla(mP);
            foreach (KeyValuePair<bool, String> entry in resuy)
            {
                if (entry.Key == true)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest(entry.Value);
                }
            }
            return Ok();
        }

        // PUT api/<EmpleadosController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<EmpleadosController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
