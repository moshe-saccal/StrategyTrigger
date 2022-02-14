using System;
using System.Collections.Generic;
using System.Text;

namespace SmartTrigger.Models
{
    public class NotificationStrategyReminder
    {
        public static NotificationStrategyReminder Create(TimeSpan interval)
        {
            if (interval.TotalSeconds < 10) throw new ArgumentNullException("interval", "Interval Min 10 seconds");
            if (interval.TotalDays > 365) throw new ArgumentNullException("interval", "Interval Max 365 days");
            return new NotificationStrategyReminder(interval);
        }
        private NotificationStrategyReminder(TimeSpan interval)
        {
            Interval = interval;
        }
        public TimeSpan Interval { get; private set; }
        //public int IntervalMultiplcator { get; set; }
    }
}
