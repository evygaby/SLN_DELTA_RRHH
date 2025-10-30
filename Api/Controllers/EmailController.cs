using Api;
using Api.Services.Implementations;
using Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Mail;

[ApiController]
[Route("/[controller]/[action]")]
public class EmailController : ControllerBase
{
    private readonly SmtpSettings _smtpSettings;
    private readonly IReportesService _reportesService;
    public EmailController(IOptions<SmtpSettings> smtpSettings, IReportesService reportesService)
    {
        _smtpSettings = smtpSettings.Value;
        _reportesService = reportesService;
    }

    [HttpPost]
    public IActionResult SendEmail([FromBody] EmailRequest request)
    {
        try
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", "Plantilla.txt");
           var codigo= _reportesService.Encrypt(request.codemp + "|" + request.idempresa, "d3lt@_act_emp_2024");
            var linkinicial = "https://actualizaciondatos.uedelta.k12.ec/wfrmfichasocial.aspx?a="+codigo;
            if (!System.IO.File.Exists(filePath))
                return NotFound(new { message = "Archivo de plantilla no encontrado" });
            var smtpClient = new SmtpClient(_smtpSettings.Host)
            {
                Port = _smtpSettings.Port,
                Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
                EnableSsl = _smtpSettings.EnableSsl
            };
            var body = System.IO.File.ReadAllText(filePath);
            body = body.Replace("[link]", linkinicial);
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_smtpSettings.FromEmail, _smtpSettings.FromName),
                Subject = request.Subject,
                Body = body,
              
                IsBodyHtml = request.IsHtml
            };
            mailMessage.CC.Add(_smtpSettings.CCO);
            mailMessage.To.Add(request.To);
            smtpClient.Send(mailMessage);
            DBOracle dB1 = new DBOracle();
            ClsConfig.cadenaoracle = dB1.crearcadena(ClsConfig.DATA_SOURCE, request.usu, request.pass);
            var login = dB1.habilitaficha("prock_personal_web.upd_envioficha", request.codemp, request.usu, request.pass);
            return Ok(new { message = "Correo enviado correctamente" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Error al enviar correo", error = ex.Message });
        }
    }
}

public class EmailRequest
{
    public string To { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    //public string Body { get; set; } = string.Empty;
    public bool IsHtml { get; set; } = true;
    public string usu { get; set; } = string.Empty;
    public string pass { get; set; } = string.Empty;
    public int codemp { get; set; } 
    public int idempresa { get; set; }
}
public class SmtpSettings
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool EnableSsl { get; set; }
    public string FromName { get; set; } = string.Empty;
    public string FromEmail { get; set; } = string.Empty;
    public string CCO { get; set; } = string.Empty;
}