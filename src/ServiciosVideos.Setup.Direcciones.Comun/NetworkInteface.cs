using System;
using System.Collections.Generic;
using System.Text;

namespace ServiciosVideos.Setup.Direcciones.Comun
{
    public class NetworkInteface
    {
        public string CardName { get; set; } = "";
        public string Inet { get; set; } = "";
        public string Netmask { get; set; } = "";
        public string Broadcast { get; set; } = "";

        public string Gateway { get; set; } = "";
        /// <summary>
        /// nameservers:
        //  addresses: 8.8.8.8,8.8.4.4
        /// </summary>
        public string NameServersAddresses { get; set; } = "";

        public string CidrBits { get => MascarasRed.ObtenerNotacionBits(Netmask); }


        public override string ToString()
        {
            return $"{CardName.PadRight(7,' ')}  Ip: {Inet.PadLeft(15,' ')}  Mascara: {CidrBits.PadLeft(2,' ')} ({Netmask.PadLeft(15,' ')})  Broadcast: {Broadcast.PadLeft(15,' ')} ";
        }
    }
}
