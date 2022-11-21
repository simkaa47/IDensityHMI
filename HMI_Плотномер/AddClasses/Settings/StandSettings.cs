using IDensity.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Timers;

namespace IDensity.AddClasses.Settings
{
    [DataContract]
    public class StandSettings:PropertyChangedBase
    {
        Timer standTimer = new Timer();
        const string TcpArg = "std=id,stdMeasUnit,duration,date,result,value,halfLifeValue";
        public StandSettings(int id)
        {
            this.Id = id;
            DescribeOnCommands();
            MeasUnitMemoryId = $"StandMeasMemory{id}";
        }
        /// <summary>
        /// Подписка на изменения 
        /// </summary>
        void DescribeOnCommands()
        {            
            StandDuration.CommandEcecutedEvent += o => CallWriteEvent("duration", StandDuration.WriteValue);
            LastStandDate.CommandEcecutedEvent += o => CallWriteEvent("date", LastStandDate.WriteValue.ToString("dd:MM:yy"));
            StandResult.CommandEcecutedEvent += o => CallWriteEvent("result", StandResult.WriteValue);
            StandPhysValue.CommandEcecutedEvent += o => CallWriteEvent("value", StandPhysValue.WriteValue);
            HalfLifeCorr.CommandEcecutedEvent += o => CallWriteEvent("halfLifeValue", HalfLifeCorr.WriteValue);
        }
        void CallWriteEvent<T>(string parName, T value)
        {
            var arg = TcpArg.Replace(parName, value.ToString().Replace(",", "."));
            var parameters = arg.Split(new char[] { ',', '=' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var par in parameters)
            {
                switch (par)
                {
                    case "id":
                        arg = arg.Replace(par, Id.ToString());
                        break;                   
                    case "duration":
                        arg = arg.Replace(par, StandDuration.Value.ToString());
                        break;
                    case "date":
                        arg = arg.Replace(par, LastStandDate.Value.ToString("dd:MM:yy"));
                        break;
                    case "result":
                        arg = arg.Replace(par, StandResult.Value.ToStringPoint());
                        break;
                    case "value":
                        arg = arg.Replace(par, StandPhysValue.Value.ToStringPoint());
                        break;
                    case "halfLifeValue":
                        arg = arg.Replace(par, StandPhysValue.Value.ToStringPoint());
                        break;
                    default:
                        break;
                }
            }
            NeedWriteEvent?.Invoke(arg);

        }
        #region Id
        private int _id;
        /// <summary>
        /// Идентификатор стандартизации
        /// </summary>
        [DataMember]
        public int Id
        {
            get { return _id; }
            set { Set(ref _id, value); }
        }


        #endregion        
        #region Длительность стандартизации
        /// <summary>
        /// Длительность стандартизации
        /// </summary>
        [DataMember]
        public Parameter<ushort> StandDuration { get; set; } = new Parameter<ushort>("StandDuration", "Длительность стандартизации, x 0.1 c.", 0, 5, 1, "hold");
        #endregion
        #region Дата последней стандартизации
        /// <summary>
        /// Дата последней стандартизации
        /// </summary>
        [DataMember]
        public Parameter<DateTime> LastStandDate { get; set; } = new Parameter<DateTime>("LastStandDate", "Дата последней стандартизации", DateTime.MinValue, DateTime.MaxValue, 2, "hold");
        #endregion
        #region Результат
        /// <summary>
        /// Результат
        /// </summary>
        [DataMember]
        public Parameter<float> StandResult { get; set; } = new Parameter<float>("StandResult", "Результат", float.MinValue, float.MaxValue, 5, "hold");
        #endregion
        #region Физическая величина
        /// <summary>
        /// Физическая величина
        /// </summary>
        [DataMember]
        public Parameter<float> StandPhysValue { get; set; } = new Parameter<float>("StandPhysValue", "Физическая величина", float.MinValue, float.MaxValue, 7, "hold");
        #endregion
        #region Значение стандартизации, скорректрованое по времени
        [DataMember]
        public Parameter<float> HalfLifeCorr { get; set; } = new Parameter<float>("StandHalfLifeCorr", "Значение с учетом полураспада", float.MinValue, float.MaxValue, 0, "");

        #endregion

        #region ID ЕИ
        private string _measUnitMemoryId;
        public string MeasUnitMemoryId
        {
            get => _measUnitMemoryId;
            set => Set(ref _measUnitMemoryId, value);
        } 
        #endregion


        /// <summary>
        /// Необходимо записать настройки стандартизаций
        /// </summary>
        public event Action<string> NeedWriteEvent;

        #region Проведение стандартизации
        #region Флаг стандартищации
        private bool _isStandartisation;

        public bool IsStandartisation
        {
            get { return _isStandartisation; }
            set { Set(ref _isStandartisation, value); }
        }
        #endregion

        private int _curStandTime;

        public int CurStandTime
        { 
            get { return _curStandTime; }
            set { Set(ref _curStandTime, value); }
        }


        RelayCommand _makeStandCommand;
        public RelayCommand MakeStandCommand => _makeStandCommand ?? (_makeStandCommand = new RelayCommand(par => {
            if (!IsStandartisation)
            {
                NeedMakeStand?.Invoke();
                standTimer.Elapsed += (o, e) => 
                {
                    IsStandartisation = false;
                    StandFinishEvent?.Invoke();
                    standTimer.Stop();
                };
                standTimer.Interval = StandDuration.Value*100+4000;
                standTimer.Start();
                IsStandartisation = true;
            }

        }, o => true));
        public event Action NeedMakeStand;
        public event Action StandFinishEvent;
        #endregion        
        
    }
}
