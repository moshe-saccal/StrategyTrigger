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
        public static NotificationStrategyWindow Create(TimeSpan start = default(TimeSpan),
            TimeSpan end = default(TimeSpan))
        {
            return new NotificationStrategyWindow(start, end);
        }
        private NotificationStrategyWindow(TimeSpan start, TimeSpan end)
        {
            if (end < start) throw new ArgumentException("end", "End must be grather than start");
            if ( (start-end ).TotalHours>24) throw new ArgumentException( "Window cannot we greather than 24hs");

            Start = start;
            End = end;
        }
        public TimeSpan Start { get; private set; }
        public TimeSpan End { get; private set; }
    }
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
    public abstract class NotificationStrategyBase : INotificationStrategy
    {
        public IEnumerable<DayOfWeek> _avoidedDayOfWeks;


        public abstract IEnumerable<DayOfWeek> AvoidedDayOfWeks { get; }
        public abstract IEnumerable<NotificationStrategyWindow> NotificationStrategyWindows { get; }
        public abstract IEnumerable<NotificationStrategyReminder> NotificationStrategyReminders { get; }

        public abstract TimeSpan ExpirationSpanAfterInitialDate { get; }

        public abstract TimeSpan ExpirationSpanBeforeEndingDate { get; }
        public abstract TimeSpan MinSpanBetweenNotifications { get; }



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
        public string NotificationIdentifier { get; }
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

    public class SmartTriggerBuilder
    {

        private INotificationsProvider _notificationsProvider;
        private INotificationStrategy _notificationStrategy;
        private ISystemDateProvider _systemDateProvider;
        public SmartTriggerBuilder WithNotificationsProvider(INotificationsProvider notificationsProvider)
        {
            _notificationsProvider = notificationsProvider;
            return this;
        }
        public SmartTriggerBuilder WithNotificationStrategy(INotificationStrategy notificationsStrategy)
        {
            _notificationStrategy = notificationsStrategy;
            return this;
        }
        public SmartTriggerBuilder WithSystemDateProvider<T>() where T : ISystemDateProvider
        {
            return WithSystemDateProvider(Activator.CreateInstance<T>());
        }


        public SmartTriggerBuilder WithSystemDateProvider(ISystemDateProvider systemDateProvider)
        {
            _systemDateProvider = systemDateProvider;
            return this;
        }
        public static SmartTriggerBuilder Create() => new SmartTriggerBuilder();

        public SmartTriggerBase Build()
        {
            return SmartTriggerBase.Create(
                notificationStrategy: _notificationStrategy,
                notificationsProvider: _notificationsProvider,
                systemDateProvider: _systemDateProvider);
        }

    }

    public sealed class SmartTriggerBase
    {
        private readonly INotificationsProvider _notificationsProvider;
        private readonly INotificationStrategy _notificationStrategy;
        private readonly ISystemDateProvider _systemDateProvider;


        public static SmartTriggerBase Create(INotificationStrategy notificationStrategy,
        INotificationsProvider notificationsProvider,
        ISystemDateProvider systemDateProvider)
        {
            return new SmartTriggerBase(notificationStrategy, notificationsProvider, systemDateProvider);
        }

        private SmartTriggerBase(INotificationStrategy notificationStrategy,
        INotificationsProvider notificationsProvider,
        ISystemDateProvider systemDateProvider)
        {
            this._notificationStrategy = notificationStrategy;
            this._notificationsProvider = notificationsProvider;
            this._systemDateProvider = systemDateProvider;

        }

        public class NotificableEvaluationResult
        {
            public static NotificableEvaluationResult Create(INotificable notificable, NotifcableEvaluationResult result)
            {
                return new NotificableEvaluationResult(notificable, result);
            }
            private NotificableEvaluationResult(INotificable notificable, NotifcableEvaluationResult result)
            {
                Notificable = notificable;
                Result = result;

            }
            public INotificable Notificable { get; private set; }
            public NotifcableEvaluationResult Result { get; private set; }
        }
        public async IAsyncEnumerable<NotificableEvaluationResult> EvaluateNotificables()
        {
            foreach (var a in _notificationsProvider.Provide())
            {
                var result = await shouldNotify(a);

                yield return NotificableEvaluationResult.Create(a, result);
            }
        }
        public enum NotifcableEvaluationResult
        {
            NOTIFY = 0,
            DONT_NOTIFY_OUTSIDE_DATE_LIMIT = 1,
            DONT_NOTIFY_VOIDED_DAY_OF_WEEK = 2,
            DONT_NOTIFY_OUTSIDE_TIME_WINDOW = 3,
            DONT_NOTIFY_OTHERS = 5,
            DONT_NOTIFY_OUTSIDE_INTERVAL = 6,

        }


        /// <summary>
        /// Evaluates notification 
        /// Order of evaluation: 
        ///  - Valid Day of Week ( AvoidedDayOfWeks )
        ///  - Time Window ( NotificationStrategyWindows )
        ///  - Date Limit ( ExpirationSpanAfterInitialDate - ExpirationSpanBeforeEndingDate ) 
        ///  - Reminders ( NotificationStrategyReminders )
        /// </summary>
        /// <param name="notificable"></param>
        /// <returns> NotifcableEvaluationResult</returns>        
        private async Task<NotifcableEvaluationResult> shouldNotify(INotificable notificable)
        {
            var current_date = _systemDateProvider.Now;

            // DAY OF WEEK
            if (_notificationStrategy.AvoidedDayOfWeks.Contains(current_date.DayOfWeek))
                return NotifcableEvaluationResult.DONT_NOTIFY_VOIDED_DAY_OF_WEEK;

            // TIME WINDOW

            if (!_notificationStrategy.NotificationStrategyWindows.IsInsideWindow(current_date))
                return NotifcableEvaluationResult.DONT_NOTIFY_OUTSIDE_TIME_WINDOW;

            // DATE LIMIT

            var start_date_limit = notificable.Start.Add(_notificationStrategy.ExpirationSpanAfterInitialDate);
            var end_date_limit = notificable.End.Add(-1 * _notificationStrategy.ExpirationSpanBeforeEndingDate);

            if (!(current_date >= start_date_limit && current_date < end_date_limit))
                return NotifcableEvaluationResult.DONT_NOTIFY_OUTSIDE_DATE_LIMIT;


            // REMINDERS

            var _last_notification = _notificationsProvider.GetNotificationStatus(notificable.NotificationIdentifier);
            var ix = _last_notification?.AcumulatedNotifications ?? 0;
            var current_reminder_seconds = _notificationStrategy.NotificationStrategyReminders.OrderBy(a => a.Interval).
                                    Skip(ix).Take(1).FirstOrDefault()?.Interval.TotalSeconds ?? 0;


            var past_reminders_acumulated_seconds = _notificationStrategy.NotificationStrategyReminders.OrderBy(a => a.Interval).
                                    Take(ix).Sum(x => x.Interval.TotalSeconds);



            var _last_notification_date = _last_notification?.LastNotificationDate;

            if (!_last_notification_date.HasValue ||
                _last_notification_date.Value
                    .AddSeconds(past_reminders_acumulated_seconds)
                    .AddSeconds(current_reminder_seconds) <= current_date)
            {
                return NotifcableEvaluationResult.NOTIFY;
            }
            else
            {
                return NotifcableEvaluationResult.DONT_NOTIFY_OUTSIDE_INTERVAL;
            }

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
