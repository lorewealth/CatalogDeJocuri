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
        private void btnMeniuAdministrJocuri_Click(object sender, RoutedEventArgs e)
        {
            CautareJoculPanel.Visibility = Visibility.Collapsed;
            AdministJocPanel.Visibility = Visibility.Visible;
            dgJocuri.Visibility = Visibility.Visible;
            ModificareMeniuPanel.Visibility = Visibility.Collapsed;
            AfisareJocuri();
            Rezultat.Visibility = Visibility.Collapsed;
            StergeInput();
        }
        private void btnMeniuCautareJocuri_Click(object sender, RoutedEventArgs e)
        {
            AdministJocPanel.Visibility = Visibility.Collapsed;
            dgJocuri.Visibility= Visibility.Collapsed;
            ModificareMeniuPanel.Visibility = Visibility.Collapsed;
            CautareJoculPanel.Visibility = Visibility.Visible;
        }

        private void btnMeniuModificareJoc_Click(object sender, RoutedEventArgs e)
        {
            AdministJocPanel.Visibility = Visibility.Collapsed;
            dgJocuri.Visibility = Visibility.Collapsed;
            dgJocuriGasiti.Visibility = Visibility.Collapsed;
            CautareJoculPanel.Visibility = Visibility.Collapsed;
            ModificareMeniuPanel.Visibility = Visibility.Visible;

            lstModificareJoacaComboBox.ItemsSource = Jocuri;
            lstModifJoacaVarstaListBox.ItemsSource = Enum.GetValues(typeof(RatingVarsta));
            lstModifJoacaPlatformeListBox.ItemsSource = Enum.GetValues(typeof(PlatformeDisponibile));
        }

        private void btnAdaugaJoc_Click(object sender, RoutedEventArgs e)
        {
            //valideaza input a userului
            if (!JocInput.ValidareJocInput(Denumirea, Pret, Rate, Genre, SelectatorPlatforme, Editori, Dezvoltatori, SelectatorVarsta, ReleaseData,
                                           ErrDenumirea, ErrPret, ErrRate, ErrGenre, ErrPlatforme, ErrEditori, ErrDezvoltatori, ErrVarsta, ErrReleaseData))
            {
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

            bool eDisponibil = false;
            if (eDisponibilCheckBox.IsChecked == true) eDisponibil = true;

            DateTime ReleaseDataForm = ReleaseData.SelectedDate ?? DateTime.Today;

            //adaug joc
            AdministrJocuri.AddJoc(new Joc(Denumirea.Text, PretForm, GenreForm, Platformele, EditoriForm, DezvoltatoriForm, RataForm, VarstaForm, ReleaseDataForm, eDisponibil));

            //mesaj de success
            Rezultat.Visibility = Visibility.Visible;
            Rezultat.Text = "Joaca a fost adaugata cu succes!";
            Rezultat.Foreground = Brushes.White;
            Rezultat.Background = Brushes.DarkGreen;

            //afisam joc adaugat in datagrid
            AfisareJocuri();

            //sterg old input
            StergeInput();
        }

        private void btnStergeUltJoc_Click(object sender, RoutedEventArgs e)
        {
            if (!AdministrJocuri.RemoveUltJoc())
            {
                Rezultat.Text = "Nu exista nici o joaca!";
                Rezultat.Foreground = Brushes.White;
                Rezultat.Background = Brushes.Crimson;
                return;
            }
            Jocuri = AdministrJocuri.GetJocuri();

            Rezultat.Visibility     = Visibility.Visible;
            Rezultat.Text           = "Joaca a fost sters cu succes!";
            Rezultat.Foreground     = Brushes.White;
            Rezultat.Background     = Brushes.DarkGreen;

            AfisareJocuri();
        }

        private void btnAfisareaJocCautat_Click(object sender, RoutedEventArgs e)
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
                case "releasedata":
                    string[] strArrPRA = criteriu.Split('-', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                    if(categoria == "releasedata")
                    {
                        //---temporar
                        // Concatenez la primul si al doilea element(care sunt anii) luna, ziua si ora,
                        // pentru a putea compara cu data selectata
                        //---
                        DateTime inceputT = Convert.ToDateTime($"{strArrPRA[0]}.01.01");
                        DateTime sfarsitT = Convert.ToDateTime($"{strArrPRA[1]}.01.01");

                        if (inceputT > sfarsitT)
                            (inceputT, sfarsitT) = (sfarsitT, inceputT);

                        joculGasit = Jocuri.Where(joc => joc.ReleaseData >= inceputT && joc.ReleaseData <= sfarsitT).ToList();
                        break;
                    }
                    double inceput = Convert.ToDouble(strArrPRA[0]);
                    double sfarsit = Convert.ToDouble(strArrPRA[1]);

                    if (inceput > sfarsit) 
                        (inceput, sfarsit) = (sfarsit, inceput);

                    if (categoria == "pret") joculGasit = Jocuri.Where(joc => joc.Pret >= inceput && joc.Pret <= sfarsit).ToList();
                    if (categoria == "rate") joculGasit = Jocuri.Where(joc => joc.Rate >= inceput && joc.Rate <= sfarsit).ToList();

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

        private void btnModificareaJocului_Click(object sender, RoutedEventArgs e)
        {
            if (!JocInput.ValidareJocInput(lstModificareJoacaComboBox, ModifJoacaPretTextBox, ModifJoacaRateTextBox, ModifJoacaGenreTextBox, lstModifJoacaPlatformeListBox, ModifJoacaEditoriTextBox, ModifJoacaDezvoltatoriTextBox, lstModifJoacaVarstaListBox, ModifJoacaReleaseData,
                               ErrModifSelectJoaca, ErrModifPret, ErrModifRate, ErrModifGenre, ErrModifPlatforme, ErrModifEditori, ErrModifDezvoltatori, ErrModifRatingVarsta, ErrModifReleaseData))
            {
                return; 
            }

            //AdministrJocuri.UpdateJoc(new Joc());

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
            ReleaseData.SelectedDate = null;
            eDisponibilCheckBox.IsChecked = false;
        }

        //metoda de a prelua de la radiobuton continut
        private string CautareRadButton() 
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
            if (ReleaseDataRadButton.IsChecked  == true)         return ReleaseDataRadButton.Content.ToString();

            else return DenumireaRadButton.Content.ToString();
        }
    }
}