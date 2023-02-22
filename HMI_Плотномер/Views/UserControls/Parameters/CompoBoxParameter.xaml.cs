using System.Windows;

namespace IDensity.Core.Views.UserControls.Parameters
{
    /// <summary>
    /// Interaction logic for CompoBoxParameter.xaml
    /// </summary>
    public partial class CompoBoxParameter : CommandExtention
    {
        public CompoBoxParameter()
        {
            InitializeComponent();
        }        

        #region Источник данных
        public object ItemsSource
        {
            get { return (object)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(object), typeof(CompoBoxParameter), new PropertyMetadata(null));


        #endregion

        #region Выбранное значение
        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(CompoBoxParameter));


        #endregion

        #region Индекс
        public int Index
        {
            get { return (int)GetValue(IndexProperty); }
            set { SetValue(IndexProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Index.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IndexProperty =
            DependencyProperty.Register("Index", typeof(int), typeof(CompoBoxParameter), new PropertyMetadata(0));


        #endregion

        #region DisplayMemberPath
        public string DisplayMemberPath
        {
            get { return (string)GetValue(DisplayMemberPathProperty); }
            set { SetValue(DisplayMemberPathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DisplayMemberPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplayMemberPathProperty =
            DependencyProperty.Register("DisplayMemberPath", typeof(string), typeof(CompoBoxParameter), new PropertyMetadata());


        #endregion
    }
}
