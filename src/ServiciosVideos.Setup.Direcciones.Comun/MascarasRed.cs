using System.Collections.Generic;
using System.Linq;

namespace ServiciosVideos.Setup.Direcciones.Comun
{
    public static class MascarasRed
    {
        private static readonly List<Cidr> _listaCidr = new List<Cidr>()
        {
            new Cidr("32", "255.255.255.255"),
            new Cidr("31", "255.255.255.254"),
            new Cidr("30", "255.255.255.252"),
            new Cidr("29", "255.255.255.248"),
            new Cidr("28", "255.255.255.240"),
            new Cidr("27", "255.255.255.224"),
            new Cidr("26", "255.255.255.192"),
            new Cidr("25", "255.255.255.128"),

            new Cidr("24", "255.255.255.0"),
            new Cidr("23", "255.255.254.0"),
            new Cidr("22", "255.255.252.0"),
            new Cidr("21", "255.255.248.0"),
            new Cidr("20", "255.255.240.0"),
            new Cidr("19", "255.255.224.0"),
            new Cidr("18", "255.255.192.0"),
            new Cidr("17", "255.255.128.0"),

            new Cidr("16", "255.255.0.0"),
            new Cidr("15", "255.254.0.0"),
            new Cidr("14", "255.252.0.0"),
            new Cidr("13", "255.248.0.0"),
            new Cidr("12", "255.240.0.0"),
            new Cidr("11", "255.224.0.0"),
             new Cidr("10", "255.192.0.0"),
             new Cidr("9", "255.128.0.0"),

             new Cidr("8", "255.0.0.0"),
             new Cidr("7", "254.0.0.0"),
             new Cidr("6", "252.0.0.0"),
             new Cidr("5", "248.0.0.0"),
             new Cidr("4", "240.0.0.0"),
             new Cidr("3", "224.0.0.0"),
             new Cidr("2", "192.0.0.0"),
             new Cidr("1", "128.0.0.0"),

        };

        public static List<Cidr> ListaCidr { get => new List<Cidr>(_listaCidr.ToArray()); }

        public static string ObtenerNotacionBits(string notacionDecimal) => _listaCidr.FirstOrDefault(q => q.NotacionDecimal == notacionDecimal)?.NotacionBits ?? "";

        public static string ObtenerNotacionDecimal(string notacionBits) => _listaCidr.FirstOrDefault(q => q.NotacionBits == notacionBits)?.NotacionDecimal ?? "";

    }

    public class Cidr
    {
        public Cidr(string notacionBits, string notacionDecimal)
        {
            NotacionBits = notacionBits;
            NotacionDecimal = notacionDecimal;
        }
        public string NotacionDecimal { get; set; }
        public string NotacionBits { get; set; }
    }

}
