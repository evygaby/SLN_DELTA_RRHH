using Api;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
using Infra.Logging;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

DBOracle dB = new DBOracle();

ClsConfig.USER_ID = builder.Configuration.GetSection("USER_ID").Value;
ClsConfig.DATA_SOURCE = builder.Configuration.GetSection("DATA_SOURCE").Value;
ClsConfig.PASSWORD = builder.Configuration.GetSection("PASSWORD").Value;
var connectioinOracle = dB.crearcadena(ClsConfig.DATA_SOURCE, ClsConfig.USER_ID, ClsConfig.PASSWORD);
ClsConfig.cadenaoracle = connectioinOracle;
builder.Logging.ClearProviders();
builder.Logging.AddCustomLogging("C:\\Logs\\Dinamico");
builder.Services.AddDbContext<ModelOracleContext>(options => options.UseOracle(connectioinOracle));
builder.Services.AddScoped<IDeltaContextProcedures, DeltaContextProcedures>();
builder.Services.AddControllers();
builder.Services
    .AddControllers()
    .AddNewtonsoftJson(options =>
    {
        // don't serialize with CamelCase (see https://github.com/aspnet/Announcements/issues/194)
        options.SerializerSettings.ContractResolver = new DefaultContractResolver();
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options => options.AddPolicy("ApiCorsPolicy", builder =>
{
    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
})); ;

var app = builder.Build();

app.UseCors("ApiCorsPolicy");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.UseStaticFiles();
app.UseAuthorization();
app.MapFallbackToFile("index.html");
app.MapControllers();

app.Run();
