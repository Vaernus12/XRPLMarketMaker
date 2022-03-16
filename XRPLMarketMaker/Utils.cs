using System;
using System.Text;

namespace EMBRS
{
    public static class Utils
    {
        public static string ConvertHex(string hexString)
        {
            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes(hexString);
                return Convert.ToHexString(bytes);
            }
            catch (Exception) { return ""; }
        }

        public static string AddZeros(string s)
        {
            while (s.Length < 40)
            {
                s += "0";
            }
            return s;
        }
    }
}
