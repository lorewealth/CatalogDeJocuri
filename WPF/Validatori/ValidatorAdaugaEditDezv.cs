using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using DespreJoc;

namespace WPF.Validatori
{
    class AddDezvEditListBoxVal
    {
        public static bool Validare<T>(TextBox Txbx, TextBlock ErrTxbk, ObservableCollection<T> Lista, Func<T, string> selectatorNume, string tip)
        {
            if (string.IsNullOrWhiteSpace(Txbx.Text))
            {
                ErrorHandler.ArataErr(ErrTxbk, Txbx, $"Introduceti un {tip}");
                return false;
            }
            else if (Lista.Any(elem => selectatorNume(elem).Equals(Txbx.Text, StringComparison.OrdinalIgnoreCase)))
            {
                ErrorHandler.ArataErr(ErrTxbk, Txbx, $"Introduceti un {tip} unic!");
                return false;
            }
            else
            {
                ErrorHandler.ResetErr(ErrTxbk, Txbx);
                return true;
            }
        }
    }
}
