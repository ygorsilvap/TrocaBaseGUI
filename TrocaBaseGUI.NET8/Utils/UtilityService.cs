using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrocaBaseGUI.Utils
{
    public static class UtilityService
    {
        public static string ToCapitalize(string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;
            return char.ToUpper(str[0]) + str.Substring(1).ToLower();
        }

        public static string IdGen()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
