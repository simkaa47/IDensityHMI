namespace IDensity.Core.Extentions
{
    public static class FloatToStringExtention
    {
        public static string ToStringPoint(this float num)
        {
            return num.ToString().Replace(",", ".");
        }
    }
    public static class StringExtensions
    {
        public static float StringToFloat(this string str)
        {
            float temp = 0;
            return float.TryParse(str.Replace(".", ","), out temp) ? temp : default;
        }
    }
}
