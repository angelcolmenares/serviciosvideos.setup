using System.Threading.Tasks;

namespace ServiciosVideos.Setup.Direcciones.Comun
{
    public interface IProveedorMarcadoresServidores
    {        
        Task<MarcadorServidor> ObtenerPorNombreServidorAsync( string nombreServidor);
        MarcadorServidor ObtenerPorNombreServidor(string nombreServidor);
    }
}

