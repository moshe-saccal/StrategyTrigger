using SmartTrigger.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartTrigger.Models
{
    public class CurrentDateProvider : ISystemDateProvider
    {
        public DateTime Now => DateTime.Now;
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
