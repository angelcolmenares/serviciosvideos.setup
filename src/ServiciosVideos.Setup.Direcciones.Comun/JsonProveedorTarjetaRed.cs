using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace ServiciosVideos.Setup.Direcciones.Comun
{
   public class JsonProveedorTarjetaRed
    {
        private readonly NetworkInteface _networInteface = new NetworkInteface();

        private readonly string _directorio =  Directory.GetCurrentDirectory();

        public JsonProveedorTarjetaRed()
        {   
            _networInteface.Bind(_directorio);
        }

        

        public  NetworkInteface Obtener()
        {
            return _networInteface;
        }

        public void Guardar(NetworkInteface networInteface)
        {
            networInteface.Save(_directorio);
        }

        public async Task GuardarAsync(NetworkInteface networInteface)
        {
           await  networInteface.SaveAsync(_directorio);
        }
    }
}