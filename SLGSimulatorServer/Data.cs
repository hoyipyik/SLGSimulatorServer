using System;
using System.Collections.Generic;
using System.Linq;

namespace SLGSimulatorServer
{
    public sealed class Data
    {
        private static Dictionary<string, string> playerTypes;
        private static Dictionary<string, string> cityTypes;
        static Data()
        {
            playerTypes = new Dictionary<string, string> {
                { "username", "string" },
                { "password", "string" },
                { "soldierNum", "int" },
                { "cityId", "int" },
            };
            cityTypes = new Dictionary<string, string>
            {
                { "userId", "int" },
                { "username", "string" },
                { "cityName", "string" },
            };
        }

        public static List<KeyValuePair<string, string>> PlayerTypes => playerTypes.ToList();

        public static List<KeyValuePair<string, string>> CityTypes => cityTypes.ToList();
    }
}