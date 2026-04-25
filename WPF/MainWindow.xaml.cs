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
            AfisareJocuri();
        }

        private void AfisareJocuri()
        {
            Jocuri = AdministrJocuri.GetJocuri();
            dgJocuri.ItemsSource = Jocuri.Where(j => !j.EsteSters);
        }

        //click metode pentru butoane
        private void AdministrJocuriClick(object sender, RoutedEventArgs e)
        {
            CautareJoculPanel.Visibility = Visibility.Collapsed;
            AdministJocPanel.Visibility = Visibility.Visible;
            dgJocuri.Visibility = Visibility.Visible;
            StergeInput();
        }
        private void CautareJocClick(object sender, RoutedEventArgs e)
        {
            AdministJocPanel.Visibility = Visibility.Collapsed;
            dgJocuri.Visibility= Visibility.Collapsed;
            CautareJoculPanel.Visibility = Visibility.Visible;
        }

        private void AdaugaJocClick(object sender, RoutedEventArgs e)
        {
            //valideaza input a userului
            if (!Validator.ValidareJocInput(Denumirea, Pret, Rate, Genre, SelectatorPlatforme, Editori, Dezvoltatori, SelectatorVarsta, Anul,
                                           ErrDenumirea, ErrPret, ErrRate, ErrGenre, ErrPlatforme, ErrEditori, ErrDezvoltatori, ErrVarsta, ErrAnul))
            {
                DescJocuri.Visibility = Visibility.Collapsed;
                return;
            }


            //transform date raw in cele bune pentru obj Joc
            double.TryParse(Pret.Text, out double PretForm);

            List<string> GenreForm = Genre.Text.Split(",", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToList();

            PlatformeDisponibile Platformele = 0;
            foreach (var elem in SelectatorPlatforme.Items)
            {
                if (elem is CheckBox checboxul && 
                    checboxul.IsChecked == true && 
                    Enum.TryParse(typeof(PlatformeDisponibile), checboxul.Content.ToString(), out object pltf))
                {
                        Platformele |= (PlatformeDisponibile)pltf;
                }
            }

            List<string> EditoriForm = Editori.Text.Split(",").Select(editor => editor.Trim()).ToList();

            List<string> DezvoltatoriForm = Dezvoltatori.Text.Split(",").Select(dezvoltator => dezvoltator.Trim()).ToList();

            double.TryParse(Rate.Text, out double RataForm);

            if (Enum.TryParse(typeof(RatingVarsta), SelectatorVarsta.Text, out object vrst));
            RatingVarsta VarstaForm = (RatingVarsta)vrst;

            int.TryParse(Anul.Text, out int AnulI);
 
            //adaug joc
            AdministrJocuri.AddJoc(new Joc(Denumirea.Text, PretForm, GenreForm, Platformele, EditoriForm, DezvoltatoriForm, RataForm, VarstaForm, AnulI));

            //mesaj de success
            Rezultat.Visibility = Visibility.Visible;
            Rezultat.Text = "Joaca a fost adaugata cu succes!";
            Rezultat.Foreground = Brushes.White;
            Rezultat.Background = Brushes.DarkGreen;
            DescJocuri.Visibility = Visibility.Collapsed;

            //afisam joc adaugat in datagrid
            AfisareJocuri();

            //sterg old input
            StergeInput();
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

            AfisareJocuri();
        }

        //private void AfiseaUltJocClick(object sender, RoutedEventArgs e)
        //{
        //    Jocuri = AdministrJocuri.GetJocuri();
        //    Joc Jocul = Jocuri.LastOrDefault(j => !j.EsteSters);

        //    if (Jocul == null)
        //    {
        //        Rezultat.Text = "Nu exista nici o joaca!";
        //        Rezultat.Foreground = Brushes.White;
        //        Rezultat.Background = Brushes.Crimson;
        //        DescJocuri.Visibility = Visibility.Collapsed;
        //        return;
        //    }
        //    DescJocSing.Text = Jocul.GetInfo(true);
        //    DescJocuri.Visibility = Visibility.Visible;

        //    Rezultat.Visibility = Visibility.Collapsed;
        //    return;
        //}

        private void AfisareaJocCautatClick(object sender, RoutedEventArgs e)
        {
            Joc joc = AdministrJocuri.GetJoc(CautareBox.Text);
        }

        //metoda de a sterge input-ul vechi
        private void StergeInput()
        {
            Denumirea.Text = "";
            Pret.Text = "";
            Rate.Text = "";
            Genre.Text = "";
            SelectatorPlatforme.SelectedIndex = 0;
            foreach (var elem in SelectatorPlatforme.Items)
            {
                if (elem is CheckBox ch) ch.IsChecked = false;
            }
            Editori.Text = "";
            Dezvoltatori.Text = "";
            SelectatorVarsta.SelectedIndex = 0;
        }
    }
}