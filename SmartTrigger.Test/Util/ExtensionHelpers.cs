using SmartTrigger.Builders;
using SmartTrigger.Interfaces;
using SmartTrigger.Models;
using SmartTrigger.NotificationStrategyGeneric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTrigger.Test.Util
{
    
    public class StrategyHelper
    {


        public static INotificationStrategy Strategy_MonFri_First2WorkingHours_2DaysEdge =>
            NotificationStrategyGenericBuilder.Create()
                .WithAvoidedDaysOfWeeks(DayOfWeek.Saturday, DayOfWeek.Sunday)
                .WithNotificationWindows(NotificationStrategyWindow.Create(TimeSpan.FromHours(10), TimeSpan.FromHours(20)))
                .WithNotificationReminders(TimeSpan.FromHours(1), TimeSpan.FromHours(2))
                .WithExpirationSpanAfterInitialDate(TimeSpan.FromDays(2))
                .WithExpirationSpanBeforeEndingDate(TimeSpan.FromDays(2))
                .Build();





        public static (IEnumerable<NotificableEvaluationResult>, INotificationsProvider, SmartTriggerBase)  Evaluate(
            INotificationStrategy notificationStrategy,
            ISystemDateProvider dateProvider,
             params SimpleTestNotificable[] notificables)
        {

            var _provider = SimpleTestNotificationProviderBuilder.Create()
                .WithSystemDateProvider(dateProvider)
                .WithSimpleTestNotificables(notificables.AsEnumerable())
                .Build();

            var smart_trigger = SmartTriggerBuilder.Create()
                .WithNotificationStrategy(Strategy_MonFri_First2WorkingHours_2DaysEdge)
                .WithSystemDateProvider(dateProvider)
                .WithNotificationsProvider(_provider)
                .Build();

            var result = smart_trigger.EvaluateNotificables().GetAsyncEnumerator().ReadAll().GetAwaiter().GetResult();
            return (result, _provider, smart_trigger);
        }

    }
    public class NotificationItemsHelper
    {
        public static List<SimpleTestNotificable> GenerateItems(DateTime startDate, DateTime endDate, int total = 4)
        {
            var lis = new List<SimpleTestNotificable>();
            for (int i = 0; i < total - 1; i++)
            {
                var _start_date = DateHelper.GenerateRandomDate(startDate, endDate);
                var _end_date = DateHelper.GenerateRandomDate(_start_date, endDate);

                lis.Add(new SimpleTestNotificable()
                {
                    NotificationIdentifier = System.Guid.NewGuid().ToString(),
                    Start = _start_date,
                    End = _end_date
                });
            }
            return lis;
        }


    }
    public class DateHelper
    {
        public static DateTime GenerateRandomDate(DateTime startDate, DateTime endDate)
        {
            var randomTest = new Random();
            TimeSpan timeSpan = endDate - startDate;
            TimeSpan newSpan = new TimeSpan(0, randomTest.Next(0, (int)timeSpan.TotalMinutes), 0);
            return startDate + newSpan;
        }
    }
    public static class ExtensionHelpers
    {
       

        public static DateTime SetTimeOfDay(this DateTime from, TimeSpan timeOfDay)
        {
            return from.AddHours(timeOfDay.Hours - from.Hour)
                .AddMinutes(timeOfDay.Minutes - from.Minute)
                .AddSeconds(timeOfDay.Seconds - from.Second);

        }
        public static DateTime Next(this DateTime from, DayOfWeek dayOfWeek)
        {
            int start = (int)from.DayOfWeek;
            int target = (int)dayOfWeek;
            if (target <= start)
                target += 7;
            return from.AddDays(target - start);
        }


        public static async Task<IEnumerable<T>> ReadAll<T>(this IAsyncEnumerator<T> enu)
        {
            var lis = new List<T>();

            while (await enu.MoveNextAsync())
            {
                lis.Add(enu.Current);
            }

            return lis;
        }


    }
}
