using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using DespreJoc;
using DespreJoc.Enums;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WPF.Validatori
{
    class JocInput
    {
        public static bool ValidareJocInput(Control Denumirea, TextBox Pret, TextBox Rate, TextBox Genre, Control Platforme, ListBox Editori, ListBox Dezvoltatori, Control Varsta, DatePicker ReleaseData,
                                            TextBlock ErrDenumirea, TextBlock ErrPret, TextBlock ErrRate, TextBlock ErrGenre, TextBlock ErrPlatforme, TextBlock ErrEditori, TextBlock ErrDezvoltatori, TextBlock ErrVarsta, TextBlock ErrReleaseData)
        {
            bool valid = true;

            DateTime ReleaseDataWPF = ReleaseData.SelectedDate ?? DateTime.MinValue;
            //pret
            if (string.IsNullOrWhiteSpace(Pret.Text))
            {
                valid = false;
                ErrorHandler.ArataErr(ErrPret, Pret, "Introduceti un pret!");
            }
            else if (!double.TryParse(Pret.Text.Replace(',', '.'), CultureInfo.InvariantCulture, out var pretD) || pretD < 0)
            {
                valid = false;
                ErrorHandler.ArataErr(ErrPret, Pret, "Introduceti un pret valid pozitiv sau 0!");
            }
            else ErrorHandler.ResetErr(ErrPret, Pret);

            //rate jocului
            if (string.IsNullOrWhiteSpace(Rate.Text))
            {
                valid = false;
                ErrorHandler.ArataErr(ErrRate, Rate, "Introduceti o rata!");
            }
            else if (!double.TryParse(Rate.Text.Replace(',', '.'), CultureInfo.InvariantCulture, out double rateD) || rateD < Joc.RATE_MIN || rateD > Joc.RATE_MAX)
            {
                valid = false;
                ErrorHandler.ArataErr(ErrRate, Rate, $"Introduceti o rata intre {Joc.RATE_MIN}-{Joc.RATE_MAX}!");
            }
            else ErrorHandler.ResetErr(ErrRate, Rate);

            //genre
            if (!string.IsNullOrWhiteSpace(Genre.Text))
            {
                string[] genreArr = Genre.Text.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                HashSet<string> unic = new HashSet<string>();

                if (genreArr.Length == 0)
                {
                    valid = false;
                    ErrorHandler.ArataErr(ErrGenre, Genre, "Nu ati introdus genre valide!");
                }
                else if (genreArr.Any(gen => !unic.Add(gen.ToLower())))
                {
                    valid = false;
                    ErrorHandler.ArataErr(ErrGenre, Genre, "Introduceti genre unice!");
                }
                else ErrorHandler.ResetErr(ErrGenre, Genre);

            }
            else
            {
                valid = false;
                ErrorHandler.ArataErr(ErrGenre, Genre, "Introduceti minim un genru!");
            }

            //editori
            DezvEditValid(Editori, ErrEditori, "editori", ref valid);

            //dezvoltatori
            DezvEditValid(Dezvoltatori, ErrDezvoltatori, "dezvoltatori", ref valid);

            //releaseData
            if (ReleaseDataWPF == DateTime.MinValue)
            {
                valid = false;
                ErrorHandler.ArataErr(ErrReleaseData, ReleaseData, "Introduceti un an!");
            }
            else ErrorHandler.ResetErr(ErrReleaseData, ReleaseData);

            //pentru buton meniu adaugarea
            if (Denumirea is TextBox txbD && Platforme is ComboBox cmbP && Varsta is ComboBox cmbV)
            {
                string denumireaWPF = txbD.Text;
                ItemCollection PlatformeWPFCmb = cmbP.Items;
                string varstaWPFCmb = cmbV.Text;

                //denumirea
                if (string.IsNullOrWhiteSpace(denumireaWPF))
                {
                    valid = false;
                    ErrorHandler.ArataErr(ErrDenumirea, Denumirea, "Introduceti o denumire!");
                }
                else ErrorHandler.ResetErr(ErrDenumirea, Denumirea);

                //platforme
                int selectat = 0;
                foreach (var elem in PlatformeWPFCmb)
                {
                    if (elem is CheckBox checkboxul && checkboxul.IsChecked == true) selectat++;
                }
                if (selectat == 0)
                {
                    valid = false;
                    ErrorHandler.ArataErr(ErrPlatforme, Platforme, "Selectati macar o optiune valida!");
                }
                else ErrorHandler.ResetErr(ErrPlatforme, Platforme);

                //varsta
                if (!Enum.IsDefined(typeof(RatingVarsta), varstaWPFCmb))
                {
                    valid = false;
                    ErrorHandler.ArataErr(ErrVarsta, Varsta, "Selectati o varsta!");
                }
                else ErrorHandler.ResetErr(ErrVarsta, Varsta);

            }

            //pentru modificare
            if (Denumirea is ComboBox cmbD && Platforme is ListBox lbP && Varsta is ListBox lbV)
            {
                //denumirea
                if (cmbD.SelectedItem == null)
                {
                    valid = false;
                    ErrorHandler.ArataErr(ErrDenumirea, Denumirea, "Selectati macar o optiune valida!");
                }
                else ErrorHandler.ResetErr(ErrDenumirea, Denumirea);

                //platforme
                if (lbP.SelectedItems == null || lbP.SelectedItems.Count == 0)
                {
                    valid = false;
                    ErrorHandler.ArataErr(ErrPlatforme, Platforme, "Selectati macar o optiune valida!");
                }
                else ErrorHandler.ResetErr(ErrPlatforme, Platforme);

                //varsta
                if (lbV.SelectedItem == null || !Enum.IsDefined(typeof(RatingVarsta), lbV.SelectedItem))
                {
                    valid = false;
                    ErrorHandler.ArataErr(ErrVarsta, Varsta, "Selectati macar o optiune valida!");
                }
                else ErrorHandler.ResetErr(ErrVarsta, Varsta);
            }

            return valid;
        }
        private static void DezvEditValid(ListBox lsbx, TextBlock txbk, string tip, ref bool valid)
        {
            if (lsbx.SelectedIndex == -1)
            {
                ErrorHandler.ArataErr(txbk, lsbx, $"Selectati minim un {tip}");
                valid = false;
            }
            else ErrorHandler.ResetErr(txbk, lsbx);
        }
    }
    
}
