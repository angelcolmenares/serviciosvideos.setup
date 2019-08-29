using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace ServiciosVideos.Setup.Direcciones.Comun
{
    public class JsonProveedorMarcadoresServidores : IProveedorMarcadoresServidores
    {
        private readonly ListaMarcadoresServidores lista = new ListaMarcadoresServidores();

        public JsonProveedorMarcadoresServidores()
        {

            var directorio = Directory.GetCurrentDirectory();

            lista.Bind(directorio);

        }

        public async Task<MarcadorServidor> ObtenerPorNombreServidorAsync(string nombreServidor)
        {
            return await Task.FromResult(lista.Items.FirstOrDefault(q => q.NombreServidor == nombreServidor));
        }

        public MarcadorServidor ObtenerPorNombreServidor(string nombreServidor)
        {
            return lista.Items.FirstOrDefault(q => q.NombreServidor == nombreServidor);
        }

    }    
    
}
