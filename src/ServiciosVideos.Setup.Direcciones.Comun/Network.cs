using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ServiciosVideos.Setup.Direcciones.Comun
{
    public static class Network
    {
        public static readonly string loopback = "lo";

        private static readonly Regex ip = new Regex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b");


        /// <summary>
        /// mascarara en  representacion 1 -32
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsValidCidr(string value)
        {
            return (int.TryParse(value, out var m) && m >= 1 && m <= 32);
        }

        public static bool IsValidIpAddress(string value)
        {
            var dots = (value.Count(q => q == '.'));
            if (string.IsNullOrEmpty(value) || dots != 3 || (value.Length> 15 || value.Length<7))
            {
                return false;
            }

            var result = ip.Matches(value);
            return result.Count == 1;
        }

                

        /// <summary>
        /// ls /sys/class/net -1"
        /// </summary>
        /// <returns></returns>
        /// 
        public static async Task<List<string>> GetNetworkCardsAsync()
        {
            var result = await Bash.ExecuteAsync("ls /sys/class/net -1");
            return result.OutputList;
        }

        /// <summary>
        /// ls /sys/class/net -1"
        /// </summary>
        /// <returns></returns>
        /// 
        public static List<string> GetNetworkCards()
        {
            var result = Bash.Execute("ls /sys/class/net -1");
            return result.OutputList;
        }

        public static List<NetworkInteface> GetNetworkInterfaces()
        {
            var cards = GetNetworkCards();

            var interfaces = (Bash.Execute("ifconfig -a")).OutputList;
            return ExtractNetworkInterfaces(cards, interfaces);

        }

        public static async Task<List<NetworkInteface>> GetNetworkInterfacesAsync()
        {
            var cards = await GetNetworkCardsAsync();

            var interfaces = (await Bash.ExecuteAsync("ifconfig -a")).OutputList;
            return ExtractNetworkInterfaces(cards, interfaces);

        }

        private static List<NetworkInteface> ExtractNetworkInterfaces(List<string> cards, List<string> interfaces)
        {
            var result = new List<NetworkInteface>();

            foreach (var card in cards)
            {

                var cardIndex = interfaces.FindIndex(q => q.StartsWith($"{card}: "));

                if (cardIndex < 0 || cardIndex + 1 >= interfaces.Count)
                {
                    result.Add(new NetworkInteface { CardName = card });
                    continue;
                }

                var candidate = interfaces[cardIndex + 1].Trim();

                if (!candidate.StartsWith("inet "))
                {
                    result.Add(new NetworkInteface { CardName = card });
                    continue;
                }


                var list = new List<string>(candidate.Split(' '));

                var inet = list.Skip(1).Take(1).FirstOrDefault() ?? "";

                var index_netmask = list.IndexOf("netmask");
                var netmask = list.Skip(index_netmask > 0 ? index_netmask + 1 : list.Count).Take(1).FirstOrDefault() ?? "";

                var index_broadcast = list.IndexOf("broadcast");
                var broadcast = list.Skip(index_broadcast > 0 ? index_broadcast + 1 : list.Count).Take(1).FirstOrDefault() ?? "";

                result.Add(new NetworkInteface { Inet = inet, Broadcast = broadcast, CardName = card, Netmask = netmask });

            }

            return result;
        }
    }
}
