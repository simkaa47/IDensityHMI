using IDensity.DataAccess.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace IDensity.Core.Views.UserControls.MeasResults
{
    /// <summary>
    /// Логика взаимодействия для MeasResult.xaml
    /// </summary>
    public partial class MeasResult : UserControl
    {
        public MeasResult()
        {
            InitializeComponent();
        }



        #region Цвет
        public Brush IndicatorColor
        {
            get { return (Brush)GetValue(IndicatorColorProperty); }
            set { SetValue(IndicatorColorProperty, value); }

        }

        // Using a DependencyProperty as the backing store for IndicatorColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IndicatorColorProperty =
            DependencyProperty.Register("IndicatorColor", typeof(Brush), typeof(MeasResult), new PropertyMetadata(Brushes.WhiteSmoke));
        #endregion


        public Visibility CounterValueVisibility
        {
            get { return (Visibility)GetValue(CounterValueVisibilityProperty); }
            set { SetValue(CounterValueVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CounterValueVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CounterValueVisibilityProperty =
            DependencyProperty.Register("CounterValueVisibility", typeof(Visibility), typeof(MeasResult), new PropertyMetadata(Visibility.Collapsed));





        public MeasResultViewSett ViewSettings
        {
            get { return (MeasResultViewSett)GetValue(ViewSettingsProperty); }
            set { SetValue(ViewSettingsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ViewSettings.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewSettingsProperty =
            DependencyProperty.Register("ViewSettings", typeof(MeasResultViewSett), typeof(MeasResult), new PropertyMetadata(null));







    }
}
