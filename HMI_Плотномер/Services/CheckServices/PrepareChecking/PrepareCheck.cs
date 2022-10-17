using IDensity.DataAccess;
using IDensity.Services.CheckServices;

namespace IDensity.Core.Services.CheckServices
{
    public class PrepareCheck:Check
    {
         public DeviceCheckResult CheckSysytemErrors { get; } = new DeviceCheckResult();
    }
}
