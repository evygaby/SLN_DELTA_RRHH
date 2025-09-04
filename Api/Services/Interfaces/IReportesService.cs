
using System.Data;

namespace Api.Services.Interfaces
{
    public interface IReportesService
    {
        Task<DataTable> ListadoPEGsxSeccion(string usu, string pass, string periodo, List<string> niveles);
        Task<DataTable> ListadoPreceptorasxSeccion(string usu, string pass, string periodo, List<string> niveles);
        Task<DataSet> ActualizaDatosEmpleado(string usuario, string pass, int codigo);
        Task<DataTable> Prestamos(string usu, string pass, Int32 empresa, Int32? saldo, DateTime? desde, DateTime? hasta);
        Task<DataTable> ListaRol(string usu, string pass, Int32 empresa, DateTime fecha, string ccosto, string trol);
        Task<DataTable> RolIndividual(string usu, string pass, Int32 empresa, DateTime fecha, Int32 codEmp);
        string SeccionesSeleccionadas(List<string> secciones);
        string Encrypt(string dataToEncrypt, string password);
    }
   
}
