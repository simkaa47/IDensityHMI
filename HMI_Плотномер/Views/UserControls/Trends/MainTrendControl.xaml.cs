using IDensity.Core.Models.Trends;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace IDensity.Core.Views.UserControls.Trends
{
    /// <summary>
    /// Логика взаимодействия для MainTrendControl.xaml
    /// </summary>
    public partial class MainTrendControl : UserControl
    {
        public MainTrendControl()
        {
            InitializeComponent();
        }
        public double MaximumRange
        {
            get { return (double)GetValue(MaximumRangeProperty); }
            set { SetValue(MaximumRangeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaximumRange.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaximumRangeProperty =
            DependencyProperty.Register("MaximumRange", typeof(double), typeof(MainTrendControl), new PropertyMetadata(double.MaxValue));


        #region Источник для трендов
        public IEnumerable<TimePoint> DataSource
        {
            get { return (IEnumerable<TimePoint>)GetValue(DataSourceProperty); }
            set { SetValue(DataSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DataSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataSourceProperty =
            DependencyProperty.Register("DataSource", typeof(IEnumerable<TimePoint>), typeof(MainTrendControl), new PropertyMetadata(null));
        #endregion

        #region Видимость тренда 1
        public Visibility LineVisibility1
        {
            get { return (Visibility)GetValue(LineVisibility1Property); }
            set { SetValue(LineVisibility1Property, value); }
        }

        // Using a DependencyProperty as the backing store for LineVisibility1.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LineVisibility1Property =
            DependencyProperty.Register("LineVisibility1", typeof(Visibility), typeof(MainTrendControl), new PropertyMetadata(Visibility.Visible));
        #endregion

        #region Навзание тренда 1
        public string LineTitle1
        {
            get { return (string)GetValue(LineTitle1Property); }
            set { SetValue(LineTitle1Property, value); }
        }

        // Using a DependencyProperty as the backing store for LineTitle1.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LineTitle1Property =
            DependencyProperty.Register("LineTitle1", typeof(string), typeof(MainTrendControl), new PropertyMetadata(""));



        #endregion

        #region Видимость тренда 2
        public Visibility LineVisibility2
        {
            get { return (Visibility)GetValue(LineVisibility2Property); }
            set { SetValue(LineVisibility2Property, value); }
        }

        // Using a DependencyProperty as the backing store for LineVisibility1.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LineVisibility2Property =
            DependencyProperty.Register("LineVisibility2", typeof(Visibility), typeof(MainTrendControl), new PropertyMetadata(Visibility.Visible));
        #endregion

        #region Навзание тренда 2
        public string LineTitle2
        {
            get { return (string)GetValue(LineTitle2Property); }
            set { SetValue(LineTitle2Property, value); }
        }

        // Using a DependencyProperty as the backing store for LineTitle1.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LineTitle2Property =
            DependencyProperty.Register("LineTitle2", typeof(string), typeof(MainTrendControl), new PropertyMetadata(""));



        #endregion

        #region Видимость тренда 3
        public Visibility LineVisibility3
        {
            get { return (Visibility)GetValue(LineVisibility3Property); }
            set { SetValue(LineVisibility3Property, value); }
        }

        // Using a DependencyProperty as the backing store for LineVisibility1.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LineVisibility3Property =
            DependencyProperty.Register("LineVisibility3", typeof(Visibility), typeof(MainTrendControl), new PropertyMetadata(Visibility.Visible));
        #endregion

        #region Навзание тренда 3
        public string LineTitle3
        {
            get { return (string)GetValue(LineTitle3Property); }
            set { SetValue(LineTitle3Property, value); }
        }

        // Using a DependencyProperty as the backing store for LineTitle1.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LineTitle3Property =
            DependencyProperty.Register("LineTitle3", typeof(string), typeof(MainTrendControl), new PropertyMetadata(""));



        #endregion

        #region Видимость тренда 4
        public Visibility LineVisibility4
        {
            get { return (Visibility)GetValue(LineVisibility4Property); }
            set { SetValue(LineVisibility4Property, value); }
        }

        // Using a DependencyProperty as the backing store for LineVisibility4.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LineVisibility4Property =
            DependencyProperty.Register("LineVisibility4", typeof(Visibility), typeof(MainTrendControl), new PropertyMetadata(Visibility.Visible));
        #endregion

        #region Навзание тренда 4
        public string LineTitle4
        {
            get { return (string)GetValue(LineTitle4Property); }
            set { SetValue(LineTitle4Property, value); }
        }

        // Using a DependencyProperty as the backing store for LineTitle4.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LineTitle4Property =
            DependencyProperty.Register("LineTitle4", typeof(string), typeof(MainTrendControl), new PropertyMetadata(""));



        #endregion

        #region Видимость тренда 5
        public Visibility LineVisibility5
        {
            get { return (Visibility)GetValue(LineVisibility5Property); }
            set { SetValue(LineVisibility5Property, value); }
        }

        // Using a DependencyProperty as the backing store for LineVisibility5.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LineVisibility5Property =
            DependencyProperty.Register("LineVisibility5", typeof(Visibility), typeof(MainTrendControl), new PropertyMetadata(Visibility.Visible));
        #endregion

        #region Навзание тренда 5
        public string LineTitle5
        {
            get { return (string)GetValue(LineTitle5Property); }
            set { SetValue(LineTitle5Property, value); }
        }

        // Using a DependencyProperty as the backing store for LineTitle4.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LineTitle5Property =
            DependencyProperty.Register("LineTitle5", typeof(string), typeof(MainTrendControl), new PropertyMetadata(""));
        #endregion

        #region Видимость тренда 6
        public Visibility LineVisibility6
        {
            get { return (Visibility)GetValue(LineVisibility6Property); }
            set { SetValue(LineVisibility6Property, value); }
        }

        // Using a DependencyProperty as the backing store for LineVisibility6.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LineVisibility6Property =
            DependencyProperty.Register("LineVisibility6", typeof(Visibility), typeof(MainTrendControl), new PropertyMetadata(Visibility.Visible));
        #endregion

        #region Навзание тренда 6
        public string LineTitle6
        {
            get { return (string)GetValue(LineTitle6Property); }
            set { SetValue(LineTitle6Property, value); }
        }

        // Using a DependencyProperty as the backing store for LineTitle6.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LineTitle6Property =
            DependencyProperty.Register("LineTitle6", typeof(string), typeof(MainTrendControl), new PropertyMetadata(""));
        #endregion

        #region Видимость тренда 7
        public Visibility LineVisibility7
        {
            get { return (Visibility)GetValue(LineVisibility7Property); }
            set { SetValue(LineVisibility7Property, value); }
        }

        // Using a DependencyProperty as the backing store for LineVisibility7.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LineVisibility7Property =
            DependencyProperty.Register("LineVisibility7", typeof(Visibility), typeof(MainTrendControl), new PropertyMetadata(Visibility.Visible));
        #endregion

        #region Навзание тренда 7
        public string LineTitle7
        {
            get { return (string)GetValue(LineTitle7Property); }
            set { SetValue(LineTitle7Property, value); }
        }

        // Using a DependencyProperty as the backing store for LineTitle7.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LineTitle7Property =
            DependencyProperty.Register("LineTitle7", typeof(string), typeof(MainTrendControl), new PropertyMetadata(""));
        #endregion

        #region Видимость тренда 8
        public Visibility LineVisibility8
        {
            get { return (Visibility)GetValue(LineVisibility8Property); }
            set { SetValue(LineVisibility8Property, value); }
        }

        // Using a DependencyProperty as the backing store for LineVisibility8.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LineVisibility8Property =
            DependencyProperty.Register("LineVisibility8", typeof(Visibility), typeof(MainTrendControl), new PropertyMetadata(Visibility.Visible));
        #endregion

        #region Навзание тренда 8
        public string LineTitle8
        {
            get { return (string)GetValue(LineTitle8Property); }
            set { SetValue(LineTitle8Property, value); }
        }

        // Using a DependencyProperty as the backing store for LineTitle8.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LineTitle8Property =
            DependencyProperty.Register("LineTitle8", typeof(string), typeof(MainTrendControl), new PropertyMetadata(""));
        #endregion

        #region Видимость тренда 9
        public Visibility LineVisibility9
        {
            get { return (Visibility)GetValue(LineVisibility9Property); }
            set { SetValue(LineVisibility9Property, value); }
        }

        // Using a DependencyProperty as the backing store for LineVisibility9.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LineVisibility9Property =
            DependencyProperty.Register("LineVisibility9", typeof(Visibility), typeof(MainTrendControl), new PropertyMetadata(Visibility.Visible));
        #endregion

        #region Навзание тренда 9
        public string LineTitle9
        {
            get { return (string)GetValue(LineTitle9Property); }
            set { SetValue(LineTitle9Property, value); }
        }

        // Using a DependencyProperty as the backing store for LineTitle9.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LineTitle9Property =
            DependencyProperty.Register("LineTitle9", typeof(string), typeof(MainTrendControl), new PropertyMetadata(""));
        #endregion

        #region Видимость тренда 10
        public Visibility LineVisibility10
        {
            get { return (Visibility)GetValue(LineVisibility10Property); }
            set { SetValue(LineVisibility10Property, value); }
        }

        // Using a DependencyProperty as the backing store for LineVisibility9.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LineVisibility10Property =
            DependencyProperty.Register("LineVisibility10", typeof(Visibility), typeof(MainTrendControl), new PropertyMetadata(Visibility.Visible));
        #endregion

        #region Навзание тренда 10
        public string LineTitle10
        {
            get { return (string)GetValue(LineTitle10Property); }
            set { SetValue(LineTitle10Property, value); }
        }

        // Using a DependencyProperty as the backing store for LineTitle10.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LineTitle10Property =
            DependencyProperty.Register("LineTitle10", typeof(string), typeof(MainTrendControl), new PropertyMetadata(""));
        #endregion

        
    }
}
