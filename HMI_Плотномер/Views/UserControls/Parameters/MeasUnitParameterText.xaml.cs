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

namespace IDensity.Core.Views.UserControls.Parameters
{
    /// <summary>
    /// Interaction logic for MeasUnitParameterText.xaml
    /// </summary>
    public partial class MeasUnitParameterText : CommandExtention
    {
        public MeasUnitParameterText()
        {
            InitializeComponent();
        }

        #region Тип измерения
        public int MeasType
        {
            get { return (int)GetValue(MeasTypeProperty); }
            set { SetValue(MeasTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MeasType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MeasTypeProperty =
            DependencyProperty.Register(nameof(MeasType), typeof(int), typeof(MeasUnitParameterText), new PropertyMetadata(0));
        #endregion

        #region Идентификатор
        public string MeasUnitMemoryId
        {
            get { return (string)GetValue(MeasUnitMemoryIdProperty); }
            set { SetValue(MeasUnitMemoryIdProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MeasUnitMemoryId.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MeasUnitMemoryIdProperty =
            DependencyProperty.Register(nameof(MeasUnitMemoryId), typeof(string), typeof(MeasUnitParameterText), new PropertyMetadata(string.Empty));
        #endregion
    }
}
