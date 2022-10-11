using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace IDensity.Views.Resourses.UserControls
{
    /// <summary>
    /// Логика взаимодействия для MeasUnitTextParameter.xaml
    /// </summary>
    public partial class MeasUnitTextParameter : CommandExtention
    {
        public MeasUnitTextParameter()
        {
            InitializeComponent();
        }
        public string StringFormat { get; set; }

        #region Тип измерения
        public int MeasType
        {
            get { return (int)GetValue(MeasTypeProperty); }
            set { SetValue(MeasTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MeasType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MeasTypeProperty =
            DependencyProperty.Register(nameof(MeasType), typeof(int), typeof(MeasUnitTextParameter), new PropertyMetadata(0));
        #endregion

        #region Идентификатор
        public string MeasUnitMemoryId
        {
            get { return (string)GetValue(MeasUnitMemoryIdProperty); }
            set { SetValue(MeasUnitMemoryIdProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MeasUnitMemoryId.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MeasUnitMemoryIdProperty =
            DependencyProperty.Register(nameof(MeasUnitMemoryId), typeof(string), typeof(MeasUnitTextParameter), new PropertyMetadata(string.Empty)); 
        #endregion

    }
}   
