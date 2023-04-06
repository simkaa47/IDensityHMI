using System.Collections.ObjectModel;

namespace IDensity.Core.Models.Parameters
{
    class ParamList
    {
        public static ObservableCollection<object> ParameterList { get; } = new ObservableCollection<object>();
    }
}
