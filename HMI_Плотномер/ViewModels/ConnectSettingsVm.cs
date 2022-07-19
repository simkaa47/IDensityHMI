using IDensity.AddClasses;
using IDensity.ViewModels.Commands;
using System;
using System.Linq;

namespace IDensity.ViewModels
{
    public class ConnectSettingsVm : PropertyChangedBase
    {
        public ConnectSettingsVm(VM vM)
        {
            VM = vM;
        }

        public VM VM { get; }

        #region Команды
        #region Команды настроек измерительного порта
        #region Сменить режим порта 
        RelayCommand _changeSerialSelectCommand;
        public RelayCommand ChangeSerialSelectCommand => 
            _changeSerialSelectCommand ?? 
            (_changeSerialSelectCommand = new RelayCommand(o => VM.CommService.ChangeSerialSelect((int)o), o => o != null));
        #endregion

        #region Сменить баудрейт 
        RelayCommand _changeBaudrateCommand;
        public RelayCommand ChangeBaudrateCommand => 
            _changeBaudrateCommand ?? 
            (_changeBaudrateCommand = new RelayCommand(o => VM.CommService.ChangeBaudrate((uint)(o)), o => o != null));
        #endregion
        #endregion      

        #region Команда "Установить настроку IP приемника UDP даных
        RelayCommand _setUpsAddrCommand;
        public RelayCommand SetUpsAddrCommand => _setUpsAddrCommand ?? (_setUpsAddrCommand = new RelayCommand(execPar =>
        {
            byte num = 0;
            var nums = (VM.mainModel.UdpAddrString.Split(".", StringSplitOptions.RemoveEmptyEntries)).Where(s => byte.TryParse(s, out num)).Select(s => num).ToArray();
            if (nums.Length == 4) VM.CommService.SetUdpAddr(nums, VM.mainModel.PortUdp);
        },
            canExecPar => VM.mainModel.Connecting.Value));

        #endregion  

        #region запись параметров Ethernet параметров платы
        RelayCommand _writeEthParamsCommand;
        public RelayCommand WriteEthParamsCommand => _writeEthParamsCommand ?? (_writeEthParamsCommand = new RelayCommand(par =>
        {
            VM.CommService.SetTcpSettings(VM.mainModel.IP, VM.mainModel.Mask, VM.mainModel.GateWay);
        },
            o => VM.mainModel.Connecting.Value));
        #endregion

        #endregion
    }
}
