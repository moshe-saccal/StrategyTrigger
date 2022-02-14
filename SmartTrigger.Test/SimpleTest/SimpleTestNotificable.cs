using SmartTrigger.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartTrigger.Test
{
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
}
