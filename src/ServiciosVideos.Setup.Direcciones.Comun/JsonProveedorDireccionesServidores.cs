using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ServiciosVideos.Setup.Direcciones.Comun
{
    public class JsonProveedorDireccionesServidores : IProveedorDireccionesServidores
    {
        private readonly ListaDireccionesServidores lista = new ListaDireccionesServidores();
        private readonly string _directorio = Directory.GetCurrentDirectory();

        public JsonProveedorDireccionesServidores()
        {
            
            lista.Bind(_directorio);
        }

        public async Task<List<DireccionServidor>> ConsultarAsync()
        {
            return await Task.FromResult(lista.Items);
        }

        public async Task GuardarAsync(IList<DireccionServidor> inputs)
        {
            var listaActualizada = new ListaDireccionesServidores();
            listaActualizada.Items.AddRange(inputs);

            await listaActualizada.SaveAsync(_directorio);

            lista.Items.Clear();
            lista.Items.AddRange(listaActualizada.Items);
        }

        public async Task GuardarAsync(IList<ActualizarDireccionServidorInput> inputs)
        {
            var listaActualizada = new ListaDireccionesServidores();
            foreach( var direccion in inputs)
            {
                listaActualizada.Items.Add(new DireccionServidor { NombreServidor = direccion.NombreServidor, DireccionIp = direccion.DireccionIp });
            }

            var directorio = Directory.GetCurrentDirectory();

            await listaActualizada.SaveAsync(directorio);
            lista.Items.Clear();
            lista.Items.AddRange(listaActualizada.Items);

        }


        public  List<DireccionServidor> Consultar()
        {
            return lista.Items;
        }

        public void Guardar(IList<ActualizarDireccionServidorInput> inputs)
        {
            var listaActualizada = new ListaDireccionesServidores();
            foreach (var direccion in inputs)
            {
                listaActualizada.Items.Add(new DireccionServidor { NombreServidor = direccion.NombreServidor, DireccionIp = direccion.DireccionIp });
            }

            
            listaActualizada.Save(_directorio);
            lista.Items.Clear();
            lista.Items.AddRange(listaActualizada.Items);
        }


        public void Guardar(IList<DireccionServidor> inputs)
        {
            var listaActualizada = new ListaDireccionesServidores();
            listaActualizada.Items.AddRange(inputs);

            listaActualizada.Save(_directorio);

            lista.Items.Clear();
            lista.Items.AddRange(listaActualizada.Items);
        }
    }
}
