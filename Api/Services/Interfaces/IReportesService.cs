
using System.Data;

namespace Api.Services.Interfaces
{
    public interface IReportesService
    {
        Task<DataTable> ListadoPEGsxSeccion(string usu, string pass, string periodo, List<string> niveles);
        Task<DataTable> ListadoPreceptorasxSeccion(string usu, string pass, string periodo, List<string> niveles);
        string SeccionesSeleccionadas(List<string> secciones);
    }
   
}
