using System.Collections.Generic;
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
        private readonly ILogger<EmpleadosController> _logger;
        public EmpleadosController(ILogger<EmpleadosController> logger,IDeltaContextProcedures deltaContextProcedures)
        {
            _logger = logger;
            _contextp = deltaContextProcedures;
           
        }

        // GET: api/<EmpleadosController>
        [HttpGet]
        public async Task<IActionResult> Get(string usu,string contrasena,int idempresa)
        {
            EMP empleado = new EMP();
            rh_familiar_discapacidad rh_discapacidad = new rh_familiar_discapacidad();
            rh_familiar_enfermedad rh_enfermedad = new rh_familiar_enfermedad();
            rh_cargas_empleados rh_cargas = new rh_cargas_empleados();
            detalle_grupocentrocosto detalle_grupocentrocosto = new detalle_grupocentrocosto();
            crgemp crgemp = new crgemp();
            CGTEMPCTAS gTEMPCTAS= new CGTEMPCTAS();
            SUELDOS sUELDOS = new SUELDOS();
            crgdep crgdep = new crgdep();
            NUMCTABCO uMCTABCO = new NUMCTABCO();
            titulosacademicos_emp titulosacademicos = new titulosacademicos_emp();
           // var SUELDOS = _contextp.consultaRAW(sUELDOS, "select r.codemp,r.fecha,r.tipcontrato,GET_NOMCCOSTO(R.CODCCOSTO, R.ID_EMPRESA) centro_costo,\r\nR.A_PAGAR,\r\n(select I.MONTO from developer1.rol_ingresos i where i.fecha=r.fecha and i.codemp=r.codemp and i.codrubro='SD') SUELDO,\r\n(select I.MONTO from developer1.rol_ingresos i where i.fecha=r.fecha and i.codemp=r.codemp and i.codrubro='HE') EXTRAS,\r\n(select I.MONTO from developer1.rol_ingresos i where i.fecha=r.fecha and i.codemp=r.codemp and i.codrubro='HO') OTROS,\r\nR.MONTOI INGRESOS,R.MONTOE EGRESOS,R.DIAS_ENF,R.DIAS_MAT,NVL(R.DIAS_SINSUELDO,0) SINSUELDO\r\n  from developer1.rol_mensual r  WHERE R.ID_EMPRESA=" + idempresa, usu, contrasena);

            var Departamentos = _contextp.consultaRAW(crgdep, "SELECT c.activo, c.codemp, C.CODCRG,  D.CODDEP,  S.codsec,C.ID_EMPRESA,C.CARGO_PRINCIPAL FROM crgemp C\r\nINNER JOIN secciones S ON C.codsec= S.codsec\r\nINNER JOIN crgdep d ON D.codCrG= c.codCrG AND D.CODDEP= S.CODDEP AND C.ID_EMPRESA=" + idempresa+ " and d.id_empresa="+idempresa, usu, contrasena);
            var CuentasContables = _contextp.consultaRAW(gTEMPCTAS, "SELECT c.ACTIVO, c.TIPO_CTA,C.CODEMP,C.ID_EMPRESA,C.PLA_CODCNTA\r\nFROM   FINANZAS.CN_CUENTA T\r\ninner join CGTEMPCTAS C  on c.pla_codcnta=T.cuenta\r\nWHERE  T.ID_PADRE IN (41415, 41918, 3540,343)\r\nAND    T.STATUS = 1\r\nAND T.ID_EMPRESA =" + idempresa, usu, contrasena);
            var CuentasBancos = _contextp.consultaRAW(uMCTABCO, "select ESTADO,ID_EMPRESA,CODPERSONA,CODBANCO,CTABCO,PORCENT,TIPO_CUENTA,TIPPERSONA from NUMCTABCO where id_empresa=" + idempresa, usu, contrasena);
            var Grupo = _contextp.consultaRAW(detalle_grupocentrocosto, "select * from detalle_grupocentrocosto", usu, contrasena);
            var Cargos = _contextp.consultaRAW(crgemp, "select * from crgemp where id_empresa="+idempresa, usu, contrasena);
            var Titulos = _contextp.consultaRAW(titulosacademicos, "SELECT IDTITULO,ID_EMPRESA, CODEMP,NIVEL,TITULO,PAIS,INSTITUCION,ESTADOESTUDIO,REGSENESCYT ,NUMREGSENESCYT,ESTADO,ANIOGRADUAPREVISTA ,NIV_EN_CURSO FROM titulosacademicos_emp where id_empresa=" + idempresa, usu, contrasena);
            var emmpleados = _contextp.consultarEMP<EMP>(empleado, usu, contrasena, idempresa).ToList();
            var cargas = _contextp.consultaRAW(rh_cargas, "select * from rh_cargas_empleados where id_empresa="+idempresa, usu, contrasena);
            var enferedades = _contextp.consultaRAW(rh_enfermedad, "select * from rh_familiar_enfermedad where id_empresa="+idempresa, usu, contrasena);
            var discapacidad = _contextp.consultaRAW(rh_discapacidad, "select * from rh_familiar_discapacidad where id_empresa="+idempresa, usu, contrasena);
            List<crgdep> lista = new List<crgdep>();
            foreach (EMP item in emmpleados)
            {
               lista = new List<crgdep>();
                item.FamiliarCargas = cargas.Where(x =>x.CODEMP==item.CODEMP).ToList();
                item.FamiliarDiscapicidad = discapacidad.Where(x => x.CODEMP == item.CODEMP).ToList();
                item.FamiliarEnfermedad = enferedades.Where(x => x.CODEMP == item.CODEMP).ToList();
                item.CentroCosto = Grupo.Where(x => x.CODEMPLEADO == item.CODEMP).ToList();
                item.CuentasBancos = CuentasBancos.Where(x => x.CODPERSONA == item.CODEMP).ToList();
                item.CuentasContables = CuentasContables.Where(x => x.CODEMP == item.CODEMP).ToList();
                item.Cargos = Cargos.Where(x => x.CODEMP == item.CODEMP).ToList();
                item.Titulos = Titulos.Where(x => x.CODEMP == item.CODEMP).ToList();
            //    item.Sueldos = SUELDOS.Where(x => x.CODEMP == item.CODEMP).ToList();
                foreach (crgemp item2 in item.Cargos)
                { 
                    var dep= Departamentos.Where(x => x.CODEMP == item2.CODEMP && x.CODCRG==item2.CODCRG && x.ID_EMPRESA==item2.ID_EMPRESA).ToList();
                    lista.AddRange(dep);
                }
                item.Departamentos = lista;
            }
            return Ok(emmpleados);
        }
        [HttpGet]
        public async Task<IActionResult> CENTRO(string usu, string contrasena, int idempresa)

        {
            centrocosto paises = new centrocosto();
            return Ok(_contextp.consultarXId<centrocosto>(paises, usu, contrasena, idempresa));
        }

        [HttpGet]
        public async Task<IActionResult> JEFAS(string usu, string contrasena)

        {
            JEFAS paises = new JEFAS();
            return Ok(_contextp.consultaRAW(paises, "select CODEMP,RAZONSOCIAL from EMP where ACTIVO='S'  order by razonsocial", usu, contrasena));
        }


        [HttpGet]
        public async Task<IActionResult> GCENTROCOSTO2(string usu, string contrasena)

        {
            grupo_centrocosto paises = new grupo_centrocosto();
            return Ok(_contextp.consultaRAW(paises, "select * from grupo_centrocosto", usu, contrasena));
        }
        [HttpGet]
        public async Task<IActionResult> CUENTAS(string usu, string contrasena, int idempresa)

        {
            CUENTAS  paises = new CUENTAS();
            var SUELDOS = _contextp.consultaRAW(paises, "SELECT CAST(t.descripcion AS varchar2(100)) cuenta , CAST(t.cuenta AS varchar2(10)) ID FROM   FINANZAS.CN_CUENTA T WHERE  T.ID_EMPRESA = " + idempresa + " AND    T.ID_PADRE IN (41415, 41918, 3540,343) AND    T.STATUS = 1" , usu, contrasena);
            return Ok(SUELDOS);
        }
        [HttpGet]
        public async Task<IActionResult> TIPOCUENTA(string usu, string contrasena, int idempresa)

        {
            TIPOCUENTA paises = new TIPOCUENTA();
            var SUELDOS = _contextp.consultaRAW(paises, "select CODRUBRO tipo_cta, DESCRIPCION nombre_tipo from rubrosrol where id_empresa =" + idempresa , usu, contrasena);
            return Ok(SUELDOS);
        }
        [HttpGet]
        public async Task<IActionResult> DESCIESS(string usu, string contrasena)

        {
            IESS paises = new IESS();
            var SUELDOS = _contextp.consultaRAW(paises, "select CODIGO_IESS, CARGO_ACTIVIDAD from CARGOS_SECTORIAL", usu, contrasena);
            return Ok(SUELDOS);
        }
        [HttpGet]
        public async Task<IActionResult> SEGUROS(string usu, string contrasena, int idempresa)

        {
            SEGUROS paises = new SEGUROS();
            var SUELDOS = _contextp.consultaRAW(paises, "SELECT P.CADENA, P.DESCRIPCION FROM PARAMETROS P WHERE P.MODULO = 'RRHH' AND P.ESTADO = 'A' AND P.CODPAR = 'SEGURO_EMP' AND P.ID_EMPRESA ="+ idempresa +" UNION SELECT 'NO', 'Ninguno' FROM DUAL " , usu, contrasena);
          
           
            return Ok(SUELDOS);
      
        }
        [HttpGet]
        public async Task<IActionResult> EXTENSION(string usu, string contrasena, int idempresa)

        {
            EXT paises = new EXT();
            var SUELDOS = _contextp.consultaRAW(paises, "select * from developer1.EXT t where ID_EMPRESA = "+ idempresa, usu, contrasena);


            return Ok(SUELDOS);

        }
        [HttpGet]
        public async Task<IActionResult> Sueldos(string usu, string contrasena,int codemp)

        {
            try
            {
            SUELDOS paises = new SUELDOS();
            var SUELDOS = _contextp.consultaRAW(paises, "select r.codemp,r.fecha,r.tipcontrato,GET_NOMCCOSTO(R.CODCCOSTO, R.ID_EMPRESA) centro_costo,\r\nR.A_PAGAR,\r\n(select I.MONTO from developer1.rol_ingresos i where i.fecha=r.fecha and i.codemp=r.codemp and i.codrubro='SD') SUELDO,\r\n(select I.MONTO from developer1.rol_ingresos i where i.fecha=r.fecha and i.codemp=r.codemp and i.codrubro='HE') EXTRAS,\r\n(select I.MONTO from developer1.rol_ingresos i where i.fecha=r.fecha and i.codemp=r.codemp and i.codrubro='HO') OTROS,\r\nR.MONTOI INGRESOS,R.MONTOE EGRESOS,R.DIAS_ENF,R.DIAS_MAT,NVL(R.DIAS_SINSUELDO,0) SINSUELDO\r\n  from developer1.rol_mensual r  WHERE R.codemp=" + codemp+" "+ " ORDER BY 2 DESC" , usu, contrasena);

            return Ok(SUELDOS);
            }catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(new {message=ex.Message });
            }
        }
        [HttpGet]
        public async Task<IActionResult> capacitaciones(string usu, string contrasena, string cedula)

        {
            try
            {
                capacitaciones paises = new capacitaciones();
                var capacitaciones = _contextp.consultaRAW(paises, "select c.fecha,c.empresa,c.descripcion,c.tipo_asistencia,c.duracion,c.porc_subsidio from rh_capacitaciones_det d  inner join rh_capacitaciones_cab c on c.id_capacitacion=d.id_capacitacion and c.estado='A' where d.identificacion='"+ cedula + "' and  d.asistencia=1", usu, contrasena);

                return Ok(capacitaciones);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet]
        public IActionResult Bancos(string usu, string contrasena)

        {
            try
            {
                BANCOS paises = new BANCOS();
            return Ok(_contextp.Consultar<BANCOS>(paises, usu, contrasena));
        }catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(new {message=ex.Message
    });
            }
        }
        [HttpGet]
        public async Task<IActionResult> PAISES(string usu, string contrasena)
        {
            try
            {
                Paises paises = new Paises();
            return Ok(_contextp.Consultar<Paises>(paises , usu, contrasena));
        }catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(new {message=ex.Message
    });
            }
        }
        [HttpGet]
        public async Task<IActionResult> Zonas(string usu, string contrasena)
        {
            try
            {
                Zonas paises = new Zonas();
            return Ok(_contextp.Consultar<Zonas>(paises, usu, contrasena));
        }catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(new {message=ex.Message
    });
            }
        }
        [HttpGet]
        public async Task<IActionResult> CENTROCOSTOMIN(string usu, string contrasena, int idempresa)
        {
            try { 
            centrocosto_minis paises = new centrocosto_minis();
            return Ok(_contextp.consultarXId<centrocosto_minis>(paises, usu, contrasena, idempresa));
        }catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(new {message=ex.Message});
            }
        }
        [HttpGet]
        public async Task<IActionResult> PROVINCIA(string usu, string contrasena)
        {
            try
            {
                Provincias paises = new Provincias();
            return Ok(_contextp.Consultar<Provincias>(paises, usu, contrasena));
        }catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(new {message=ex.Message});
            }
        }

        [HttpGet]
        public async Task<IActionResult> CANTONES(string usu, string contrasena)
        {
            try
            {
                CANTONES paises = new CANTONES();
            return Ok(_contextp.Consultar<CANTONES>(paises, usu, contrasena));
        }catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(new {message=ex.Message
    });
            }
        }
        [HttpGet]
        public async Task<IActionResult> CUIDADES(string usu, string contrasena)
        {
            try
            {
                CIUDADES paises = new CIUDADES();
            return Ok(_contextp.Consultar<CIUDADES>(paises, usu, contrasena));
}catch(Exception ex)
            {
                _logger.LogError(ex.Message);
return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet]
        public async Task<IActionResult> EMPRESAS(string usu, string contrasena)
        {
            try
            {
                Empresa paises = new Empresa();
            return Ok(_contextp.Consultar<Empresa>(paises, usu, contrasena));
}catch(Exception ex)
            {
                _logger.LogError(ex.Message);
return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Depa(string usu, string contrasena, int idempresa)
        {
            try
            {
                dept paises = new dept();
            return Ok(_contextp.consultaRAW<dept>(paises, "select CODDEP,nomdep from dept where id_empresa="+idempresa, usu, contrasena));
}catch(Exception ex)
            {
                _logger.LogError(ex.Message);
return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ListaCargos(string usu, string contrasena,int idempresa)
        {
            try
            {
                Cargos paises = new Cargos();
            return Ok(_contextp.consultaRAW<Cargos>(paises, "select CODCRG,NOMCRG from cargos where id_empresa="+idempresa, usu, contrasena));
}catch(Exception ex)
            {
                _logger.LogError(ex.Message);
return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet]
        public async Task<IActionResult> Secciones(string usu, string contrasena, int idempresa)
        {
            try
            {
                SECCIONES paises = new SECCIONES();
            return Ok(_contextp.consultaRAW<SECCIONES>(paises, "select coddep,codsec,nomsec from SECCIONES where id_empresa=" + idempresa, usu, contrasena));
        }catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET api/<EmpleadosController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            return Ok(_contextp.consultarEMPID(id));
        }

        // POST api/<EmpleadosController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] EMP value, string usu, string contrasena)
        {

            try
            {
                _logger.LogInformation("guardando");
                var ID = _contextp.ConsultarUltimoCODEMP(value, usu, contrasena);
                value.CODEMP = ID + 1;
                var resuy = await _contextp.Guardar(value, usu, contrasena);
                string mensaje = "";
                if (validar(resuy, ref mensaje) == false)
                {
                    _logger.LogError(mensaje);
                    return BadRequest(new { message = mensaje });
                }
                foreach (rh_cargas_empleados item in value.FamiliarCargas!)

                {
                    var sec = _contextp.ConsultarnumeroUltimoRegistro(item, "ID_HIJO", usu, contrasena);
                    item.ID_HIJO = sec;
                    item.CODEMP = (short?)value.CODEMP;
                    item.ID_EMPRESA = (short?)value.ID_EMPRESA;
                    resuy = await _contextp.Guardar(item, usu, contrasena);
                     mensaje = "";
                    if (validar(resuy, ref mensaje) == false)
                    {
                        _logger.LogError(mensaje);
                        return BadRequest(new { message = mensaje });
                    }
                }
             
                foreach (crgdep item in value.Departamentos!)

                {
                    var sec = _contextp.ConsultarnumeroUltimoRegistro(new crgemp(), "IDCRGEMP", usu, contrasena);
                    crgemp crgemp = new crgemp();
                    crgemp.CODSEC = item.CODSEC;
                    crgemp.ACTIVO = item.ACTIVO;
                    crgemp.CODCRG = item.CODCRG;
              
                    crgemp.CARGO_PRINCIPAL = item.CARGO_PRINCIPAL;
                    crgemp.CODEMP = (short?)value.CODEMP;
                    crgemp.IDCRGEMP = sec;
                 
                    crgemp.ID_EMPRESA = (short?)value.ID_EMPRESA;
                    resuy = await _contextp.Guardar(crgemp, usu, contrasena);
                    mensaje = "";
                    if (validar(resuy, ref mensaje) == false)
                    {
                        _logger.LogError(mensaje);
                        return BadRequest(new { message = mensaje });
                    }
                }
                 foreach (detalle_grupocentrocosto item in value.CentroCosto!)
                {

                    item.CODEMPLEADO = (short?)value.CODEMP;
                    item.IDEMPRESA = (short?)value.ID_EMPRESA;
                    resuy = await _contextp.Guardar(item, usu, contrasena);
                    mensaje = "";
                    if (validar(resuy, ref mensaje) == false)
                    {
                        _logger.LogError(mensaje);
                        return BadRequest(new { message = mensaje });
                    }
                }
                foreach (titulosacademicos_emp item in value.Titulos!)
                {
                    var sec = _contextp.ConsultarnumeroUltimoRegistro(item, "IDTITULO", usu, contrasena);
                    item.CODEMP = (short?)value.CODEMP;
                    item.IDTITULO = sec;
                    item.ID_EMPRESA = (short?)value.ID_EMPRESA;
                    resuy = await _contextp.Guardar(item, usu, contrasena);
                    mensaje = "";
                    if (validar(resuy, ref mensaje) == false)
                    {
                        _logger.LogError(mensaje);
                        return BadRequest(new { message = mensaje });
                    }
                }

                foreach (CGTEMPCTAS item in value.CuentasContables!)
                {

                    item.CODEMP = (short?)value.CODEMP;
                    item.ID_EMPRESA = (short?)value.ID_EMPRESA;
                    resuy = await _contextp.Guardar(item, usu, contrasena);
                    if (validar(resuy, ref mensaje) == false)
                    {
                        _logger.LogError(mensaje);
                        return BadRequest(new { message = mensaje });
                    }
                }
                 foreach (rh_familiar_discapacidad item in value.FamiliarDiscapicidad!)
                {
                    var sec = _contextp.ConsultarnumeroUltimoRegistro(item, "IDFAMILIA", usu, contrasena);
                    item.IDFAMILIA = sec;
                    item.CODEMP = (short?)value.CODEMP;
                    item.ID_EMPRESA = (short?)value.ID_EMPRESA;
                    resuy = await _contextp.Guardar(item, usu, contrasena);
                    if (validar(resuy, ref mensaje) == false)
                    {
                        _logger.LogError(mensaje);
                        return BadRequest(new { message = mensaje });
                    }
                }

                foreach (NUMCTABCO item in value.CuentasBancos!)
                {
                    item.TIPPERSONA = "E";
                    item.CODPERSONA = (short?)value.CODEMP;
                    item.ID_EMPRESA = (short?)value.ID_EMPRESA;
                    resuy = await _contextp.Guardar(item, usu, contrasena);
                    if (validar(resuy, ref mensaje) == false)
                    {
                        _logger.LogError(mensaje);
                        return BadRequest(new { message = mensaje });
                    }
                }
                foreach (rh_familiar_enfermedad item in value.FamiliarEnfermedad!)
                {
                    var sec = _contextp.ConsultarnumeroUltimoRegistro(item, "IDFAMILIA", usu, contrasena);
                    item.IDFAMILIA = sec;
                    item.CODEMP = value.CODEMP;
                    item.ID_EMPRESA = (short?)value.ID_EMPRESA;
                    resuy = await _contextp.Guardar(item, usu, contrasena);
                    if (validar(resuy, ref mensaje) == false)
                    {
                        _logger.LogError(mensaje);
                        return BadRequest(new { message = mensaje });
                    }
                }


                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.InnerException.ToString());
                return BadRequest(new { message = ex.Message });
            }
           
        }
        private Boolean  validar(Dictionary<bool, string> valor, ref string error)
        {
            bool valid = false;
            foreach (KeyValuePair<bool, String> entry in valor)
            {
                if (entry.Key == true)
                {
                    valid= true;
                }
                else
                {
                    error = entry.Value;
                    valid= false;
                }
            }
         return valid;
        }
            // PUT api/<EmpleadosController>/5
            [HttpPut]
        public async Task<IActionResult> Put(int id, [FromBody] EMP value, string usu, string contrasena)
        {
            try { 
            
     
         
            var res5 = new Dictionary<bool, string>();
            var res4 = new Dictionary<bool, string>();
            var res3 = new Dictionary<bool, string>();
            var res2 = new Dictionary<bool, string>();
            var res1 = new Dictionary<bool, string>();
            var res6 = new Dictionary<bool, string>();
            var cargos = new Dictionary<bool, string>();
            var titulos = new Dictionary<bool, string>();
            var resuy = await _contextp.Update(value, "CODEMP", value.CODEMP.ToString(), usu, contrasena);
                string mensaje = "";
                if (validar(resuy, ref mensaje) == false)
                {
                    _logger.LogError(mensaje);
                    return BadRequest(new { message = mensaje });
                }
                res1 = await _contextp.delete(new rh_cargas_empleados(), "CODEMP", value.CODEMP.ToString(), usu, contrasena);
                mensaje = "";
                if (validar(res1, ref mensaje) == false)
                {
                    _logger.LogError(mensaje);
                    return BadRequest(new { message = mensaje });
                }
                foreach (rh_cargas_empleados item in value.FamiliarCargas! ?? new List<rh_cargas_empleados>())

            {   
                var sec =  _contextp.ConsultarnumeroUltimoRegistro(item, "ID_HIJO", usu, contrasena);
                item.ID_HIJO = sec;
                item.CODEMP = (short?)value.CODEMP;
                item.ID_EMPRESA = (short?)value.ID_EMPRESA;
                resuy = await _contextp.Guardar(item, usu, contrasena);
                    mensaje = "";
                    if (validar(res1, ref mensaje) == false)
                    {
                        _logger.LogError(mensaje);
                        return BadRequest(new { message = mensaje + "familias Cargas " });
                    }
                }
            cargos = await _contextp.delete(new crgemp(), "CODEMP", value.CODEMP.ToString(), usu, contrasena);
                mensaje = "";
                if (validar(cargos, ref mensaje) == false)
                {
                    _logger.LogError(mensaje);
                    return BadRequest(new { message = mensaje + "FamiliarEnfermedad " });
                }
                foreach (crgdep item in value.Departamentos! ?? new List<crgdep>())

            {
                var sec = _contextp.ConsultarnumeroUltimoRegistro(new crgemp(), "IDCRGEMP", usu, contrasena);
                crgemp crgemp = new crgemp();
                crgemp.CODSEC = item.CODSEC;
                crgemp.ACTIVO = item.ACTIVO;
                crgemp.CODCRG = item.CODCRG;

                crgemp.CARGO_PRINCIPAL = item.CARGO_PRINCIPAL;
                crgemp.CODEMP = (short?)value.CODEMP;
                crgemp.IDCRGEMP = sec;

                crgemp.ID_EMPRESA = (short?)value.ID_EMPRESA;
                resuy = await _contextp.Guardar(crgemp, usu, contrasena);
                    mensaje = "";
                    if (validar(res1, ref mensaje) == false)
                    {
                        _logger.LogError(mensaje);
                        return BadRequest(new { message = mensaje + "Departamentos " });
                    }
                }
            res6 = await _contextp.delete(new detalle_grupocentrocosto(), "CODEMPLEADO", value.CODEMP.ToString(), usu, contrasena);
                mensaje = "";
                if (validar(res6, ref mensaje) == false)
                {
                    _logger.LogError(mensaje);
                    return BadRequest(new { message = mensaje + "FamiliarEnfermedad " });
                }
                foreach (detalle_grupocentrocosto item in value.CentroCosto! ?? new List<detalle_grupocentrocosto>())
            {

                item.CODEMPLEADO = (short?)value.CODEMP;
                item.IDEMPRESA = (short?)value.ID_EMPRESA;
                resuy = await _contextp.Guardar(item, usu, contrasena);
                    mensaje = "";
                    if (validar(res1, ref mensaje) == false)
                    {
                        _logger.LogError(mensaje);
                        return BadRequest(new { message = mensaje + "Centros "});
                    }
                }
            titulos  = await _contextp.delete(new titulosacademicos_emp(), "CODEMP", value.CODEMP.ToString(), usu, contrasena);
                mensaje = "";
                if (validar(titulos , ref mensaje) == false)
                {
                    _logger.LogError(mensaje);
                    return BadRequest(new { message = mensaje + "FamiliarEnfermedad " });
                }
                foreach (titulosacademicos_emp item in value.Titulos! ?? new List<titulosacademicos_emp>())
            {
                var sec = _contextp.ConsultarnumeroUltimoRegistro(item, "IDTITULO", usu, contrasena);
                item.CODEMP = (short?)value.CODEMP;
                item.IDTITULO = sec;
                item.ID_EMPRESA = (short?)value.ID_EMPRESA;
                resuy = await _contextp.Guardar(item, usu, contrasena);
                    mensaje = "";
                    if (validar(res1, ref mensaje) == false)
                    {
                        _logger.LogError(mensaje);
                        return BadRequest(new { message = mensaje + "titulos " });
                    }
                }

            res2 = await _contextp.delete(new CGTEMPCTAS(), "CODEMP", value.CODEMP.ToString(), usu, contrasena);
                mensaje = "";
                if (validar(res2, ref mensaje) == false)
                {
                    _logger.LogError(mensaje);
                    return BadRequest(new { message = mensaje + "FamiliarEnfermedad " });
                }
                foreach (CGTEMPCTAS item in value.CuentasContables! ?? new List<CGTEMPCTAS>())
            {

                item.CODEMP = (short?)value.CODEMP;
                item.ID_EMPRESA = (short?)value.ID_EMPRESA;
                resuy = await _contextp.Guardar(item, usu, contrasena);
                    mensaje = "";
                    if (validar(res1, ref mensaje) == false)
                    {
                        _logger.LogError(mensaje);
                        return BadRequest(new { message = mensaje + "CuentasContables " });
                    }
                }
            res2 = await _contextp.delete(new rh_familiar_discapacidad(), "CODEMP", value.CODEMP.ToString(), usu, contrasena);
                mensaje = "";
                if (validar(res2, ref mensaje) == false)
                {
                    _logger.LogError(mensaje);
                    return BadRequest(new { message = mensaje + "FamiliarEnfermedad " });
                }
                foreach (rh_familiar_discapacidad item in value.FamiliarDiscapicidad! ?? new List<rh_familiar_discapacidad>())
            {
                var sec = _contextp.ConsultarnumeroUltimoRegistro(item, "IDFAMILIA", usu, contrasena);
                item.IDFAMILIA = sec;
                item.CODEMP = (short?)value.CODEMP;
                item.ID_EMPRESA = (short?)value.ID_EMPRESA;
                resuy = await _contextp.Guardar(item, usu, contrasena);
                    mensaje = "";
                    if (validar(res1, ref mensaje) == false)
                    {
                        _logger.LogError(mensaje);
                        return BadRequest(new { message = mensaje + "FamiliarDiscapicidad " });
                    }
                }

            res5 = await _contextp.delete(new NUMCTABCO(), "CODPERSONA", value.CODEMP.ToString(), usu, contrasena);
                mensaje = "";
                if (validar(res5, ref mensaje) == false)
                {
                    _logger.LogError(mensaje);
                    return BadRequest(new { message = mensaje + "FamiliarEnfermedad " });
                }
                foreach (NUMCTABCO item in value.CuentasBancos! ?? new List<NUMCTABCO>())
            {
                item.TIPPERSONA = "E";
                item.CODPERSONA = (short?)value.CODEMP;
                item.ID_EMPRESA = (short?)value.ID_EMPRESA;
                resuy = await _contextp.Guardar(item, usu, contrasena);
                    mensaje = "";
                    if (validar(res1, ref mensaje) == false)
                    {
                        _logger.LogError(mensaje);
                        return BadRequest(new { message = mensaje + "CuentasBancos " });
                    }
                }

            res3 = await _contextp.delete(new rh_familiar_enfermedad(), "CODEMP", value.CODEMP.ToString(), usu, contrasena);
                mensaje = "";
                if (validar(res3, ref mensaje) == false)
                {
                    _logger.LogError(mensaje);
                    return BadRequest(new { message = mensaje + "FamiliarEnfermedad " });
                }
                foreach (rh_familiar_enfermedad item in value.FamiliarEnfermedad! ?? new List<rh_familiar_enfermedad>())
            {
                var sec = _contextp.ConsultarnumeroUltimoRegistro(item, "IDFAMILIA", usu, contrasena);
                item.IDFAMILIA = sec;
                item.CODEMP = value.CODEMP;
                item.ID_EMPRESA = (short?)value.ID_EMPRESA;
                resuy = await _contextp.Guardar(item, usu, contrasena);
                    mensaje = "";
                    if (validar(res1, ref mensaje) == false)
                    {
                        _logger.LogError(mensaje);
                        return BadRequest(new { message = mensaje + "FamiliarEnfermedad " });
                    }
              }
          
         
            return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.InnerException.ToString());
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE api/<EmpleadosController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }


    public class JEFAS
    {

        public Int16? CODEMP { get; set; }
        public string? RAZONSOCIAL { get; set; }



    }
}
