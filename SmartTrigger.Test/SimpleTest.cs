using NUnit.Framework;
using SmartTrigger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static SmartTrigger.SmartTriggerBase;

namespace SmartTrigger.Test
{
    public static class helpext
    {
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
                    UniqueId = System.Guid.NewGuid().ToString(),
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




        private IEnumerable<NotificableEvaluationResult> Evaluate(INotificationStrategy notificationStrategy,
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
            return result;
        }
        [Test]
        public async Task Test_Outside_Time_Of_Day()
        {
            
            var next_mon_11am = DateTime.Now.AddDays(Math.Abs(3 - (int)DateTime.Now.DayOfWeek));
            
            var _date = ChangeableDateProvider.Create( DateTime.Now );
            
            var result = Evaluate(Strategy_MonFri_First2WorkingHours_2DaysEdge,
                   _date,
                   SimpleTestNotificable.Create(_date.Now.AddDays(-1)));
            Assert.True(result.FirstOrDefault().Result.Equals(NotifcableEvaluationResult.DONT_NOTIFY_VOIDED_DAY_OF_WEEK));
        }

        [Test]
        public async Task Test_Voided_Day_Of_Week()
        {

            var next_sunday = DateTime.Now.AddDays(Math.Abs(7 - (int)DateTime.Now.DayOfWeek));
            var _date = ChangeableDateProvider.Create(next_sunday);
            var result = Evaluate(Strategy_MonFri_First2WorkingHours_2DaysEdge,
                   _date,
                   SimpleTestNotificable.Create(_date.Now.AddDays(-10)));
            Assert.True(result.FirstOrDefault().Result.Equals(NotifcableEvaluationResult.DONT_NOTIFY_VOIDED_DAY_OF_WEEK));
        }




        [Test]
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

    public static SimpleTestNotificable Create(string uniqueId, DateTime start, DateTime end)
    {
        return new SimpleTestNotificable()
        {
            UniqueId = uniqueId,
            Start = start,
            End = end
        };


    }


    public string UniqueId { get; set; }
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
        _data = data;
        _systemDateProvider = systemDateProvider;
    }

    public void SetProvide(IEnumerable<SimpleTestNotificable> data)
    {
        _data = data;
    }
    public IEnumerable<INotificable> Provide()
    {
        return _data;

    }

    public void SetNotified(string UniqueId)
    {
        var current = _hiist[UniqueId];
        if (current == null)
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
