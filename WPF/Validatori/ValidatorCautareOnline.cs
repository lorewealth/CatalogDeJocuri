using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DespreJoc;

namespace WPF.Validatori
{
    class ValidatorCautareOnline
    {
        public static bool ValidareAdaugare(Joc JocCurent, List<Joc> Jocuri, TextBlock txbk, Control tipObj = null)
        {
            if (JocCurent == null)
            {
                ErrorHandler.ArataErr(txbk, tipObj, "Nu ati cautat nici o joaca!");
                return false;
            }
            else if (Jocuri.Any(j => j.ExternalId.Equals(JocCurent.ExternalId, StringComparison.OrdinalIgnoreCase)))
            {
                ErrorHandler.ArataErr(txbk, tipObj, "Aceasta joaca deja exista!");
                return false;
            }
            else
            {
                ErrorHandler.ResetErr(txbk, tipObj);
                return true;
            }
        }
        public static bool ValidareCautare(TextBox txb, TextBlock txbk, Joc JocCurent, bool dupaAPI = false, Control tipObj = null)
        {
            if (string.IsNullOrWhiteSpace(txb.Text))
            {
                ErrorHandler.ArataErr(txbk, tipObj, "Nu ati introdus un caracter!");
                return false;
            }
            else if(dupaAPI && JocCurent == null)
            {
                ErrorHandler.ArataErr(txbk, tipObj, "Nu a fost gasit joaca!");
                return false;
            }
            else
            {
                ErrorHandler.ResetErr(txbk, tipObj);
                return true;
            }
        }
    }
}
