using SmartTrigger.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartTrigger.Test
{

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
}
