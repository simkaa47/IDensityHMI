using IDensity.AddClasses;
using IDensity.Models;
using IDensity.Services.CheckServices;
using IDensity.Services.ComminicationServices;
using IDensity.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace IDensity.ViewModels.MasrerSettings
{
    public class CheckMasterVm : PropertyChangedBase
    {
        public CheckMasterVm(VM vM)
        {
            VM = vM;
            _model = vM.mainModel;
            _commService = vM.CommService;
            Describe();
        }
        private readonly MainModel _model;
        private readonly CommunicationService _commService;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        #region Статусы процесса

        #region Основной статус процессв
        /// <summary>
        /// Основной статус процессв
        /// </summary>
        private string _mainStatus;
        /// <summary>
        /// Основной статус процессв
        /// </summary>
        public string MainStatus
        {
            get => _mainStatus;
            set => Set(ref _mainStatus, value);
        }
        #endregion

        #region Вспомогательный статус процесса
        /// <summary>
        /// Вспомогательный статус процесса
        /// </summary>
        private string _subStatus;
        /// <summary>
        /// Вспомогательный статус процесса
        /// </summary>
        public string SubStatus
        {
            get => _subStatus;
            set => Set(ref _subStatus, value);
        }
        #endregion

        #endregion

        #region Список результатов
        /// <summary>
        /// Список результатов
        /// </summary>
        private List<DeviceCheckResult> _results = new List<DeviceCheckResult>();
        /// <summary>
        /// Список результатов
        /// </summary>
        public List<DeviceCheckResult> Results
        {
            get => _results;
            set => Set(ref _results, value);
        }
        #endregion






        public VM VM { get; }

        #region Стадия проверки
        /// <summary>
        /// Стадия проверки
        /// </summary>
        private CheckMasterStates _stage;
        /// <summary>
        /// Стадия проверки
        /// </summary>
        public CheckMasterStates Stage
        {
            get => _stage;
            set => Set(ref _stage, value);
        }
        #endregion 

        #region Отмена 
        /// <summary>
        /// Отмена 
        /// </summary>
        RelayCommand _cancelCommand;
        /// <summary>
        /// Отмена 
        /// </summary>
        public RelayCommand CancelCommand => _cancelCommand ?? (_cancelCommand = new RelayCommand(execPar =>
        {
            Cancel();
            Stage = CheckMasterStates.CancelByUser;

        }, canExecPar => true));
        #endregion

        #region Старт
        /// <summary>
        /// Старт
        /// </summary>
        RelayCommand _startCheckCommandCommand;
        /// <summary>
        /// Старт
        /// </summary>
        public RelayCommand StartCheckCommandCommand => _startCheckCommandCommand ?? (_startCheckCommandCommand = new RelayCommand(execPar =>
        {
            Check();
        }, canExecPar => true));
        #endregion

        public async void Check()
        {
            if (Stage != CheckMasterStates.Start) return;
            Stage = CheckMasterStates.Process;
            try
            {
                MainStatus = "Проверка работы аналоговых выходов...";
                for (int i = 0; i < 2; i++)
                    await CheckAnalog(i);
                Stage = CheckMasterStates.Success;
            }
            catch (Exception ex)
            {

            }
            finally
            {

            }
        }

        async Task CheckAnalog(int number)
        {
            //List<SetCurrentResult> setCurrentResults = new List<SetCurrentResult>();
            await SwitchAnalog(number);
            //setCurrentResults.Add(await SetCurrent(4, number));
            //setCurrentResults.Add(await SetCurrent(12, number));
            //setCurrentResults.Add(await SetCurrent(20, number));
            ///Print(setCurrentResults, number);

        }

        async Task SwitchAnalog(int outNum)
        {
            bool needSetSettings = false;
            do
            {
                if (!(_model.AnalogGroups[outNum].AO.Clone() is AnalogOutput analog)) return;
                analog.DacType.Value = 0;
                needSetSettings = false;
                if (!_model.AnalogGroups[outNum].AO.CommState.Value)
                    needSetSettings = await CheckAnalogAndExecute(() =>
                    {
                        _commService.SwitchAm(outNum, 0, true);
                        _commService.SwitchAm(outNum, 1, true);
                    });

                if (_model.AnalogGroups[outNum].AO.DacType.Value != 0)
                    needSetSettings = await CheckAnalogAndExecute(() => _commService.ChangeDacAct(outNum, 0, analog));

            } while (needSetSettings && !_cancellationTokenSource.IsCancellationRequested);
        }

        async Task<bool> CheckAnalogAndExecute(Action action)
        {
            if (_cancellationTokenSource.IsCancellationRequested) return false;
            action?.Invoke();
            await Task.Delay(1000, _cancellationTokenSource.Token);
            return true;
        }

        //async Task<SetCurrentResult> SetCurrent(int setValue, int moduleNum)
        //{
        //    SubStatus = ($"Аналоговый выход {moduleNum} : Выставляем ток {setValue} mA");
        //    var result = new SetCurrentResult();
        //    _model.AnalogGroups[moduleNum].AO.AmTestValue.Value = (ushort)(setValue * 1000);
        //    _commService.SetTestValueAm(moduleNum, 0, (ushort)(setValue*1000));
        //    await Task.Delay(5000, _cancellationTokenSource.Token);
        //    result.SetLevel = setValue;
        //    result.CurrentLevel = (float)_model.AnalogGroups[moduleNum].AO.AdcValue.Value / 69;
        //    if (_model.AnalogGroups[moduleNum].AO.VoltageTest.Value < 200)
        //    {
        //        SubStatus = ($"Аналоговый выход {moduleNum} : Обрыв цепи аналогового выхода");
        //    }
        //    else
        //    {
        //        SubStatus = ($"Аналоговый выход {moduleNum} :   " +
        //            $"Результат замера {result.CurrentLevel} mA при ожидаемых {result.SetLevel} mA " +
        //            $"(Отклонение {((Math.Abs(result.CurrentLevel - result.SetLevel)) / result.SetLevel * 100).ToString("f3")}%)");
        //    }

        //    return result;
        //}


        //void Print(List<SetCurrentResult> results, int moduleNum)
        //{
        //    //States.Add($"Проверка аналогового модуля {moduleNum}:");
        //    //foreach (var result in results)
        //    //{
        //    //    States.Add($"Уровень тока {result.SetLevel} mA:");
        //    //    States.Add($"Измеренное значение {result.CurrentLevel.ToString("f3")}");
        //    //    States.Add($"Отклонение {((Math.Abs(result.CurrentLevel-result.SetLevel))/result.SetLevel*100).ToString("f3")}%");                
        //    //}
        //}

        void Describe()
        {
            _model.Connecting.PropertyChanged += (o, e) =>
            {
                if (e.PropertyName == nameof(_model.Connecting.Value))
                {
                    if (!_model.Connecting.Value)
                    {
                        Cancel();
                        Stage = CheckMasterStates.CancelByCommunicate;
                    }
                }
            };
        }

        void Cancel()
        {
            if (_cancellationTokenSource.Token.CanBeCanceled)
            {
                _cancellationTokenSource.Cancel();
            }
        }


    }
}
