using System;
using System.Collections.Generic;
using System.Text;

namespace ServiciosVideos.Setup.Direcciones.Comun
{
    
    /// <summary>
    /// Contenedor  de la informacion almacenada
    /// de un servidor:
    /// NombreServidor y DireccionIp
    /// </summary>
    public class DireccionServidor
    {
        /// <summary>
        /// Equipo en el cual corren los servicios
        /// Videos || Placas || ReconfTransferCentral  || VideoCentral 
        /// con este nombre se busca el Marcador en MarcadorServidor
        /// </summary>
        public string NombreServidor { get; set; }
        /// <summary>
        /// Direccion Ip del servidior
        /// </summary>
        public string DireccionIp { get; set; }

    }

    public class ListaDireccionesServidores
    {
        public List<DireccionServidor> Items { get; set; } = new List<DireccionServidor>();
    }
}
