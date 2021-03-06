using SmartTrigger.Interfaces;
using System;
using System.Collections.Generic;

namespace SmartTrigger.Models
{
    public abstract class NotificationStrategyBase : INotificationStrategy
    {
        public IEnumerable<DayOfWeek> _avoidedDayOfWeks;


        public abstract IEnumerable<DayOfWeek> AvoidedDayOfWeks { get; }
        public abstract IEnumerable<NotificationStrategyWindow> NotificationStrategyWindows { get; }
        public abstract IEnumerable<NotificationStrategyReminder> NotificationStrategyReminders { get; }

        public abstract TimeSpan SpanAfterInitialDate { get; }

        public abstract TimeSpan SpanBeforeEndingDate { get; }
        public abstract TimeSpan MinSpanBetweenNotifications { get; }



    }
}
