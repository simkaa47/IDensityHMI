using System;
using System.Collections.Generic;
using System.Text;

namespace IDensity.Views.SettingsMaster.ConnectionMaster
{
    public enum StateConnectionMaster
    {
        Start,
        ConnectionReady,
        SearchInterfaces,
        ScaningIP,
        WaitingForConnect,
        FailureSearch,
        NoInterfaces,
        Exit
    };
}
