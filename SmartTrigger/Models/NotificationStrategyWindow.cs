using System;
using System.Collections.Generic;
using System.Text;

namespace SmartTrigger.Models
{
    public class NotificationStrategyWindow
    {
        public static NotificationStrategyWindow Create(TimeSpan start = default(TimeSpan),
            TimeSpan end = default(TimeSpan))
        {
            return new NotificationStrategyWindow(start, end);
        }
        private NotificationStrategyWindow(TimeSpan start, TimeSpan end)
        {
            if (end < start) throw new ArgumentException("end", "End must be grather than start");
            if ((start - end).TotalHours > 24) throw new ArgumentException("Window cannot we greather than 24hs");

            Start = start;
            End = end;
        }
        public TimeSpan Start { get; private set; }
        public TimeSpan End { get; private set; }
    }
}
