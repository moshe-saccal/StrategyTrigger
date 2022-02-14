using SmartTrigger.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartTrigger.Models
{
    public class NotificableEvaluationResult
    {
        public static NotificableEvaluationResult Create(INotificable notificable, NotifcableEvaluationResult result)
        {
            return new NotificableEvaluationResult(notificable, result);
        }
        private NotificableEvaluationResult(INotificable notificable, NotifcableEvaluationResult result)
        {
            Notificable = notificable;
            Result = result;

        }
        public INotificable Notificable { get; private set; }
        public NotifcableEvaluationResult Result { get; private set; }
    }
}
