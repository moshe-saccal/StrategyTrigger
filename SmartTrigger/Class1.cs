using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTrigger
{

    public static class SmartTriggerInternalExtensions
    {
        public static bool IsInsideWindow(this IEnumerable<NotificationStrategyWindow> windows,
            DateTime current)
        {
            foreach (var w in windows)
            {
                if (current.TimeOfDay > w.Start && current.TimeOfDay <= w.End)
                    return true;
            }
            return false;
        }
    }
    public class NotificationStrategyWindow
    {
        public static NotificationStrategyWindow Create( TimeSpan start =default(TimeSpan),
            TimeSpan end = default(TimeSpan))
        {
            return new NotificationStrategyWindow(start, end);
        }
        private NotificationStrategyWindow(TimeSpan start, TimeSpan end)
        {
            if (end > start) throw new ArgumentException("end","End must be grather than start");
            Start = start;
            End = end;
        }
        public TimeSpan Start { get; private set; }
        public TimeSpan End { get; private  set; }
    }
    public class NotificationStrategyReminder
    {
        public static NotificationStrategyReminder Create(TimeSpan interval)
        {
            if (interval.TotalSeconds <= 10) throw new ArgumentNullException("interval","Interval Min 10 seconds");
            return new NotificationStrategyReminder(interval);
        }
        private NotificationStrategyReminder(TimeSpan interval)
        {
            Interval = interval;
        }
        public TimeSpan Interval { get; private set; }
        //public int IntervalMultiplcator { get; set; }
    }
    public class NotificationStrategyGenericBuilder
    { 
        private IEnumerable<DayOfWeek> _avoidedDayOfWeks;
        private IEnumerable<NotificationStrategyWindow> _notificationStrategyWindows;
        private IEnumerable<NotificationStrategyReminder> _notificationStrategyReminders;
        private TimeSpan _expirationSpanAfterInitialDate;
        private TimeSpan _expirationSpanBeforeEndingDate;
        public NotificationStrategyGenericBuilder WithAvoidedDaysOfWeeks(params DayOfWeek [] avoidedDayOfWeks)
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
            return WithNotificationReminders(notificationStrategyReminders.Select(a=> NotificationStrategyReminder.Create(a)));
        }
        
        public NotificationStrategyGenericBuilder WithNotificationReminders(
            params NotificationStrategyReminder[] notificationStrategyReminders)
        {
            return WithNotificationReminders(notificationStrategyReminders.AsEnumerable());
        }


        public NotificationStrategyGenericBuilder WithNotificationReminders(
            IEnumerable<NotificationStrategyReminder> notificationStrategyReminders )
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
            _expirationSpanBeforeEndingDate = expirationSpanBeforeEndingDate ;
            return this;
        }


        //IEnumerable<NotificationStrategyReminder> notificationStrategyReminders

        public NotificationStrategyGeneric Build()
        {
            return NotificationStrategyGeneric.Create(avoidedDayOfWeks:_avoidedDayOfWeks, 
                notificationStrategyWindows:_notificationStrategyWindows);
        }

    } 

    public class NotificationStrategyGeneric : NotificationStrategyBase
    {

        public static NotificationStrategyGeneric Create(IEnumerable<DayOfWeek> avoidedDayOfWeks = null,
                   IEnumerable<NotificationStrategyWindow> notificationStrategyWindows = null,
                   IEnumerable<NotificationStrategyReminder> notificationStrategyReminders = null,
                   TimeSpan expirationSpanAfterInitialDate = default(TimeSpan),
                   TimeSpan expirationSpanBeforeEndingDate = default(TimeSpan))
        {
            return new NotificationStrategyGeneric(avoidedDayOfWeks: avoidedDayOfWeks);
        }

        private NotificationStrategyGeneric(IEnumerable<DayOfWeek> avoidedDayOfWeks = null,
          IEnumerable<NotificationStrategyWindow> notificationStrategyWindows = null,
          IEnumerable<NotificationStrategyReminder> notificationStrategyReminders = null,
          TimeSpan expirationSpanAfterInitialDate = default(TimeSpan),
          TimeSpan expirationSpanBeforeEndingDate = default(TimeSpan)
          )
        {

            init(avoidedDayOfWeks);
        }

        private IEnumerable<DayOfWeek> _avoidedDayOfWeks;
        private IEnumerable<NotificationStrategyWindow> _notificationStrategyWindows;
        private IEnumerable<NotificationStrategyReminder> _notificationStrategyReminders;
        private TimeSpan _expirationSpanAfterInitialDate;
        private TimeSpan _expirationSpanBeforeEndingDate;

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


        private void init(IEnumerable<DayOfWeek> avoidedDayOfWeks = null,
            IEnumerable<NotificationStrategyWindow> notificationStrategyWindows = null,
          IEnumerable<NotificationStrategyReminder> notificationStrategyReminders = null,
          TimeSpan expirationSpanAfterInitialDate = default(TimeSpan),
          TimeSpan expirationSpanBeforeEndingDate = default(TimeSpan))
        {
            _avoidedDayOfWeks = avoidedDayOfWeks;
            _notificationStrategyWindows = notificationStrategyWindows;
            _notificationStrategyReminders = notificationStrategyReminders;
            _expirationSpanAfterInitialDate = expirationSpanAfterInitialDate;
            _expirationSpanBeforeEndingDate = expirationSpanBeforeEndingDate;
        }

    }
    public abstract class NotificationStrategyBase : INotificationStrategy
    {
        public IEnumerable<DayOfWeek> _avoidedDayOfWeks;


        public abstract IEnumerable<DayOfWeek> AvoidedDayOfWeks { get; }
        public abstract IEnumerable<NotificationStrategyWindow> NotificationStrategyWindows { get; }
        public abstract IEnumerable<NotificationStrategyReminder> NotificationStrategyReminders { get; }

        public abstract TimeSpan ExpirationSpanAfterInitialDate { get; }

        public abstract TimeSpan ExpirationSpanBeforeEndingDate { get; }

    }
    public interface INotificationStrategy
    {
        //public bool RetryIfDayIsVoided { get;  }
        public IEnumerable<DayOfWeek> AvoidedDayOfWeks { get; }
        public IEnumerable<NotificationStrategyWindow> NotificationStrategyWindows { get; } // only sent 
        public IEnumerable<NotificationStrategyReminder> NotificationStrategyReminders { get; }
        public TimeSpan ExpirationSpanAfterInitialDate { get; } // Expiration Span after the starting date...
        public TimeSpan ExpirationSpanBeforeEndingDate { get; } //  Expiration Span after the ending date...
    }



    public interface INotificationsProvider
    {
        public IEnumerable<INotificable> Provide();

        public NotificationStatus GetNotificationStatus(string UniqueId);
        public void SetNotified(string UniqueId);
    }
    public class NotificationStatus
    {
        public DateTime LastNotificationDate { get; set; }
        public int AcumulatedNotifications { get; set; }
    }
    public interface INotificable
    {
        public string UniqueId { get; }
        public DateTime Start { get; }
        public DateTime End { get; }
    }

    public interface ISystemDateProvider
    {
        public DateTime Now { get; }
        public DateTime UtcNow => Now.ToUniversalTime();

    }
    public class CurrentDateProvider : ISystemDateProvider
    {
        public DateTime Now => DateTime.Now;
        public DateTime UtcNow => DateTime.UtcNow;
    }
    public abstract class SmartTriggerBase
    {
        private readonly INotificationsProvider _notificationsProvider;
        private readonly INotificationStrategy _notificationStrategy;
        private readonly ISystemDateProvider _systemDateProvider;

        public SmartTriggerBase(INotificationStrategy notificationStrategy,
        INotificationsProvider notificationsProvider,
        ISystemDateProvider systemDateProvider)
        {
            this._notificationStrategy = notificationStrategy;
            this._notificationsProvider = notificationsProvider;
            this._systemDateProvider = systemDateProvider;

        }

        public async IAsyncEnumerable<Tuple<INotificable, NotifcableEvaluation>> GetNotificables()
        {
            foreach (var a in _notificationsProvider.Provide())
            {
                var reason = await shouldNotify(a);

                yield return new Tuple<INotificable, NotifcableEvaluation>(a, reason);
            }
        }
        public enum NotifcableEvaluationReason
        {
            NONE = 0,
            OUTSIDE_DATE_LIMIT = 1,
            VOIDED_DAY_OF_WEEK = 2,
            OUTSIDE_TIME_WINDOW = 3,
            NO_REMINDERS = 3
        }
        public class NotifcableEvaluation
        {
            public static NotifcableEvaluation NotifyTrue() => new NotifcableEvaluation() { Notify = true };
            public static NotifcableEvaluation NotifyFalse(NotifcableEvaluationReason reason)
                => new NotifcableEvaluation() { Notify = false, Reason = reason };
            public bool Notify { get; private set; }
            public NotifcableEvaluationReason Reason { get; private set; }
        }
        private async Task<NotifcableEvaluation> shouldNotify(INotificable notificable)
        {
            var current_date = _systemDateProvider.Now;

            if (!_notificationStrategy.NotificationStrategyReminders.Any())
                return NotifcableEvaluation.NotifyFalse(NotifcableEvaluationReason.NO_REMINDERS);


            // DAY OF WEEK
            if (_notificationStrategy.AvoidedDayOfWeks.Contains(current_date.DayOfWeek))
                return NotifcableEvaluation.NotifyFalse(NotifcableEvaluationReason.VOIDED_DAY_OF_WEEK);

            // TIME WINDOW

            if (!_notificationStrategy.NotificationStrategyWindows.IsInsideWindow(current_date))
                return NotifcableEvaluation.NotifyFalse(NotifcableEvaluationReason.OUTSIDE_TIME_WINDOW);

            // DATE LIMIT

            var start_date_limit = notificable.Start.Add(_notificationStrategy.ExpirationSpanAfterInitialDate);
            var end_date_limit = notificable.End.Add(-1 * _notificationStrategy.ExpirationSpanBeforeEndingDate);

            if (!(current_date >= start_date_limit && current_date <= end_date_limit))
                return NotifcableEvaluation.NotifyFalse(NotifcableEvaluationReason.OUTSIDE_DATE_LIMIT);




            var _last_notification = _notificationsProvider.GetNotificationStatus(notificable.UniqueId);
            var ix = _last_notification?.AcumulatedNotifications ?? 0;
            var current_reminder = _notificationStrategy.NotificationStrategyReminders.OrderBy(a => a.Interval).
                                    Skip(ix).Take(1).FirstOrDefault();

            var _last_notification_date = _last_notification?.LastNotificationDate;

            if (!_last_notification_date.HasValue ||
                _last_notification_date.Value.Add(current_reminder.Interval) > current_date)
            {
                return NotifcableEvaluation.NotifyTrue();
            }

            return NotifcableEvaluation.NotifyFalse(NotifcableEvaluationReason.NONE);


        }
    }

    public enum NotificationInterval
    {

        // Summary:
        //     Day of month (1 through 31)
        Day = 4,
        //
        // Summary:
        //     Hour (0 through 23)
        Hour = 7,
        //
        // Summary:
        //     Minute (0 through 59)
        Minute = 8,
        //
        // Summary:
        //     Month (1 through 12)
        Month = 2,
        //
        // Summary:
        //     Quarter of year (1 through 4)
        Quarter = 1,
        //
        // Summary:
        //     Second (0 through 59)
        Second = 9,
        //
        // Summary:
        //     Day of week (1 through 7)
        Weekday = 6,
        //
        // Summary:
        //     Year
        Year = 0
    }
}
