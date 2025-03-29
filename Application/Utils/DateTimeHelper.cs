using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Utils
{
    public static class DateTimeHelper
    {
        public static DateTime GetColombiaTimeNow()
        {
            var colombiaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, colombiaTimeZone)
                               .AddTicks(-(DateTime.UtcNow.Ticks % TimeSpan.TicksPerSecond));
        }
    }
}