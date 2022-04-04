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
    /// Логика взаимодействия для CompoboxParameter.xaml
    /// </summary>
    public partial class MenuItemCompoboxParameter : CommandExtention
    {


        #region Ширина компобокса
        public int CompoboxWidth
        {
            get { return (int)GetValue(CompoboxWidthProperty); }
            set { SetValue(CompoboxWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CompoboxWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CompoboxWidthProperty =
            DependencyProperty.Register("CompoboxWidth", typeof(int), typeof(MenuItemCompoboxParameter), new PropertyMetadata(0));
        #endregion

        #region Источник данных
        public object ItemSource
        {
            get { return (object)GetValue(ItemSourceProperty); }
            set { SetValue(ItemSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemSourceProperty =
            DependencyProperty.Register("ItemSource", typeof(object), typeof(MenuItemCompoboxParameter), new PropertyMetadata(null));


        #endregion

        #region Выбранное значение
        public object  SelectedItem
        {
            get { return (object )GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object ), typeof(MenuItemCompoboxParameter));


        #endregion

        #region Индекс
        public int Index
        {
            get { return (int)GetValue(IndexProperty); }
            set { SetValue(IndexProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Index.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IndexProperty =
            DependencyProperty.Register("Index", typeof(int), typeof(MenuItemCompoboxParameter), new PropertyMetadata(null));


        #endregion

        #region DisplayMemberPath
        public string DisplayMemberPath
        {
            get { return (string)GetValue(DisplayMemberPathProperty); }
            set { SetValue(DisplayMemberPathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DisplayMemberPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplayMemberPathProperty =
            DependencyProperty.Register("DisplayMemberPath", typeof(string), typeof(MenuItemCompoboxParameter), new PropertyMetadata());


        #endregion

        #region Команда
        public ICommand DropOpenedCommand
        {
            get { return (ICommand)GetValue(DropOpenedCommandProperty); }
            set { SetValue(DropOpenedCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DropOpenedCommandProperty =
            DependencyProperty.Register(nameof(DropOpenedCommand), typeof(ICommand), typeof(MenuItemCompoboxParameter), new PropertyMetadata(null, OnValuePropertyChanged, OnCoerceValue), OnValidateValue);

        private static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        private static object OnCoerceValue(DependencyObject d, object baseValue)
        {
            return baseValue;
        }

        private static bool OnValidateValue(object o)
        {
            return true;
        }
        #endregion

        #region Параметр команды
        public object DropOpenedCommandParameter
        {
            get { return (object)GetValue(DropOpenedCommandParameterProperty); }
            set { SetValue(DropOpenedCommandParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CommanDParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DropOpenedCommandParameterProperty =
            DependencyProperty.Register("DropOpenedCommandParameter", typeof(object), typeof(MenuItemCompoboxParameter));
        #endregion

        public MenuItemCompoboxParameter()
        {
            InitializeComponent();
        }
    }
}
