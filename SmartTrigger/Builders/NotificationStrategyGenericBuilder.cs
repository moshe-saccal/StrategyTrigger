using SmartTrigger.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartTrigger.NotificationStrategyGeneric
{
    public class NotificationStrategyGenericBuilder
    {
        private IEnumerable<DayOfWeek> _avoidedDayOfWeks;
        private IEnumerable<NotificationStrategyWindow> _notificationStrategyWindows;
        private IEnumerable<NotificationStrategyReminder> _notificationStrategyReminders;
        private TimeSpan _expirationSpanAfterInitialDate;
        private TimeSpan _expirationSpanBeforeEndingDate;
        private TimeSpan _minSpanBetweenNotifications;
        public NotificationStrategyGenericBuilder WithAvoidedDaysOfWeeks(params DayOfWeek[] avoidedDayOfWeks)
        {
            return WithAvoidedDaysOfWeeks(avoidedDayOfWeks.AsEnumerable());
        }
        public NotificationStrategyGenericBuilder WithAvoidedDaysOfWeeks(IEnumerable<DayOfWeek> avoidedDayOfWeks)
        {
            _avoidedDayOfWeks = avoidedDayOfWeks;
            return this;
        }
        public static NotificationStrategyGenericBuilder Create() => new NotificationStrategyGenericBuilder();

        public NotificationStrategyGenericBuilder WithNotificationWindows(params NotificationStrategyWindow[] notificationStrategyWindows)
        {
            return WithNotificationWindows(notificationStrategyWindows.AsEnumerable());
        }
        public NotificationStrategyGenericBuilder WithNotificationWindows(IEnumerable<NotificationStrategyWindow> notificationStrategyWindows)
        {
            _notificationStrategyWindows = notificationStrategyWindows;
            return this;
        }
        public NotificationStrategyGenericBuilder WithNotificationReminders(
            params TimeSpan[] notificationStrategyReminders)
        {
            return WithNotificationReminders(notificationStrategyReminders.Select(a => NotificationStrategyReminder.Create(a)));
        }

        public NotificationStrategyGenericBuilder WithNotificationReminders(
            params NotificationStrategyReminder[] notificationStrategyReminders)
        {
            return WithNotificationReminders(notificationStrategyReminders.AsEnumerable());
        }


        public NotificationStrategyGenericBuilder WithNotificationReminders(

            IEnumerable<NotificationStrategyReminder> notificationStrategyReminders)
        {
            //_notificationStrategyWindows = notificationStrategyWindows;
            _notificationStrategyReminders = notificationStrategyReminders;
            return this;
        }
        public NotificationStrategyGenericBuilder WithExpirationSpanAfterInitialDate(
            TimeSpan expirationSpanAfterInitialDate)
        {
            _expirationSpanAfterInitialDate = expirationSpanAfterInitialDate;
            return this;
        }
        public NotificationStrategyGenericBuilder WithExpirationSpanBeforeEndingDate(
         TimeSpan expirationSpanBeforeEndingDate)
        {
            _expirationSpanBeforeEndingDate = expirationSpanBeforeEndingDate;
            return this;
        }
        public NotificationStrategyGenericBuilder WithMinSpanBetweenNotifications(
     TimeSpan minSpanBetweenNotifications)
        {
            _minSpanBetweenNotifications = minSpanBetweenNotifications;
            return this;
        }


        //IEnumerable<NotificationStrategyReminder> notificationStrategyReminders

        public NotificationStrategyGeneric Build()
        {
            return NotificationStrategyGeneric.Create(avoidedDayOfWeks: _avoidedDayOfWeks,
                notificationStrategyWindows: _notificationStrategyWindows,
                  notificationStrategyReminders: _notificationStrategyReminders,
                   expirationSpanAfterInitialDate: _expirationSpanAfterInitialDate,
                   expirationSpanBeforeEndingDate: _expirationSpanBeforeEndingDate,
                   minSpanBetweenNotifications: _minSpanBetweenNotifications);
        }

    }
}
