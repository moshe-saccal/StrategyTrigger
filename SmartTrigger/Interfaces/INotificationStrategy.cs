using SmartTrigger.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartTrigger.Interfaces
{
    public interface INotificationStrategy
    {
        //public bool RetryIfDayIsVoided { get;  }
        public IEnumerable<DayOfWeek> AvoidedDayOfWeks { get; }
        public IEnumerable<NotificationStrategyWindow> NotificationStrategyWindows { get; } // only sent 
        public IEnumerable<NotificationStrategyReminder> NotificationStrategyReminders { get; }
        public TimeSpan SpanAfterInitialDate { get; } // Expiration Span after the starting date...
        public TimeSpan SpanBeforeEndingDate { get; } //  Expiration Span after the ending date...
    }
}
