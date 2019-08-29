using System;
using System.Collections.Generic;
using System.Text;

namespace ServiciosVideos.Setup.Direcciones.Comun
{
    public class MarcadorServidor
    {
        /// <summary>
        /// Videos || Placas || ReconfTransferCentral  || VideoCentral 
        /// Identificador unico del Servidor (PC ) donde corren los servicios
        /// </summary>
        public string NombreServidor { get; set; }

        /// <summary>
        ///  Marcador de posicion para la DireccionIp
        /// </summary>
        public string MarcadorDireccionIp { get; set; }

    }

    public class ListaMarcadoresServidores
    {
        public IList<MarcadorServidor> Items { get; set; } = new List<MarcadorServidor>();
    }


}
