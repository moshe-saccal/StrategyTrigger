using NUnit.Framework;
 
using SmartTrigger.Builders;
using SmartTrigger.Interfaces;
using SmartTrigger.Models;
using SmartTrigger.NotificationStrategyGeneric; 
using SmartTrigger.Test.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks; 

namespace SmartTrigger.Test
{
    

    public class SimpleTest
    {

        [SetUp]
        public void Setup()
        {
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



            var (result, provider, smart_trigger) = StrategyHelper.Evaluate(StrategyHelper.Strategy_MonFri_First2WorkingHours_2DaysEdge,
                   _date,SimpleTestNotificable.Create("notif_user1234", tues_11am));

           
            Assert.True(result.FirstOrDefault().Result.Equals(NotifcableEvaluationResult.NOTIFY)); 
            ((SimpleTestNotificationProvider)provider).SetNotified("notif_user1234");


            // thrusday 12.59 am its only 1.59 hs after the first one...
            // its outside the interval...  INTERVAL = 2HS

            _date.SetDate(thu_1259am); 
            var second_result = await smart_trigger.EvaluateNotificables().GetAsyncEnumerator().ReadAll(); 
            Assert.IsTrue(second_result.FirstOrDefault().Result.Equals(NotifcableEvaluationResult.DONT_NOTIFY_OUTSIDE_INTERVAL));

            // thrusday 5 pm should notify... 

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

            var (result, _, _) = StrategyHelper.Evaluate(StrategyHelper.Strategy_MonFri_First2WorkingHours_2DaysEdge,
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

            var (result, _, _) = StrategyHelper.Evaluate(StrategyHelper.Strategy_MonFri_First2WorkingHours_2DaysEdge,
                   _date,
                   SimpleTestNotificable.Create(_date.Now.AddDays(-1)));

            Assert.True(result.FirstOrDefault().Result.Equals(NotifcableEvaluationResult.DONT_NOTIFY_OUTSIDE_TIME_WINDOW));
        }

        [Test]
        public async Task Test_Voided_Day_Of_Week()
        {

            var next_sunday = DateTime.Now.Next(DayOfWeek.Sunday);
            var _date = ChangeableDateProvider.Create(next_sunday);
            var (result, _, _) = StrategyHelper.Evaluate(StrategyHelper.Strategy_MonFri_First2WorkingHours_2DaysEdge,
                   _date,
                   SimpleTestNotificable.Create(_date.Now.AddDays(-10)));
            Assert.True(result.FirstOrDefault().Result.Equals(NotifcableEvaluationResult.DONT_NOTIFY_VOIDED_DAY_OF_WEEK));
        }


        [Test]
        public async Task Test_Develop()
        {
            try
            {

                var _date = new ChangeableDateProvider();
                var cur_date = DateTime.Now.AddDays(-1).AddHours(-5);
                _date.Now = cur_date;

                var _items = NotificationItemsHelper.GenerateItems(cur_date.AddDays(-100), cur_date.AddDays(100), 100);


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

                Assert.IsTrue(1==1);


            }
            catch (Exception ex)
            {
                ex.ToString();
            }

        }
    }
}



