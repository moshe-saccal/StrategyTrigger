using NUnit.Framework;
using SmartTrigger;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using static SmartTrigger.SmartTriggerBase;

namespace SmartTrigger.Test
{
    public static class helpext
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

    public class Tests
    {

        [SetUp]
        public void Setup()
        {


        }
        public DateTime GenerateRandomDate(DateTime startDate, DateTime endDate)
        {
            var randomTest = new Random();
            TimeSpan timeSpan = endDate - startDate;
            TimeSpan newSpan = new TimeSpan(0, randomTest.Next(0, (int)timeSpan.TotalMinutes), 0);
            return startDate + newSpan;
        }
        public List<SimpleTestNotificable> GenerateItems(DateTime startDate, DateTime endDate, int total = 4)
        {
            var lis = new List<SimpleTestNotificable>();
            for (int i = 0; i < total - 1; i++)
            {
                var _start_date = GenerateRandomDate(startDate, endDate);
                var _end_date = GenerateRandomDate(_start_date, endDate);

                lis.Add(new SimpleTestNotificable()
                {
                    NotificationIdentifier = System.Guid.NewGuid().ToString(),
                    Start = _start_date,
                    End = _end_date
                });
            }
            return lis;
        }
        public class ChangeableDateProvider : ISystemDateProvider
        {
            public static ChangeableDateProvider Create(DateTime current)
            {
                return new ChangeableDateProvider(current);

            }
            public static ChangeableDateProvider Create()
            {
                return new ChangeableDateProvider();
            }
            public ChangeableDateProvider()
            {
                Now = DateTime.Now;
            }
            public ChangeableDateProvider(DateTime current)
            {
                Now = current;
            }
            public void AddDays(int d)
            {
                Now = Now.AddDays(d);
            }
            public void AddMinutes(int m)
            {
                Now = Now.AddMinutes(m);
            }
            public void AddHours(int h)
            {
                Now = Now.AddHours(h);
            }

            public void SetDate(DateTime currentDate)
            {
                Now = currentDate;
            }
            public DateTime Now { get; set; }
        }

        public static INotificationStrategy Strategy_MonFri_First2WorkingHours_2DaysEdge =>
            NotificationStrategyGenericBuilder.Create()
                .WithAvoidedDaysOfWeeks(DayOfWeek.Saturday, DayOfWeek.Sunday)
                .WithNotificationWindows(NotificationStrategyWindow.Create(TimeSpan.FromHours(10), TimeSpan.FromHours(20)))
                .WithNotificationReminders(TimeSpan.FromHours(1), TimeSpan.FromHours(2))
                .WithExpirationSpanAfterInitialDate(TimeSpan.FromDays(2))
                .WithExpirationSpanBeforeEndingDate(TimeSpan.FromDays(2))
                .Build();




        private (IEnumerable<NotificableEvaluationResult>, INotificationsProvider, SmartTriggerBase) Evaluate(
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



        [Test]
        public async Task Test_Notificable_1stNotificationOk_2ndOmitted_3rd_ok()
        {

            var thu_11am = DateTime.Now.Next(DayOfWeek.Thursday).
                SetTimeOfDay(TimeSpan.ParseExact("11:01:00", "g", DateTimeFormatInfo.InvariantInfo));
            

            var tues_11am = DateTime.Now.Next(DayOfWeek.Tuesday).
                SetTimeOfDay(TimeSpan.ParseExact("11:00:00", "g", DateTimeFormatInfo.InvariantInfo));
             

            var thu_1259am = thu_11am.AddHours(1).AddMinutes(59);
            var thu_5pm = thu_11am.AddHours(6);

            var _date = ChangeableDateProvider.Create(thu_11am);



            var (result, provider, smart_trigger) = Evaluate(Strategy_MonFri_First2WorkingHours_2DaysEdge,
                   _date,
                   SimpleTestNotificable.Create("notif_user1234", tues_11am
                   ));

           
            Assert.True(result.FirstOrDefault().Result.Equals(NotifcableEvaluationResult.NOTIFY)); 
            ((SimpleTestNotificationProvider)provider).SetNotified("notif_user1234"); 
            _date.SetDate(thu_1259am); 
            var second_result = await smart_trigger.EvaluateNotificables().GetAsyncEnumerator().ReadAll(); 
            Assert.IsTrue(second_result.FirstOrDefault().Result.Equals(NotifcableEvaluationResult.DONT_NOTIFY_OUTSIDE_INTERVAL)); 
            _date.SetDate(thu_5pm); 
            var third_result = await smart_trigger.EvaluateNotificables().GetAsyncEnumerator().ReadAll(); 
            Assert.IsTrue(third_result.FirstOrDefault().Result.Equals(NotifcableEvaluationResult.NOTIFY)); 
        }




        [Test]
        public async Task Test_Too_Close_To_Creation()
        {

            var next_mon_11am = DateTime.Now.Next(DayOfWeek.Monday).
                SetTimeOfDay(TimeSpan.ParseExact("11:00:00", "g", DateTimeFormatInfo.InvariantInfo));

            var _date = ChangeableDateProvider.Create(next_mon_11am);

            var (result, _, _) = Evaluate(Strategy_MonFri_First2WorkingHours_2DaysEdge,
                   _date,
                   SimpleTestNotificable.Create(_date.Now.AddMinutes(1))); // creating 1 min after the init period


            Assert.True(result.FirstOrDefault().Result.Equals(NotifcableEvaluationResult.DONT_NOTIFY_OUTSIDE_DATE_LIMIT));
        }

        [Test]
        public async Task Test_Outside_Time_Of_Day()
        {

            var next_mon_11am = DateTime.Now.Next(DayOfWeek.Monday).
                SetTimeOfDay(TimeSpan.ParseExact("9:00:00", "g", DateTimeFormatInfo.InvariantInfo));

            var _date = ChangeableDateProvider.Create(next_mon_11am);

            var (result, _, _) = Evaluate(Strategy_MonFri_First2WorkingHours_2DaysEdge,
                   _date,
                   SimpleTestNotificable.Create(_date.Now.AddDays(-1)));

            Assert.True(result.FirstOrDefault().Result.Equals(NotifcableEvaluationResult.DONT_NOTIFY_OUTSIDE_TIME_WINDOW));
        }

        [Test]
        public async Task Test_Voided_Day_Of_Week()
        {

            var next_sunday = DateTime.Now.Next(DayOfWeek.Sunday);
            var _date = ChangeableDateProvider.Create(next_sunday);
            var (result, _, _) = Evaluate(Strategy_MonFri_First2WorkingHours_2DaysEdge,
                   _date,
                   SimpleTestNotificable.Create(_date.Now.AddDays(-10)));
            Assert.True(result.FirstOrDefault().Result.Equals(NotifcableEvaluationResult.DONT_NOTIFY_VOIDED_DAY_OF_WEEK));
        }




        public async Task TestSimpleUseCaseWithManyRandomItems()
        {
            try
            {

                var _date = new ChangeableDateProvider();
                var cur_date = DateTime.Now.AddDays(-1).AddHours(-5);
                _date.Now = cur_date;

                var _items = GenerateItems(cur_date.AddDays(-100), cur_date.AddDays(100), 100);


                var _strategy = NotificationStrategyGenericBuilder.Create()
                    .WithAvoidedDaysOfWeeks(DayOfWeek.Saturday, DayOfWeek.Sunday)
                    .WithNotificationWindows(NotificationStrategyWindow.Create(TimeSpan.FromHours(10), TimeSpan.FromHours(20)))
                    .WithNotificationReminders(TimeSpan.FromHours(1), TimeSpan.FromHours(2))
                    .WithExpirationSpanAfterInitialDate(TimeSpan.FromHours(2))
                    .WithExpirationSpanBeforeEndingDate(TimeSpan.FromHours(2))
                    .Build();

                var _provider = SimpleTestNotificationProviderBuilder.Create()
                    .WithSystemDateProvider(_date)
                    .WithSimpleTestNotificables(_items)
                    .Build();

                var pac = SmartTriggerBuilder.Create()
                    .WithNotificationStrategy(_strategy)
                    .WithSystemDateProvider(_date)
                    .WithNotificationsProvider(_provider)
                    .Build();



                var all = await pac.EvaluateNotificables().GetAsyncEnumerator().ReadAll();

                //var all =await res.ReadAll();


                Assert.IsTrue(all.Count() < _items.Count());

            }
            catch (Exception ex)
            {
                ex.ToString();
            }

        }
    }
}




public class SimpleTestNotificable : INotificable
{
    public static SimpleTestNotificable Create(DateTime start)
    {
        return Create(System.Guid.NewGuid().ToString(), start, DateTime.MaxValue);
    }

    public static SimpleTestNotificable Create(string notificationIdentifier, DateTime start)
    {
        return Create(notificationIdentifier, start, DateTime.MaxValue);

    }
    public static SimpleTestNotificable Create(string notificationIdentifier, DateTime start, DateTime end)
    {
        return new SimpleTestNotificable()
        {
            NotificationIdentifier = notificationIdentifier,
            Start = start,
            End = end
        };


    }


    public string NotificationIdentifier { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }

}
public class SimpleTestNotificationProviderBuilder
{
    private IEnumerable<SimpleTestNotificable> _data;
    private ISystemDateProvider _systemDateProvider;
    private SimpleTestNotificationProviderBuilder()
    {

    }

    public SimpleTestNotificationProviderBuilder WithSystemDateProvider<T>() where T : ISystemDateProvider
    {
        return WithSystemDateProvider(Activator.CreateInstance<T>());
    }
    public SimpleTestNotificationProviderBuilder WithSystemDateProvider(ISystemDateProvider systemDateProvider)
    {
        _systemDateProvider = systemDateProvider;
        return this;
    }

    public SimpleTestNotificationProviderBuilder WithSimpleTestNotificables(params SimpleTestNotificable[] data)
    {
        return WithSimpleTestNotificables(data.AsEnumerable());
    }

    public SimpleTestNotificationProviderBuilder WithSimpleTestNotificables(IEnumerable<SimpleTestNotificable> data)
    {
        _data = data;
        return this;
    }

    public static SimpleTestNotificationProviderBuilder Create()
    {
        return new SimpleTestNotificationProviderBuilder();
    }
    public SimpleTestNotificationProvider Build()
    {
        return new SimpleTestNotificationProvider(_data, _systemDateProvider);
    }
}
public class SimpleTestNotificationProvider : INotificationsProvider
{
    private IEnumerable<SimpleTestNotificable> _data;
    private SortedDictionary<string, NotificationStatus> _hiist = new SortedDictionary<string, NotificationStatus>();
    private ISystemDateProvider _systemDateProvider;
    public SimpleTestNotificationProvider(IEnumerable<SimpleTestNotificable> data,
        ISystemDateProvider systemDateProvider)
    {
        _systemDateProvider = systemDateProvider;
        SetProvidedData(data);

    }

    public void SetProvidedData(params SimpleTestNotificable[] data)
        => SetProvidedData(data.AsEnumerable());
    public void SetProvidedData(IEnumerable<SimpleTestNotificable> data)
        => _data = data;

    public IEnumerable<INotificable> Provide()
        => _data;

    public void SetNotified(string UniqueId)
    {
        NotificationStatus current ;
        if (!_hiist.TryGetValue(UniqueId, out current))
        {
            _hiist.Add(UniqueId, new NotificationStatus()
            {
                AcumulatedNotifications = 1,
                LastNotificationDate = _systemDateProvider.Now
            });

        }
        else
        {
            current.AcumulatedNotifications++;
            current.LastNotificationDate = _systemDateProvider.Now;
        }

        //throw new NotImplementedException();
    }

    public NotificationStatus GetNotificationStatus(string UniqueId)
    {
        return (_hiist.Where(x => x.Key == UniqueId)?.FirstOrDefault())?.Value;
        // return current?.Value;
    }

}
