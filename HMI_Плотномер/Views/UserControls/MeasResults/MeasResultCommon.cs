using IDensity.DataAccess.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace IDensity.Core.Views.UserControls.MeasResults
{
    public class MeasResultCommon:UserControl
    {
        #region Цвет
        public Brush IndicatorColor
        {
            get { return (Brush)GetValue(IndicatorColorProperty); }
            set { SetValue(IndicatorColorProperty, value); }

        }

        // Using a DependencyProperty as the backing store for IndicatorColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IndicatorColorProperty =
            DependencyProperty.Register("IndicatorColor", typeof(Brush), typeof(MeasResultCommon), new PropertyMetadata(Brushes.WhiteSmoke));
        #endregion

        #region Настройки видимости
        public MeasResultViewSett ViewSettings
        {
            get { return (MeasResultViewSett)GetValue(ViewSettingsProperty); }
            set { SetValue(ViewSettingsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ViewSettings.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewSettingsProperty =
            DependencyProperty.Register("ViewSettings", typeof(MeasResultViewSett), typeof(MeasResultCommon), new PropertyMetadata(null));
        #endregion
    }
}
