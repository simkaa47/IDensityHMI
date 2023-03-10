using IDensity.Core.Views;
using IDensity.DataAccess;
using IDensity.DataAccess.Models;
using IDensity.DataAccess.Repositories;
using IDensity.ViewModels;
using IDensity.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace IDensity.Core.ViewModels.MeasUnits
{
    public class MeasUnitVm : PropertyChangedBase
    {

        private IRepository<MeasUnit> _measUnitRepository;
        public MeasUnitVm(VM vM)
        {
            VM = vM;
            _measUnitRepository = new EfRepository<MeasUnit>();
            Init();
        }

        #region Команды

        #region Добавить ЕИ
        /// <summary>
        /// Добавить ЕИ
        /// </summary>
        RelayCommand _addMeasUnitCommand;
        /// <summary>
        /// Добавить ЕИ
        /// </summary>
        public  RelayCommand AddMeasUnitCommand => _addMeasUnitCommand ?? (_addMeasUnitCommand = new RelayCommand(execPar =>
        {
            SafetyAction(() =>
            {
                var addMeasNum = new MeasUnit();
                MeasUnitDialog dialog = new MeasUnitDialog(addMeasNum);               
                if (dialog.ShowDialog() == true)
                {
                    _measUnitRepository.Add(addMeasNum);
                }
            });            

        }, canExecPar => true));
        #endregion

        #region Редактировать ЕИ
        /// <summary>
        /// Редактировать ЕИ
        /// </summary>
        RelayCommand _editMeasNumCommand;
        /// <summary>
        /// Редактировать ЕИ
        /// </summary>
        public RelayCommand EditMeasNumCommand => _editMeasNumCommand ?? (_editMeasNumCommand = new RelayCommand(execPar => 
        {
            if (SelectedMeasUnit is null) return;
            SafetyAction(() =>
            {
                MeasUnitDialog dialog = new MeasUnitDialog(SelectedMeasUnit);
                if (dialog.ShowDialog() == true)
                {
                    _measUnitRepository.Update(SelectedMeasUnit);
                }
            });
        }, canExecPar => true));
        #endregion

        #region Удалить
        /// <summary>
        /// Удалить
        /// </summary>
        RelayCommand _delMeasNumCommand;
        /// <summary>
        /// Удалить
        /// </summary>
        public RelayCommand DelMeasNumCommand => _delMeasNumCommand ?? (_delMeasNumCommand = new RelayCommand(execPar => 
        {
            if (SelectedMeasUnit is null) return;
            if (SelectedMeasUnit.UserCantDelete) return;
            SafetyAction(() =>
            {
                _measUnitRepository.Delete(SelectedMeasUnit);
            });
        }, canExecPar => true));
        #endregion

        #endregion

        public VM VM { get; }

        #region Коллекция ЕИ
        /// <summary>
        /// Коллекция ЕИ
        /// </summary>
        private IEnumerable<MeasUnit> _measUnits;


        /// <summary>
        /// Коллекция ЕИ
        /// </summary>
        public IEnumerable<MeasUnit> MeasUnits
        {
            get => _measUnits;
            set => Set(ref _measUnits, value);
        }
        #endregion

        #region Выбранная ЕИ
        /// <summary>
        /// Выбранная ЕИ
        /// </summary>
        private MeasUnit _selectedMeasUnit;
        /// <summary>
        /// Выбранная ЕИ
        /// </summary>
        public MeasUnit SelectedMeasUnit
        {
            get => _selectedMeasUnit;
            set => Set(ref _selectedMeasUnit, value);
        }
        #endregion

        void Init()
        {
            MeasUnits = _measUnitRepository.Init(new List<MeasUnit>
            {
                new MeasUnit{Name="г/см^3",K=1, Offset = 0, Mode=0, DeviceType=0, UserCantDelete=true},
                new MeasUnit{Name="кг/м^3",K=1000, Offset = 0, Mode=0, DeviceType=0, UserCantDelete=true},
                new MeasUnit{Name="г/л",K=0.001f, Offset = 0, Mode=0, DeviceType=0, UserCantDelete=true},
                new MeasUnit{Name="г/л",K=0.001f, Offset = 0, Mode=1, DeviceType=0, UserCantDelete=true},
                new MeasUnit{Name="%",K=1, Offset = 0, Mode=1, DeviceType=0, UserCantDelete=true},
                new MeasUnit{Name="%",K=1, Offset = 0, Mode=2, DeviceType=0, UserCantDelete=true},
                new MeasUnit{Name="%",K=1, Offset = 0, Mode=3, DeviceType=0, UserCantDelete=true},
                new MeasUnit{Name="%",K=1, Offset = 0, Mode=4, DeviceType=0, UserCantDelete=true},
                new MeasUnit{Name="%",K=1, Offset = 0, Mode=5, DeviceType=0, UserCantDelete=true},
                new MeasUnit{Name="%",K=1, Offset = 0, Mode=6, DeviceType=0, UserCantDelete = true},
                new MeasUnit{Name="мм",K=1, Offset=0, Mode=0, DeviceType=1, UserCantDelete=true},
                new MeasUnit{Name="%",K=1, Offset=0, Mode=1, DeviceType=1, UserCantDelete = true},
                new MeasUnit{Name="м^3",K=1, Offset=0, Mode=2, DeviceType=1, UserCantDelete=true},
                new MeasUnit{Name="мм/c",K=1, Offset=0, Mode=3, DeviceType=1, UserCantDelete=true}
            });
        }

        void SafetyAction(Action action)
        {
            try
            {
                action.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }
}
