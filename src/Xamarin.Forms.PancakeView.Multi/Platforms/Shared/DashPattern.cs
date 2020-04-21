using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Xamarin.Forms.PancakeView
{
    [TypeConverter(typeof(DashPatternTypeConverter))]
    public struct DashPattern
    {
        public int[] Pattern { get; set; }

        public DashPattern(params int[] pattern) : this()
        {
            Pattern = pattern;
        }

        public override string ToString()
        {
            if (Pattern == null)
                return string.Empty;

            return string.Join(",", Pattern.Select(x => x.ToString(CultureInfo.InvariantCulture)));
        }
    }
}
