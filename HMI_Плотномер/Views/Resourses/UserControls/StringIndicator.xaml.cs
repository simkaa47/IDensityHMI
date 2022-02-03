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
    /// Логика взаимодействия для StringIndicator.xaml
    /// </summary>
    public partial class StringIndicator : UserControl
    {


        #region Индекс
        public int Index
        {
            get { return (int)GetValue(IndexProperty); }
            set { SetValue(IndexProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Index.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IndexProperty =
            DependencyProperty.Register(nameof(Index), typeof(int), typeof(StringIndicator), new PropertyMetadata(0));
        #endregion

        #region Коллекция строк(массив)
        public object Collection
        {
            get { return (object)GetValue(CollectionProperty); }
            set { SetValue(CollectionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Collection.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CollectionProperty =
            DependencyProperty.Register(nameof(Collection), typeof(object), typeof(StringIndicator), new PropertyMetadata(null)); 
        #endregion

        public StringIndicator()
        {
            InitializeComponent();
        }
    }
}
