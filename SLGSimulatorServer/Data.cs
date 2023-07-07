using System;
using System.Collections.Generic;
using System.Linq;

namespace SLGSimulatorServer
{
    public sealed class Data
    {
        private static Dictionary<string, string> types;

        static Data()
        {
            var rawTypes = new Dictionary<string, string> {
                { "username", "string" },
                {"password", "string" },
                {"soldierNum", "int" },
            };
            types = rawTypes;
        }

        public static List<KeyValuePair<string, string>> Types => types.ToList();
    }
}