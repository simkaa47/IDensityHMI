using IDensity.DataAccess;
using IDensity.DataAccess.Models;
using IDensity.DataAccess.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace IDensity.ViewModels
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

        public VM VM { get; }

        #region Коллекция ЕИ
        /// <summary>
        /// Коллекция ЕИ
        /// </summary>
        private List<MeasUnit> _measUnits;
       

        /// <summary>
        /// Коллекция ЕИ
        /// </summary>
        public List<MeasUnit> MeasUnits
        {
            get => _measUnits;
            set => Set(ref _measUnits, value);
        }
        #endregion

        void Init()
        {
           _measUnitRepository.Init(new List<MeasUnit>
            {
                new MeasUnit{Name="г/см^3",K=1, Mode=0},
                new MeasUnit{Name="мм",K=1, Mode=1}
            });      
        }

    }
}
