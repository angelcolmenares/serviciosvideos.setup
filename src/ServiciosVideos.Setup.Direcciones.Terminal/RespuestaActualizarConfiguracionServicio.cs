using ServiciosVideos.Setup.Direcciones.Comun;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace ServiciosVideos.Setup.Direcciones.TerminalGui
{
    public class RespuestaActualizarConfiguracionServicio
    {
        public DescripcionServicio  Servicio { get; set; }
        public ResultadoActualizarDirecciones ResultadoActualizacion { get; set; }
    }

    public class ListaRespuestaActualizarConfiguracionServicio
    {
        public List<RespuestaActualizarConfiguracionServicio> Items { get; set; } = new List<RespuestaActualizarConfiguracionServicio>();

        public void Add( RespuestaActualizarConfiguracionServicio item)
        {            
            Items.Add(item);
        }

        public void SaveToFile(string directorio=null)
        {
            string _directorio = string.IsNullOrEmpty(directorio) ? Directory.GetCurrentDirectory() : directorio;
            this.Save(_directorio);
        }

        public async Task SaveToFileAsync(string directorio = null)
        {
            string _directorio = string.IsNullOrEmpty(directorio) ? Directory.GetCurrentDirectory() : directorio;
            await this.SaveAsync(_directorio);
        }


        public void LoadFromFile(string directorio = null)
        {
            string _directorio = string.IsNullOrEmpty(directorio) ? Directory.GetCurrentDirectory() : directorio;
            Items.Clear();
            this.Bind(_directorio);
        }

    }


}
