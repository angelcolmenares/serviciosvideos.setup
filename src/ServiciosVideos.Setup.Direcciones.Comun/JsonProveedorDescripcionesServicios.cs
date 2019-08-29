using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ServiciosVideos.Setup.Direcciones.Comun
{

    public class JsonProveedorDescripcionesServicios : IProveedorDescripcionesServicios
    {
        private readonly ListaDescripcionesServicios lista = new ListaDescripcionesServicios();

        public JsonProveedorDescripcionesServicios()
        {
            var directorio = Directory.GetCurrentDirectory();
            lista.Bind(directorio);
        }

        public async Task<List<DescripcionServicio>> ConsultarAsync()
        {
            return await Task.FromResult(lista.Items);
        }

        public List<DescripcionServicio> Consultar()
        {
            return lista.Items;
        }

    }
}
