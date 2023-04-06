using IDensity.AddClasses;
using IDensity.DataAccess;
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
            (_changeSerialSelectCommand = new RelayCommand(o =>
            {
                VM.mainModel.PortSelectMode.IsWriting = true;
                VM.CommService.ChangeSerialSelect((int)o); 
            }, o => o != null));
        #endregion

        #region Сменить баудрейт 
        RelayCommand _changeBaudrateCommand;
        public RelayCommand ChangeBaudrateCommand => 
            _changeBaudrateCommand ?? 
            (_changeBaudrateCommand = new RelayCommand(o => 
            { 
                VM.CommService.ChangeBaudrate((uint)(o));
                VM.mainModel.PortBaudrate.IsWriting = true;
            }, o => o != null));
        #endregion
        #endregion      

        #region Команда "Установить настроку IP приемника UDP даных
        RelayCommand _setUpsAddrCommand;
        public RelayCommand SetUpsAddrCommand => _setUpsAddrCommand ?? (_setUpsAddrCommand = new RelayCommand(execPar =>
        {
            byte num = 0;
            var nums = (VM.mainModel.UdpAddrString.Split(".", StringSplitOptions.RemoveEmptyEntries)).Where(s => byte.TryParse(s, out num)).Select(s => num).ToArray();
            if (nums.Length == 4)
            {
                VM.mainModel.UdpWriting = true;
                VM.CommService.SetUdpAddr(nums, VM.mainModel.PortUdp);
            }
        },
            canExecPar => VM.mainModel.Connecting.Value));

        #endregion  

        #region запись параметров Ethernet параметров платы
        RelayCommand _writeEthParamsCommand;
        public RelayCommand WriteEthParamsCommand => _writeEthParamsCommand ?? (_writeEthParamsCommand = new RelayCommand(par =>
        {
            VM.CommService.SetTcpSettings(VM.mainModel.IP, VM.mainModel.Mask, VM.mainModel.GateWay);
            VM.mainModel.TcpWriting = true;
        },
            o => VM.mainModel.Connecting.Value));
        #endregion


        #region Записать mac
        /// <summary>
        /// Записать mac
        /// </summary>
        RelayCommand _macAddrWriteCommand;
        /// <summary>
        /// Записать mac
        /// </summary>
        public RelayCommand MacAddrWriteCommand => _macAddrWriteCommand ?? (_macAddrWriteCommand = new RelayCommand(execPar => 
        {
            var mac = VM.mainModel.Mac;
            if (mac is null) return;
            var arg = $"*SETT,mac=";
            foreach(var b in mac)
            {
                arg += $"{b},";
            }
            arg += "#";
            VM.mainModel.MacWriting = true;
            VM.CommService.Tcp.SetFsrd8(arg);
        }, canExecPar => VM.mainModel.Connecting.Value));
        #endregion


        #region Перезагрузить плату
        RelayCommand _rstBoardCommand;
        public RelayCommand RstBoardCommand => _rstBoardCommand ?? (_rstBoardCommand = new RelayCommand(par => VM.CommService.RstBoard(), o => VM.mainModel.Connecting.Value));
        #endregion

        #endregion
    }
}
