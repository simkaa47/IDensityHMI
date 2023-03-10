using System;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace IDensity.Core.Views.Converters
{
    internal class HighRegisterConverter:Converter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is string notFormattedText)) return null;
            TextBlock textBlock = new TextBlock();
            

            var list = notFormattedText.Split('^').ToList();
            int i = 0;
            foreach (var str in list)
            {
                i++;
                if (str.Length > 0)
                {
                    var run = new Run(str);
                    textBlock.Inlines.Add(run);
                    if (i % 2 == 0)
                    {
                        run.Typography.Variants = System.Windows.FontVariants.Superscript;
                        run.FontFamily = new FontFamily("Palatino Linotype");
                    }
                }


            }
            return textBlock;
        }
    }
}
