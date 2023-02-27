using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace IDensity.Core.Views.UserControls.Parameters
{
    public class CommandExtention : UserControl
    {
        #region Команда
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(CommandExtention), new PropertyMetadata(null, OnValuePropertyChanged, OnCoerceValue), OnValidateValue);

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
        public object CommanDParameter
        {
            get { return (object)GetValue(CommanDParameterProperty); }
            set { SetValue(CommanDParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CommanDParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommanDParameterProperty =
            DependencyProperty.Register(nameof(CommanDParameter), typeof(object), typeof(CommandExtention));
        #endregion

        public bool DescriptionNotVisible
        {
            get { return (bool)GetValue(DescriptionNotVisibleProperty); }
            set { SetValue(DescriptionNotVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DescriptionNotVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DescriptionNotVisibleProperty =
            DependencyProperty.Register("DescriptionNotVisible", typeof(bool), typeof(CommandExtention), new PropertyMetadata(false));


    }
}
