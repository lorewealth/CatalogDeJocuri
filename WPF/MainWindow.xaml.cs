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
using StocareJocurilor;
using CatalogDeJocuri;
using DespreJoc;
using EnumGestionare;

namespace WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IStocare AdministrJocuri = Decident.PrelucrareaDatelor();
        List<Joc> Jocuri;

        public MainWindow()
        {
            InitializeComponent();
        }
        private void AdaugaJocClick(object sender, RoutedEventArgs e)
        {
            string DenumireaWPF = Denumirea.Text;
            string PretWPF = Pret.Text;
            string RataWPF = Rate.Text;
            string GenreWPF = Genre.Text;
            ItemCollection PlatformeWPF = SelectatorPlatforme.Items;
            string EditoriWPF = Editori.Text;
            string DezvolatoriWPF = Dezvoltatori.Text;
            string VarstaWPF = SelectatorVarsta.Text;

            bool valid = true;

            if (Validari.ValidareDenumire(DenumireaWPF) != ErrCod.OK)
            {
                ErrDenumirea.Visibility = Visibility.Visible;
                Denumirea.BorderBrush = new SolidColorBrush(Colors.Red);
                valid = false;
            }
            else
            {
                ErrDenumirea.Visibility = Visibility.Collapsed;
                Denumirea.BorderBrush = new SolidColorBrush(Color.FromRgb(221, 216, 208));
            }

            if (Validari.ValidarePret(PretWPF) != ErrCod.OK)
            {
                ErrPret.Visibility = Visibility.Visible;
                Pret.BorderBrush = new SolidColorBrush(Colors.Red);
                valid = false;
            }
            else
            {
                ErrPret.Visibility = Visibility.Collapsed;
                Pret.BorderBrush = new SolidColorBrush(Color.FromRgb(221, 216, 208));
            }

            if (Validari.ValidareGenre(GenreWPF) != ErrCod.OK)
            {
                ErrGenre.Visibility = Visibility.Visible;
                Genre.BorderBrush = new SolidColorBrush(Colors.Red);
                valid = false;
            }
            else
            {
                ErrGenre.Visibility = Visibility.Collapsed;
                Genre.BorderBrush = new SolidColorBrush(Color.FromRgb(221, 216, 208));
            }

            if (Validari.ValidarePlatforma(PlatformeWPF) != ErrCod.OK)
            {
                ErrPlatforme.Visibility = Visibility.Visible;
                valid = false;
            }
            else
            {
                ErrPlatforme.Visibility = Visibility.Collapsed;
            }

            if (Validari.ValidareRate(RataWPF) != ErrCod.OK)
            {
                ErrRate.Visibility = Visibility.Visible;
                Rate.BorderBrush = new SolidColorBrush(Colors.Red);
                valid = false;
            }
            else
            {
                ErrRate.Visibility = Visibility.Collapsed;
                Rate.BorderBrush = new SolidColorBrush(Color.FromRgb(221, 216, 208));
            }

            if (Validari.ValidareEditori(EditoriWPF) != ErrCod.OK)
            {
                ErrEditori.Visibility = Visibility.Visible;
                Editori.BorderBrush = new SolidColorBrush(Colors.Red);
                valid = false;
            }
            else
            {
                ErrEditori.Visibility = Visibility.Collapsed;
                Editori.BorderBrush = new SolidColorBrush(Color.FromRgb(221, 216, 208));
            }

            if (Validari.ValidareDezvoltatori(DezvolatoriWPF) != ErrCod.OK)
            {
                ErrDezvoltatori.Visibility = Visibility.Visible;
                Dezvoltatori.BorderBrush = new SolidColorBrush(Colors.Red);
                valid = false;
            }
            else
            {
                ErrDezvoltatori.Visibility = Visibility.Collapsed;
                Dezvoltatori.BorderBrush = new SolidColorBrush(Color.FromRgb(221, 216, 208));
            }

            if (Validari.ValidareVarsta(VarstaWPF) != ErrCod.OK)
            {
                ErrVarsta.Visibility = Visibility.Visible;
                valid = false;
            }
            else
            {
                ErrVarsta.Visibility = Visibility.Collapsed;
            }

            if (!valid)
            {
                Rezultat.Visibility = Visibility.Visible;
                Rezultat.Text = "Nu ati completat campuri necesare!";
                Rezultat.Foreground = Brushes.White;
                Rezultat.Background = Brushes.Crimson;
                DescJocuri.Visibility = Visibility.Collapsed;

                return;
            }
            double.TryParse(PretWPF, out double PretForm);

            List<string> GenreForm = GenreWPF.Split(",").Select(genrul => genrul.Trim()).ToList();

            PlatformeDisponibile Platformele = 0;
            foreach (var elem in PlatformeWPF)
            {
                if (elem is CheckBox checboxul && 
                    checboxul.IsChecked == true && 
                    Enum.TryParse(typeof(PlatformeDisponibile), checboxul.Content.ToString(), out object pltf))
                {
                        Platformele |= (PlatformeDisponibile)pltf;
                }
            }

            List<string> EditoriForm = EditoriWPF.Split(",").Select(editor => editor.Trim()).ToList();

            List<string> DezvoltatoriForm = DezvolatoriWPF.Split(",").Select(dezvoltator => dezvoltator.Trim()).ToList();

            double.TryParse(RataWPF, out double RataForm);

            if (Enum.TryParse(typeof(RatingVarsta), VarstaWPF, out object vrst));
            RatingVarsta VarstaForm = (RatingVarsta)vrst; 
 
            AdministrJocuri.AddJoc(new Joc(DenumireaWPF, PretForm, GenreForm, Platformele, EditoriForm, DezvoltatoriForm, RataForm, VarstaForm));

            Rezultat.Visibility = Visibility.Visible;
            Rezultat.Text = "Joaca a fost adaugata cu succes!";
            Rezultat.Foreground = Brushes.White;
            Rezultat.Background = Brushes.DarkGreen;
            DescJocuri.Visibility = Visibility.Collapsed;

            Denumirea.Text = "";
            Pret.Text = "";
            Rate.Text = "";
            Genre.Text = "";
            SelectatorPlatforme.SelectedIndex = 0;
            foreach(var elem in SelectatorPlatforme.Items)
            {
                if (elem is CheckBox ch) ch.IsChecked = false;
            }
            Editori.Text = "";
            Dezvoltatori.Text = "";
            SelectatorVarsta.SelectedIndex = 0;

        }

        private void StergeUltJocClick(object sender, RoutedEventArgs e)
        {
            if (!AdministrJocuri.RemoveUltJoc())
            {
                Rezultat.Text = "Nu exista nici o joaca!";
                Rezultat.Foreground = Brushes.White;
                Rezultat.Background = Brushes.Crimson;
                DescJocuri.Visibility = Visibility.Collapsed;
                return;
            }
            Jocuri = AdministrJocuri.GetJocuri();

            Rezultat.Visibility = Visibility.Visible;
            Rezultat.Text = "Joaca a fost sters cu succes!";
            Rezultat.Foreground = Brushes.White;
            Rezultat.Background = Brushes.DarkGreen;
            DescJocuri.Visibility = Visibility.Collapsed;
        }

        private void AfiseaUltJocClick(object sender, RoutedEventArgs e)
        {
            Jocuri = AdministrJocuri.GetJocuri();
            Joc Jocul = Jocuri.LastOrDefault();

            if (Jocul == null)
            {
                Rezultat.Text = "Nu exista nici o joaca!";
                Rezultat.Foreground = Brushes.White;
                Rezultat.Background = Brushes.Crimson;
                DescJocuri.Visibility = Visibility.Collapsed;
                return;
            }
            DescJocSing.Text = Jocul.GetInfo();
            DescJocuri.Visibility = Visibility.Visible;

            Rezultat.Visibility = Visibility.Collapsed;
            return;

        }
    }
}