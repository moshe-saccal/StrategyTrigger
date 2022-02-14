using SmartTrigger.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartTrigger.Interfaces
{
    public interface INotificationsProvider
    {
        public IEnumerable<INotificable> Provide();

        public NotificationStatus GetNotificationStatus(string UniqueId);
        public void SetNotified(string UniqueId);
    }
}
