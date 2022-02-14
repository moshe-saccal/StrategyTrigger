using System;
using System.Collections.Generic;
using System.Text;

namespace SmartTrigger.Interfaces
{
    public interface INotificable
    {
        public string NotificationIdentifier { get; }
        public DateTime Start { get; }
        public DateTime End { get; }
    }
}
