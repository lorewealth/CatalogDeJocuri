using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPF.Validatori
{
    class ErrorHandler
    {
        public static void ArataErr(TextBlock ErrTextBlock, Control TipObj, string Mesaj)
        {
            if (TipObj is TextBox)
            {
                ErrTextBlock.Visibility = Visibility.Visible;
                ErrTextBlock.Text = Mesaj;
                TipObj.ToolTip = Mesaj;
                TipObj.BorderBrush = new SolidColorBrush(Colors.Red);
            }
            else
            {
                ErrTextBlock.Visibility = Visibility.Visible;
                ErrTextBlock.Text = Mesaj;
            }
        }
        public static void ResetErr(TextBlock ErrTextBlock, Control TipObj)
        {
            if (TipObj is TextBox)
            {
                ErrTextBlock.Visibility = Visibility.Collapsed;
                TipObj.ClearValue(Control.BorderBrushProperty);
            }
            else
            {
                ErrTextBlock.Visibility = Visibility.Collapsed;
            }
        }
    }
}
