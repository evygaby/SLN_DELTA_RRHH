
using System.Data;

namespace Api.Services.Interfaces
{
    public interface IReportesService
    {
        Task<DataTable> ListadoPEGsxSeccion(string usu, string pass, string periodo, List<string> niveles);
        Task<DataTable> ListadoPreceptorasxSeccion(string usu, string pass, string periodo, List<string> niveles);
        Task<DataSet> ActualizaDatosEmpleado(string usuario, string pass, int codigo);
        string SeccionesSeleccionadas(List<string> secciones);
        string Encrypt(string dataToEncrypt, string password);
    }
   
}
