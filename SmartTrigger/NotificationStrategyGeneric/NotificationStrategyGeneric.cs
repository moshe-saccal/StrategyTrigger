using SmartTrigger.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartTrigger.NotificationStrategyGeneric
{
    public class NotificationStrategyGeneric : NotificationStrategyBase
    {

        public static NotificationStrategyGeneric Create(IEnumerable<DayOfWeek> avoidedDayOfWeks = null,
                   IEnumerable<NotificationStrategyWindow> notificationStrategyWindows = null,
                   IEnumerable<NotificationStrategyReminder> notificationStrategyReminders = null,
                   TimeSpan expirationSpanAfterInitialDate = default(TimeSpan),
                   TimeSpan expirationSpanBeforeEndingDate = default(TimeSpan),
                   TimeSpan minSpanBetweenNotifications = default(TimeSpan))
        {
            return new NotificationStrategyGeneric(
                avoidedDayOfWeks: avoidedDayOfWeks,
                notificationStrategyWindows: notificationStrategyWindows,
                notificationStrategyReminders: notificationStrategyReminders,
                expirationSpanAfterInitialDate: expirationSpanAfterInitialDate,
                expirationSpanBeforeEndingDate: expirationSpanBeforeEndingDate,
                minSpanBetweenNotifications: minSpanBetweenNotifications
                );
        }

        private NotificationStrategyGeneric(IEnumerable<DayOfWeek> avoidedDayOfWeks = null,
          IEnumerable<NotificationStrategyWindow> notificationStrategyWindows = null,
          IEnumerable<NotificationStrategyReminder> notificationStrategyReminders = null,
          TimeSpan expirationSpanAfterInitialDate = default(TimeSpan),
          TimeSpan expirationSpanBeforeEndingDate = default(TimeSpan),
          TimeSpan minSpanBetweenNotifications = default(TimeSpan)
          )
        {

            init(
                _avoidedDayOfWeks = avoidedDayOfWeks,
                _notificationStrategyWindows = notificationStrategyWindows,
                _notificationStrategyReminders = notificationStrategyReminders,
                _expirationSpanAfterInitialDate = expirationSpanAfterInitialDate,
                _expirationSpanBeforeEndingDate = expirationSpanBeforeEndingDate,
                _minSpanBetweenNotifications = minSpanBetweenNotifications
                );
        }

        private IEnumerable<DayOfWeek> _avoidedDayOfWeks;
        private IEnumerable<NotificationStrategyWindow> _notificationStrategyWindows;
        private IEnumerable<NotificationStrategyReminder> _notificationStrategyReminders;
        private TimeSpan _expirationSpanAfterInitialDate;
        private TimeSpan _expirationSpanBeforeEndingDate;
        private TimeSpan _minSpanBetweenNotifications;


        public override IEnumerable<DayOfWeek> AvoidedDayOfWeks
            => _avoidedDayOfWeks;

        public override IEnumerable<NotificationStrategyWindow> NotificationStrategyWindows
            => _notificationStrategyWindows;
        public override IEnumerable<NotificationStrategyReminder> NotificationStrategyReminders
            => _notificationStrategyReminders;

        public override TimeSpan ExpirationSpanAfterInitialDate
               => _expirationSpanAfterInitialDate;

        public override TimeSpan ExpirationSpanBeforeEndingDate
            => _expirationSpanBeforeEndingDate;

        public override TimeSpan MinSpanBetweenNotifications
            => _minSpanBetweenNotifications;
        private void init(IEnumerable<DayOfWeek> avoidedDayOfWeks = null,
            IEnumerable<NotificationStrategyWindow> notificationStrategyWindows = null,
          IEnumerable<NotificationStrategyReminder> notificationStrategyReminders = null,
          TimeSpan expirationSpanAfterInitialDate = default(TimeSpan),
          TimeSpan expirationSpanBeforeEndingDate = default(TimeSpan),
          TimeSpan minSpanBetweenNotifications = default(TimeSpan))
        {
            _avoidedDayOfWeks = avoidedDayOfWeks;
            _notificationStrategyWindows = notificationStrategyWindows;
            _notificationStrategyReminders = notificationStrategyReminders;
            _expirationSpanAfterInitialDate = expirationSpanAfterInitialDate;
            _expirationSpanBeforeEndingDate = expirationSpanBeforeEndingDate;
        }

    }
}
