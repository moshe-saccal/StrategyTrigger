using SmartTrigger.Models;
using System;
using System.Collections.Generic;

namespace SmartTrigger.Extensions
{
    public static class SmartTriggerInternalExtensions
    {
        public static bool IsInsideWindow(this IEnumerable<NotificationStrategyWindow> windows,
            DateTime current)
        {
            foreach (var w in windows)
            {
                if (current.TimeOfDay > w.Start && current.TimeOfDay <= w.End)
                    return true;
            }
            return false;
        }
    }
}
