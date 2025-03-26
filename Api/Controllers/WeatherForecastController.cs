using System;
using System.Data;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Text;
using Api.Modelos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Api.Controllers
{
    [ApiController]
    [Route("/[controller]/[action]")]

    public class WeatherForecastController : Controller
    {
        private readonly IDeltaContextProcedures _contextp;
        private readonly ModelOracleContext _context;
        
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

       
       


        public WeatherForecastController(IDeltaContextProcedures deltaContextProcedures, ModelOracleContext context,ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
            _contextp = deltaContextProcedures;
            _context = context;
        }


        [HttpGet(Name = "Empleados")]
        public async Task<IActionResult> Empleados()
        {
        return Ok(_contextp.consultarEMP("",""));
        }

       [HttpGet(Name = "GetWeatherForecast")]
        public  async Task<IActionResult> Get()
        {
         
            EMP mP = new EMP();
            mP.MAIL = "YO";
            mP.CODGRUPO = 7;
            mP.NOMBRES = "Hector Montero";
            mP.NOMBRES = "Hector Montero";
            var resuy= await  _contextp.GuardarTabla(mP);
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
       
      
    }
}
