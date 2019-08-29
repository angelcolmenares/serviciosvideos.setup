using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServiciosVideos.Setup.Direcciones.Comun
{
    public interface IProveedorDescripcionesServicios
    {
        Task<List<DescripcionServicio>> ConsultarAsync();
        List<DescripcionServicio> Consultar();
    }
}