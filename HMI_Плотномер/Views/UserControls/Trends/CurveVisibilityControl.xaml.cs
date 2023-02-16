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

namespace IDensity.Core.Views.Resourses.UserControls
{
    /// <summary>
    /// Логика взаимодействия для CurveVisibilityControl.xaml
    /// </summary>
    public partial class CurveVisibilityControl : UserControl
    {
        public CurveVisibilityControl()
        {
            InitializeComponent();
        }

        public Brush Color
        {
            get { return (Brush)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Color.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Brush), typeof(CurveVisibilityControl), new PropertyMetadata());



        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Description.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(CurveVisibilityControl), new PropertyMetadata(string.Empty));





        public bool Check
        {
            get { return (bool)GetValue(CheckProperty); }
            set { SetValue(CheckProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Check.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CheckProperty =
            DependencyProperty.Register("Check", typeof(bool), typeof(CurveVisibilityControl), new PropertyMetadata(false));







    }
}
