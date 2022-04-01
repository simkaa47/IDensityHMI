using IDensity.AddClasses;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IDensity.Views.Resourses.UserControls
{
    /// <summary>
    /// Логика взаимодействия для TrendControl.xaml
    /// </summary>
    public partial class TrendControl : UserControl
    {
        public TrendControl()
        {
            InitializeComponent();
        }

        #region Источник для трендов
        public IEnumerable<TimePoint> DataSource
        {
            get { return (IEnumerable<TimePoint>)GetValue(DataSourceProperty); }
            set { SetValue(DataSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DataSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataSourceProperty =
            DependencyProperty.Register("DataSource", typeof(IEnumerable<TimePoint>), typeof(TrendControl), new PropertyMetadata(null));
        #endregion

        #region Видимость тренда 1
        public Visibility LineVisibility1
        {
            get { return (Visibility)GetValue(LineVisibility1Property); }
            set { SetValue(LineVisibility1Property, value); }
        }

        // Using a DependencyProperty as the backing store for LineVisibility1.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LineVisibility1Property =
            DependencyProperty.Register("LineVisibility1", typeof(Visibility), typeof(TrendControl), new PropertyMetadata(Visibility.Visible));
        #endregion

        #region Навзание тренда
        public string LineTitle1
        {
            get { return (string)GetValue(LineTitle1Property); }
            set { SetValue(LineTitle1Property, value); }
        }

        // Using a DependencyProperty as the backing store for LineTitle1.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LineTitle1Property =
            DependencyProperty.Register("LineTitle1", typeof(string), typeof(TrendControl), new PropertyMetadata(""));



        #endregion



    }
}
