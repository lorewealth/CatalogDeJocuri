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
using DespreJoc.Enums;
using WPF.Validatori;

namespace WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IStocare AdministrJocuri = Decident.PrelucrareaDatelor();
        List<Joc> Jocuri;
        protected const int RATE_MIN = 1;
        protected const int RATE_MAX = 10;


        public MainWindow()
        {
            InitializeComponent();
            AfisareJocuri();
        }

        private void AfisareJocuri()
        {
            Jocuri = AdministrJocuri.GetJocuri();
            dgJocuri.ItemsSource = Jocuri;
        }

        //click metode pentru butoane
        private void AdministrJocuriClick(object sender, RoutedEventArgs e)
        {
            CautareJoculPanel.Visibility = Visibility.Collapsed;
            AdministJocPanel.Visibility = Visibility.Visible;
            dgJocuri.Visibility = Visibility.Visible;
            AfisareJocuri();
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
            if (!JocInput.ValidareJocInput(Denumirea, Pret, Rate, Genre, SelectatorPlatforme, Editori, Dezvoltatori, SelectatorVarsta, Anul,
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

            bool eDisponibil = false;
            if (eDisponibilCheckBox.IsChecked == true) eDisponibil = true;

 
            //adaug joc
            AdministrJocuri.AddJoc(new Joc(Denumirea.Text, PretForm, GenreForm, Platformele, EditoriForm, DezvoltatoriForm, RataForm, VarstaForm, AnulI, eDisponibil));

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

            Rezultat.Visibility     = Visibility.Visible;
            Rezultat.Text           = "Joaca a fost sters cu succes!";
            Rezultat.Foreground     = Brushes.White;
            Rezultat.Background     = Brushes.DarkGreen;
            DescJocuri.Visibility   = Visibility.Collapsed;

            AfisareJocuri();
        }

        private void AfisareaJocCautatClick(object sender, RoutedEventArgs e)
        {
            string categoria = CautareRadButton().ToLower();
            string criteriu = CautareBox.Text;

            if (!Cautare.CautareInputValidator(categoria, criteriu))
            {
                ErrCautareBox.Text = $"Introduceti macar un criteriu valid!";
                ErrCautareBox.Visibility = Visibility.Visible;
                dgJocuriGasiti.Visibility = Visibility.Collapsed;
                return;
            }
            ErrCautareBox.Visibility = Visibility.Collapsed;

            List<Joc> joculGasit = [];

            switch (categoria)
            {
                case "denumirea":
                case "genre":
                case "platforme":
                case "dezvoltatori":
                case "editori":
                case "varsta":
                    string[] strArrRest = criteriu.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                    foreach(string str in strArrRest)
                    {
                        if (categoria == "denumirea") joculGasit = Jocuri.Where(joc => joc.Denumirea.Equals(str, StringComparison.OrdinalIgnoreCase)).ToList();
                        if (categoria == "genre") joculGasit = Jocuri.Where(joc => joc.Genre.Any(genrul => genrul.Equals(str, StringComparison.OrdinalIgnoreCase))).ToList();
                        if (categoria == "platforme" && Enum.TryParse<PlatformeDisponibile>(str, true, out PlatformeDisponibile res))
                        {
                            joculGasit = Jocuri.Where(joc => joc.Platforme.HasFlag(res)).ToList();
                        }
                        if (categoria == "editori") joculGasit = Jocuri.Where(joc => joc.Editori.Any(editor => editor.Equals(str, StringComparison.OrdinalIgnoreCase))).ToList();
                        if (categoria == "dezvoltatori") joculGasit = Jocuri.Where(joc => joc.Dezvoltatori.Any(dezvoltator => dezvoltator.Equals(str, StringComparison.OrdinalIgnoreCase))).ToList();
                        if (categoria == "varsta") joculGasit = Jocuri.Where(joc => joc.Varsta.ToString().Equals(str, StringComparison.OrdinalIgnoreCase)).ToList();
                    }

                    break;
                case "pret":
                case "rate":
                case "anul":
                    string[] strArrPRA = criteriu.Split('-', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    double inceput = Convert.ToDouble(strArrPRA[0]);
                    double sfarsit = Convert.ToDouble(strArrPRA[1]);

                    if (inceput > sfarsit) 
                        (inceput, sfarsit) = (sfarsit, inceput);

                    if (categoria == "pret") joculGasit = Jocuri.Where(joc => joc.Pret >= inceput && joc.Pret <= sfarsit).ToList();
                    if (categoria == "rate") joculGasit = Jocuri.Where(joc => joc.Rate >= inceput && joc.Rate <= sfarsit).ToList();
                    if (categoria == "anul") joculGasit = Jocuri.Where(joc => joc.Anul >= inceput && joc.Anul <= sfarsit).ToList();

                    break;
                default:
                    break;  
            }

            if(joculGasit.Count == 0)
            {
                ErrCautareBox.Text = $"Nu a fost gasit joaca dupa acest criteriul(ii)!";
                ErrCautareBox.Visibility = Visibility.Visible;
                dgJocuriGasiti.Visibility = Visibility.Collapsed;
                return;
            }

            dgJocuriGasiti.ItemsSource = joculGasit;
            dgJocuriGasiti.Visibility = Visibility.Visible;

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
            Anul.Text = "";
            eDisponibilCheckBox.IsChecked = false;
        }

        //metoda de a prelua de la radiobuton continut
        private string CautareRadButton () 
        {
            // imi place stilul acest de cod tbh
            if (DenumireaRadButton.IsChecked    == true)         return DenumireaRadButton.Content.ToString();
            if (PretRadButton.IsChecked         == true)         return PretRadButton.Content.ToString();
            if (RateRadButton.IsChecked         == true)         return RateRadButton.Content.ToString();
            if (GenreRadButton.IsChecked        == true)         return GenreRadButton.Content.ToString();
            if (PlatformeRadButton.IsChecked    == true)         return PlatformeRadButton.Content.ToString();
            if (EditoriRadButton.IsChecked      == true)         return EditoriRadButton.Content.ToString();
            if (DezvoltatoriRadButton.IsChecked == true)         return DezvoltatoriRadButton.Content.ToString();
            if (VarstaRadButton.IsChecked       == true)         return VarstaRadButton.Content.ToString();
            if (AnulRadButton.IsChecked         == true)         return AnulRadButton.Content.ToString();

            else return DenumireaRadButton.Content.ToString();
        }
    }
}