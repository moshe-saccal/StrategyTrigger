using SmartTrigger.Interfaces;
using SmartTrigger.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartTrigger.Builders
{
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
}
