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
    /// Логика взаимодействия для DateTimeParameter.xaml
    /// </summary>
    public partial class DateTimeParameter : UserControl
    {
        #region Ширина поля DateTime
        public int DateTimeWidth
        {
            get { return (int)GetValue(DateTimeWidthProperty); }
            set { SetValue(DateTimeWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DateTimeWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DateTimeWidthProperty =
            DependencyProperty.Register("DateTimeWidth", typeof(int), typeof(DateTimeParameter), new PropertyMetadata(100));
        #endregion
        #region Текст
        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Description.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(DateTimeParameter), new PropertyMetadata(""));


        #endregion
        #region Дата и время
        public DateTime DateTime
        {
            get { return (DateTime)GetValue(DateTimeProperty); }
            set { SetValue(DateTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DateTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DateTimeProperty =
            DependencyProperty.Register("DateTime", typeof(DateTime), typeof(DateTimeParameter), new PropertyMetadata(default(DateTime)));
        #endregion

        

        public DateTimeParameter()
        {
            InitializeComponent();
        }
    }
}
