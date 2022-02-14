using System;
using System.Collections.Generic;
using System.Text;

namespace SmartTrigger.Models
{
    public class NotificationStatus
    {
        public DateTime LastNotificationDate { get; set; }
        public int AcumulatedNotifications { get; set; }
    }

}
