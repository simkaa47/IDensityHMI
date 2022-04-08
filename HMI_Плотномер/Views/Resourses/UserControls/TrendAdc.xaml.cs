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
    /// Interaction logic for TrendAdc.xaml
    /// </summary>
    public partial class TrendAdc : UserControl
    {
        public TrendAdc()
        {
            InitializeComponent();
        }

        #region Коллекция
        public IEnumerable<Point> Collection
        {
            get { return (IEnumerable<Point>)GetValue(CollectionProperty); }
            set { SetValue(CollectionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Collection.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CollectionProperty =
            DependencyProperty.Register("Collection", typeof(IEnumerable<Point>), typeof(TrendAdc), new PropertyMetadata(null));
        #endregion

        #region Режим отображения данных
        public int Mode
        {
            get { return (int)GetValue(ModeProperty); }
            set { SetValue(ModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Mode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ModeProperty =
            DependencyProperty.Register("Mode", typeof(int), typeof(TrendAdc), new PropertyMetadata(0));
        #endregion

        #region Верхняя линия
        public int HighLineAnnotation
        {
            get { return (int)GetValue(HighLineAnnotationProperty); }
            set { SetValue(HighLineAnnotationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HighLineAnnotation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HighLineAnnotationProperty =
            DependencyProperty.Register("HighLineAnnotation", typeof(int), typeof(TrendAdc), new PropertyMetadata(0));
        #endregion

        #region Нижняя линия


        public int LowAnnotation
        {
            get { return (int)GetValue(LowAnnotationProperty); }
            set { SetValue(LowAnnotationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LowAnnotation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LowAnnotationProperty =
            DependencyProperty.Register("LowAnnotation", typeof(int), typeof(TrendAdc), new PropertyMetadata(0));


        #endregion

    }
}
