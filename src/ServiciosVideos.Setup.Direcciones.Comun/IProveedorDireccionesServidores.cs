using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServiciosVideos.Setup.Direcciones.Comun
{
    public interface IProveedorDireccionesServidores
    {
        Task<List<DireccionServidor>> ConsultarAsync();
        Task GuardarAsync(IList<ActualizarDireccionServidorInput> inputs);

        List<DireccionServidor> Consultar();
        void Guardar(IList<ActualizarDireccionServidorInput> inputs);
    }
}
