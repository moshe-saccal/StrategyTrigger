using Microsoft.VisualBasic;
using SmartTrigger.Interfaces;
using SmartTrigger.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartTrigger.Extensions;

namespace SmartTrigger.Models
{ 
    public   sealed class SmartTriggerBase
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

        
        public async IAsyncEnumerable<NotificableEvaluationResult> EvaluateNotificables()
        {
            foreach (var a in _notificationsProvider.Provide())
            {
                var result = await shouldNotify(a);

                yield return NotificableEvaluationResult.Create(a, result);
            }
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

            var start_date_limit = notificable.Start.Add(_notificationStrategy.SpanAfterInitialDate);
            var end_date_limit = notificable.End.Add(-1 * _notificationStrategy.SpanBeforeEndingDate);

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

 
}
