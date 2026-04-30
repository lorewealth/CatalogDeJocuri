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

namespace WPF.Validatori
{
    class JocInput
    {
        public static bool ValidareJocInput(Control Denumirea, TextBox Pret, TextBox Rate, TextBox Genre, Control Platforme, TextBox Editori, TextBox Dezvoltatori, Control Varsta, DatePicker ReleaseData,
                                            TextBlock ErrDenumirea, TextBlock ErrPret, TextBlock ErrRate, TextBlock ErrGenre, TextBlock ErrPlatforme, TextBlock ErrEditori, TextBlock ErrDezvoltatori, TextBlock ErrVarsta, TextBlock ErrReleaseData)
        {
            bool valid = true;

            DateTime ReleaseDataWPF = ReleaseData.SelectedDate ?? DateTime.MinValue;

            //pret
            if (string.IsNullOrWhiteSpace(Pret.Text))
            {
                valid = false;
                ErrHandler(ErrPret, Pret, "Introduceti un pret!");
            }
            else if (!double.TryParse(Pret.Text, CultureInfo.InvariantCulture, out var pretD) || pretD < 0)
            {
                valid = false;
                ErrHandler(ErrPret, Pret, "Introduceti un pret valid pozitiv sau 0!");
            }
            else ResetErr(ErrPret, Pret);

            //rate jocului
            if (string.IsNullOrWhiteSpace(Rate.Text))
            {
                valid = false;
                ErrHandler(ErrRate, Rate, "Introduceti o rata!");
            }
            else if (!double.TryParse(Rate.Text, CultureInfo.InvariantCulture, out double rateD) || rateD < Joc.RATE_MIN || rateD > Joc.RATE_MAX)
            {
                valid = false;
                ErrHandler(ErrRate, Rate, $"Introduceti o rata intre {Joc.RATE_MIN}-{Joc.RATE_MAX}!");
            }
            else ResetErr(ErrRate, Rate);

            //genre
            if (!string.IsNullOrWhiteSpace(Genre.Text))
            {
                string[] genreArr = Genre.Text.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                HashSet<string> unic = new HashSet<string>();

                if (genreArr.Length == 0)
                {
                    valid = false;
                    ErrHandler(ErrGenre, Genre, "Nu ati introdus genre valide!");
                }
                else if (genreArr.Any(gen => !unic.Add(gen.ToLower())))
                {
                    valid = false;
                    ErrHandler(ErrGenre, Genre, "Introduceti genre unice!");
                }
                else ResetErr(ErrGenre, Genre);

            }
            else
            {
                valid = false;
                ErrHandler(ErrGenre, Genre, "Introduceti minim un genru!");
            }

            //editori
            if (string.IsNullOrWhiteSpace(Editori.Text))
            {
                valid = false;
                ErrHandler(ErrEditori, Editori, "Introduceti macar un editor!");
            }
            else ResetErr(ErrEditori, Editori);

            //dezvoltatori
            if (string.IsNullOrWhiteSpace(Dezvoltatori.Text))
            {
                valid = false;
                ErrHandler(ErrDezvoltatori, Dezvoltatori, "Introduceti macar un dezvoltator!");
            }
            else ResetErr(ErrDezvoltatori, Dezvoltatori);

            //releaseData
            if (ReleaseDataWPF == DateTime.MinValue)
            {
                valid = false;
                ErrHandler(ErrReleaseData, ReleaseData, "Introduceti un an!");
            }
            else ResetErr(ErrReleaseData, ReleaseData);

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
                    ErrHandler(ErrDenumirea, Denumirea, "Introduceti o denumire!");
                }
                else ResetErr(ErrDenumirea, Denumirea);

                //platforme
                int selectat = 0;
                foreach (var elem in PlatformeWPFCmb)
                {
                    if (elem is CheckBox checkboxul && checkboxul.IsChecked == true) selectat++;
                }
                if (selectat == 0)
                {
                    valid = false;
                    ErrHandler(ErrPlatforme, Platforme, "Selectati macar o optiune valida!");
                }
                else ResetErr(ErrPlatforme, Platforme);

                //varsta
                if (!Enum.IsDefined(typeof(RatingVarsta), varstaWPFCmb))
                {
                    valid = false;
                    ErrHandler(ErrVarsta, Varsta, "Selectati o varsta!");
                }
                else ResetErr(ErrVarsta, Varsta);

            }

            //pentru modificare
            if (Denumirea is ComboBox cmbD && Platforme is ListBox lbP && Varsta is ListBox lbV)
            {
                //denumirea
                if (cmbD.SelectedItem == null)
                {
                    valid = false;
                    ErrHandler(ErrDenumirea, Denumirea, "Selectati macar o optiune valida!");
                }
                else ResetErr(ErrDenumirea, Denumirea);

                //platforme
                if (lbP.SelectedItems == null || lbP.SelectedItems.Count == 0)
                {
                    valid = false;
                    ErrHandler(ErrPlatforme, Platforme, "Selectati macar o optiune valida!");
                }
                else ResetErr(ErrPlatforme, Platforme);

                //varsta
                if (lbV.SelectedItem == null || !Enum.IsDefined(typeof(RatingVarsta), lbV.SelectedItem))
                {
                    valid = false;
                    ErrHandler(ErrVarsta, Varsta, "Selectati macar o optiune valida!");
                }
                else ResetErr(ErrVarsta, Varsta);
            }

            return valid;
        }
        private static void ErrHandler(TextBlock ErrTextBlock, Control TipObj, string Mesaj)
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
