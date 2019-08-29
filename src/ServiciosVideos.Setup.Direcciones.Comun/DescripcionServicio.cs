using System;
using System.Collections.Generic;

namespace ServiciosVideos.Setup.Direcciones.Comun
{

    /// <summary>
    /// Contenedor de la informacion sobre el servicio en el cual 
    /// se desea  establecer las direcciones de los servidores
    /// </summary>
    public class DescripcionServicio
    {
        /// <summary
        /// Nombre Descriptivo del Servicio sujeto de actualizacion
        /// Granada || Platino || RegistroPlacas || ReconocimientoPlacas
        /// </summary>
        public string Nombre { get; set; }
        /// <summary>
        /// granada.service || platino.service || reconf-transfer.service 
        /// </summary>
        public string UnitFile { get; set; }  
        /// <summary>
        /// /home/videos/Despliegues//Ci2.Granada.Web
        /// </summary>
        public string DirectorioDespliegue { get; set; }
        /// <summary>
        /// appsettings.Production.json // application.properties 
        /// </summary>
        public string SettingsFile { get; set; }          
        /// <summary>
        /// appsettings.Production.json.template 
        /// </summary>
        public string SettingsTemplate { get; set; }      

    }


    public class ListaDescripcionesServicios
    {
        public List<DescripcionServicio> Items { get; set; } = new List<DescripcionServicio>();
    }
}
