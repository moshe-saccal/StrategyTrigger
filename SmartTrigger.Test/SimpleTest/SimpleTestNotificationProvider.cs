using SmartTrigger.Interfaces;
using SmartTrigger.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartTrigger.Test
{


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
            NotificationStatus current;
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

}
