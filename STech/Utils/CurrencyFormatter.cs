using System.Globalization;

namespace STech.Utils
{
    public static class CurrencyFormatter
    {
        private static readonly string CURRENCY_UNIT = "đ";
        private static readonly string CULTURE_INFO = "vi-VN";

        public static string Format(decimal? amount)
        {
            if(amount == null)
            {
                return "";
            }

            CultureInfo cultureInfo = new CultureInfo(CULTURE_INFO);
            return amount.Value.ToString("##,###", cultureInfo) + CURRENCY_UNIT;
        }
    }
}
