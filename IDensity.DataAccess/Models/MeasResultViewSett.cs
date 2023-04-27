using IDensity.DataAccess.Repositories;

namespace IDensity.DataAccess.Models
{
    public class MeasResultViewSett:PropertyChangedBase, IDataBased
    {
        public long Id { get; set; }

		#region Cur visibility
		/// <summary>
		/// Cur visibility
		/// </summary>
		private bool _curVisibility;
		/// <summary>
		/// Cur visibility
		/// </summary>
		public bool CurVisibility
		{
			get => _curVisibility;
			set => Set(ref _curVisibility, value);
		}
		#endregion

		#region AvgVisibility
		/// <summary>
		/// AvgVisibility
		/// </summary>
		private bool _avgVisibility;
		/// <summary>
		/// AvgVisibility
		/// </summary>
		public bool AvgVisibility
		{
			get => _avgVisibility;
			set => Set(ref _avgVisibility, value);
		}
		#endregion

		#region Time unit
		/// <summary>
		/// Time unit
		/// </summary>
		private int _timeUnit;
		/// <summary>
		/// Time unit
		/// </summary>
		public int TimeUnit
		{
			get => _timeUnit;
			set => Set(ref _timeUnit, value);
		}
		#endregion

	}

	

	public class TimeUnit
	{
		public string Name { get; set; }
		public float K { get; set; }

	}
}
