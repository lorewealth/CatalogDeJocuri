using System;
using System.Collections.Generic;
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
        public static bool ValidareJocInput(TextBox Denumirea, TextBox Pret, TextBox Rate, TextBox Genre, ComboBox Platforme, TextBox Editori, TextBox Dezvoltatori, ComboBox Varsta, TextBox Anul,
                                            TextBlock ErrDenumirea, TextBlock ErrPret, TextBlock ErrRate, TextBlock ErrGenre, TextBlock ErrPlatforme, TextBlock ErrEditori, TextBlock ErrDezvoltatori, TextBlock ErrVarsta, TextBlock ErrAnul)
        {
            //variabile
            string denumireaWPF = Denumirea.Text;
            string pretWPF = Pret.Text;
            string rateWPF = Rate.Text;
            string genreWPF = Genre.Text;
            ItemCollection PlatformeWPF = Platforme.Items;
            string editoriWPF = Editori.Text;
            string dezvoltatoriWPF = Dezvoltatori.Text;
            string varstaWPF = Varsta.Text;
            string AnulWPF = Anul.Text;

            bool valid = true;

            //denumirea
            if (string.IsNullOrWhiteSpace(denumireaWPF))
            {
                valid = false;
                ErrHandler(ErrDenumirea, Denumirea, "Introduceti o denumire!");
            }
            else ResetErr(ErrDenumirea, Denumirea);

            //pret
            if (string.IsNullOrWhiteSpace(pretWPF))
            {
                valid = false;
                ErrHandler(ErrPret, Pret, "Introduceti un pret!");
            }
            else if (!double.TryParse(pretWPF, out var pretD) || pretD < 0)
            {
                valid = false;
                ErrHandler(ErrPret, Pret, "Introduceti un pret valid pozitiv sau 0!");
            }
            else ResetErr(ErrPret, Pret);

            //rate jocului
            if (string.IsNullOrWhiteSpace(rateWPF))
            {
                valid = false;
                ErrHandler(ErrRate, Rate, "Introduceti o rata!");
            }
            else if (!double.TryParse(rateWPF, out double rateD) || rateD < Joc.RATE_MIN || rateD > Joc.RATE_MAX)
            {
                valid = false;
                ErrHandler(ErrRate, Rate, $"Introduceti o rata intre {Joc.RATE_MIN}-{Joc.RATE_MAX}!");
            }
            else ResetErr(ErrRate, Rate);

            //genre
            if (!string.IsNullOrWhiteSpace(genreWPF))
            {
                string[] genreArr = genreWPF.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
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

            //platforme
            int selectat = 0;
            foreach (var elem in PlatformeWPF)
            {
                if (elem is CheckBox checkboxul && checkboxul.IsChecked == true) selectat++;
            }
            if (selectat == 0)
            {
                valid = false;
                ErrHandler(ErrPlatforme, Platforme, "Selectati macar o optiune valida!");
            }
            else ResetErr(ErrPlatforme, Platforme);

            //editori
            if (string.IsNullOrWhiteSpace(editoriWPF))
            {
                valid = false;
                ErrHandler(ErrEditori, Editori, "Introduceti macar un editor!");
            }
            else ResetErr(ErrEditori, Editori);

            //dezvoltatori
            if (string.IsNullOrWhiteSpace(dezvoltatoriWPF))
            {
                valid = false;
                ErrHandler(ErrDezvoltatori, Dezvoltatori, "Introduceti macar un dezvoltator!");
            }
            else ResetErr(ErrDezvoltatori, Dezvoltatori);

            //varsta
            if (!Enum.IsDefined(typeof(RatingVarsta), varstaWPF))
            {
                valid = false;
                ErrHandler(ErrVarsta, Varsta, "Selectati o varsta!");
            }
            else ResetErr(ErrVarsta, Varsta);

            //anul
            if (string.IsNullOrWhiteSpace(AnulWPF))
            {
                valid = false;
                ErrHandler(ErrAnul, Anul, "Introduceti un an!");
            }
            else if (!int.TryParse(AnulWPF, out int anulI) || anulI < Joc.ANUL_MIN || anulI > Joc.ANUL_MAX)
            {
                valid = false;
                ErrHandler(ErrAnul, Anul, $"Introduceti un an valid intre {Joc.ANUL_MIN} - {Joc.ANUL_MAX}");
            }
            else ResetErr(ErrAnul, Anul);

            return valid;

        }
        private static void ErrHandler(TextBlock ErrTextBlock, Control TipObj, string Mesaj)
        {
            if (TipObj is TextBox)
            {
                ErrTextBlock.Visibility = Visibility.Visible;
                ErrTextBlock.Text = Mesaj;
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
