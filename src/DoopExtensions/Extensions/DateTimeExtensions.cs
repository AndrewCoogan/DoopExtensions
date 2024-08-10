using System;

namespace DoopExtensions.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime AddWorkingDays(this DateTime date, int daysToAdd)
        {
            if (daysToAdd < 0)
            {
                throw new ArgumentException($"Can not add negative days: {daysToAdd}.");
            }

            while (daysToAdd > 0)
            {
                date = date.AddDays(1);

                if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                {
                    daysToAdd -= 1;
                }
            }
            return date;
        }

        public static DateTime SubtractWorkingDays(this DateTime date, int daysToSubtract)
        {
            if (daysToSubtract < 0)
            {
                throw new ArgumentException($"Can not subtract negative days: {daysToSubtract}.");
            }

            while (daysToSubtract > 0)
            {
                date = date.AddDays(-1);

                if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                {
                    daysToSubtract -= 1;
                }
            }
            return date;
        }
    }
}