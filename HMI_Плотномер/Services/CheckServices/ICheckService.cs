using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IDensity.Services.CheckServices
{
    public interface ICheckService
    {
        public event Action<string> ProcessEvent;
        public  Task<List<DeviceCheckResult>> Check();        
        
    }
}
