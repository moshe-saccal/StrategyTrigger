using SmartTrigger.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartTrigger.Test.Util
{
    public class ChangeableDateProvider : ISystemDateProvider
    {
        public static ChangeableDateProvider Create(DateTime current)
        {
            return new ChangeableDateProvider(current);

        }
        public static ChangeableDateProvider Create()
        {
            return new ChangeableDateProvider();
        }
        public ChangeableDateProvider()
        {
            Now = DateTime.Now;
        }
        public ChangeableDateProvider(DateTime current)
        {
            Now = current;
        }
        public void AddDays(int d)
        {
            Now = Now.AddDays(d);
        }
        public void AddMinutes(int m)
        {
            Now = Now.AddMinutes(m);
        }
        public void AddHours(int h)
        {
            Now = Now.AddHours(h);
        }

        public void SetDate(DateTime currentDate)
        {
            Now = currentDate;
        }
        public DateTime Now { get; set; }
    }
}
