namespace ServiciosVideos.Setup.Direcciones.Comun
{
    /// <summary>
    /// Contenedor  con la informacion 
    /// del servidor (NombreServidor y DireccionIp)
    /// que debe ser registrada
    /// en los diferentes servicios
    /// </summary>
    public class ActualizarDireccionServidorInput
    {
        /// <summary>
        /// Equipo en el cual corren los servicios
        /// Videos || Placas || ReconfTransferCentral  || VideoCentral 
        /// con este nombre se busca el Marcador en MarcadorServidor
        /// </summary>
        public string NombreServidor { get; set; }
        /// <summary>
        /// Direccion Ip que se debe registrar en las configuraciones
        /// de acuerdo en el valor establecido en MarcadorServidor 
        /// El valor de DireccionIp remplaza el Marcador dentro del archivo de configuraciones
        /// </summary>
        public string DireccionIp { get; set; }
        

    }
}
