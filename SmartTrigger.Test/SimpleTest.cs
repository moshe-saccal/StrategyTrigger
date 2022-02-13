using NUnit.Framework;
using SmartTrigger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            public DateTime Now { get; set; }
        }





        [Test]
        public async Task Test_Simple_To_Early()
        {
            var _date = new ChangeableDateProvider();
            _date.Now = DateTime.Now;

            var _items = new List<SimpleTestNotificable>() { SimpleTestNotificable.Create("test1", _date.Now.AddDays(-1), _date.Now) };
            var _notificator = CreateTestNotificator(_items, _date);
            var _all = _notificator.GetNotificables().GetAsyncEnumerator().ReadAll();


        }
        private SimpleTestNotificator CreateTestNotificator(IEnumerable<SimpleTestNotificable> items,
            ISystemDateProvider provider)
        => new SimpleTestNotificator(new SimpleTestNotificationProvider(items, provider), provider);



        [Test]
        public async Task TestSimpleUseCaseWithManyRandomItems()
        {
            var _items = GenerateItems(DateTime.Now.AddDays(-100), DateTime.Now.AddDays(100), 100);

            var _date = new ChangeableDateProvider();
            var cur_date = DateTime.Now.AddDays(-1).AddHours(-5);
            _date.Now = cur_date;

            var b = NotificationStrategyGenericBuilder.Create()
                .WithAvoidedDaysOfWeeks(DayOfWeek.Saturday, DayOfWeek.Sunday)
                .WithNotificationWindows(NotificationStrategyWindow.Create(TimeSpan.FromHours(10), TimeSpan.FromHours(20)))
                .WithNotificationReminders(TimeSpan.FromHours(1), TimeSpan.FromHours(2))
                .WithExpirationSpanAfterInitialDate(TimeSpan.FromHours(2))
                .WithExpirationSpanBeforeEndingDate(TimeSpan.FromHours(2)).Build();

                

            var pac = new SimpleTestNotificator(new SimpleTestNotificationProvider(_items, _date), _date);


            var all = await pac.GetNotificables().GetAsyncEnumerator().ReadAll();

            //var all =await res.ReadAll();

            all.ToList().ForEach(x =>
            {
                System.Diagnostics.Debug.WriteLine(x.Item2.Reason.ToString());
                TestContext.Out.WriteLine(x.Item2.Notify.ToString() + "->" + x.Item1.Start.ToString() + "->" + x.Item2.Reason.ToString());
            });
            Assert.IsTrue(all.Count() == _items.Count());
            Assert.IsTrue(all.Where(a => a.Item2.Notify).Count() < _items.Count() / 2);


        }
    }
}

public class WeekDays_10_to_20_Every_Hour_NotificationStrategy : INotificationStrategy
{

    private Bsae
    
}



public class SimpleTestNotificable : INotificable
{
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
public class SimpleTestNotificationProvider : INotificationsProvider
{
    private IEnumerable<SimpleTestNotificable> _data;
    private SortedDictionary<string, NotificationStatus> _hiist = new SortedDictionary<string, NotificationStatus>();
    private ISystemDateProvider systemDateProvider;
    public SimpleTestNotificationProvider(IEnumerable<SimpleTestNotificable> data,
        ISystemDateProvider systemDateProvider)
    {
        _data = data;
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
                LastNotificationDate = systemDateProvider.Now
            });

        }
        else
        {
            current.AcumulatedNotifications++;
            current.LastNotificationDate = systemDateProvider.Now;
        }

        //throw new NotImplementedException();
    }

    public NotificationStatus GetNotificationStatus(string UniqueId)
    {
        return (_hiist.Where(x => x.Key == UniqueId)?.FirstOrDefault())?.Value;
        // return current?.Value;
    }

}
