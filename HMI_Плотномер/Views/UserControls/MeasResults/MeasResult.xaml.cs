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









    }
}
