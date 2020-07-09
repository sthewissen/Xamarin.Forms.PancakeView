using System;
using System.Globalization;
using System.Linq;

namespace Xamarin.Forms.PancakeView
{
    [Xaml.TypeConversion(typeof(DashPattern))]
    public class DashPatternTypeConverter : TypeConverter
    {
        public override object ConvertFromInvariantString(string value)
        {
            if (value != null)
            {
                string[] values = value.Split(',');
                bool[] areValidIntegers = values.Select(s => Int32.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out var x)).ToArray();

                if (Array.TrueForAll(areValidIntegers, x => x))
                {
                    return new DashPattern(values.Select(s => int.Parse(s, NumberStyles.Number, CultureInfo.InvariantCulture)).ToArray());
                }
            }

            throw new InvalidOperationException(string.Format("Cannot convert \"{0}\" into {1}", value, typeof(DashPattern)));
        }
    }
}
