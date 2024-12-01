using System.Globalization;

namespace STech.Utils
{
    public class NumberFormater
    {
        private static readonly string CULTURE_INFO = "vi-VN";

        public static string Format(decimal number)
        {
            CultureInfo cultureInfo = new CultureInfo(CULTURE_INFO);

            if (number > 1_000_000_000)
            {
                return (number / 1_000_000_000).ToString("0,###", cultureInfo) + "tỉ";
            }
            else if (number >= 1_000_000)
            {
                return (number / 1_000_000).ToString("0,###", cultureInfo) + "tr";
            }
            else if (number >= 1_000)
            {
                return (number / 1_000).ToString("0,###", cultureInfo) + "k";
            }
            else
            {
                return number.ToString("0");
            }
        }

        public static string Format(double number)
        {
            CultureInfo cultureInfo = new CultureInfo(CULTURE_INFO);

            if (number > 1_000_000_000)
            {
                return (number / 1_000_000_000).ToString("0,###", cultureInfo) + "tỉ";
            }
            else if (number >= 1_000_000)
            {
                return (number / 1_000_000).ToString("0,###", cultureInfo) + "tr";
            }
            else if (number >= 1_000)
            {
                return (number / 1_000).ToString("0,###", cultureInfo) + "k";
            }
            else
            {
                return number.ToString("0");
            }
        }
    }
}
