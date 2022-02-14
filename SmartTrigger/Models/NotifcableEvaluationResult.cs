using System;
using System.Collections.Generic;
using System.Text;

namespace SmartTrigger.Models
{
    public enum NotifcableEvaluationResult
    {
        NOTIFY = 0,
        DONT_NOTIFY_OUTSIDE_DATE_LIMIT = 1,
        DONT_NOTIFY_VOIDED_DAY_OF_WEEK = 2,
        DONT_NOTIFY_OUTSIDE_TIME_WINDOW = 3,
        DONT_NOTIFY_OTHERS = 5,
        DONT_NOTIFY_OUTSIDE_INTERVAL = 6,

    }
}
