using System;
using System.Collections.Generic;
using System.Text;

namespace SmartTrigger.Interfaces
{
    public interface ISystemDateProvider
    {
        public DateTime Now { get; }
        public DateTime UtcNow => Now.ToUniversalTime();

    }
}
