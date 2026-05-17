using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using StocareJocurilor;
using CatalogDeJocuri;
using DespreJoc;
using DespreJoc.Enums;
using WPF.Validatori;
using System.Globalization;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        IStocare AdministrJocuri = Decident.PrelucrareaDatelor();
        List<Joc> Jocuri;
        private Joc _JocCurent;
        public Joc JocCurent
        {
            get => _JocCurent;
            set
            {
                _JocCurent = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyDen = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyDen));
        }

        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();
            AfisareJocuri();
        }

        private void AfisareJocuri()
        {
            Jocuri = AdministrJocuri.GetJocuri();
            dgJocuri.ItemsSource = Jocuri;
        }
        private void dgJocuriSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(ModificareMeniuPanel.Visibility == Visibility.Visible)
            {
                JocCurent = dgJocuri.SelectedItem as Joc;
                if (JocCurent == null) return;

                foreach(PlatformeDisponibile pltf in Enum.GetValues(typeof(PlatformeDisponibile)))
                    if (JocCurent.Platforme.HasFlag(pltf))
                        lstModifJoacaPlatformeListBox.SelectedItems.Add(pltf);

                InputModificare(true);
            }
        }

        //click metode pentru butoane
        private void btnMeniuAdministrJocuri_Click(object sender, RoutedEventArgs e)
        {
            CautareJoculPanel.Visibility = Visibility.Collapsed;
            AdministJocPanel.Visibility = Visibility.Visible;
            dgJocuri.Visibility = Visibility.Visible;
            ModificareMeniuPanel.Visibility = Visibility.Collapsed;
            Rezultat.Visibility = Visibility.Collapsed;
            AfisareJocuri();
            StergeInputAdaugare();
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
            dgJocuri.Visibility = Visibility.Visible;
            dgJocuriGasiti.Visibility = Visibility.Collapsed;
            CautareJoculPanel.Visibility = Visibility.Collapsed;
            ModificareMeniuPanel.Visibility = Visibility.Visible;

            lstModifJoacaVarstaListBox.ItemsSource = Enum.GetValues(typeof(RatingVarsta));
            lstModifJoacaPlatformeListBox.ItemsSource = Enum.GetValues(typeof(PlatformeDisponibile));
            btnRezultatModif.Visibility = Visibility.Collapsed;

            AfisareJocuri();
            StergeInputModificare();
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
            StergeInputAdaugare();
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

            List<Joc> joculGasit = AdministrJocuri.GetJocuriCautare(categoria, criteriu);

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
            if (!JocInput.ValidareJocInput(ModifJoacaDenumireaTextBox, ModifJoacaPretTextBox, ModifJoacaRateTextBox, ModifJoacaGenreTextBox, lstModifJoacaPlatformeListBox, ModifJoacaEditoriTextBox, ModifJoacaDezvoltatoriTextBox, lstModifJoacaVarstaListBox, ModifJoacaReleaseData,
                               ErrModifDenumirea, ErrModifPret, ErrModifRate, ErrModifGenre, ErrModifPlatforme, ErrModifEditori, ErrModifDezvoltatori, ErrModifRatingVarsta, ErrModifReleaseData))
            {
                btnRezultatModif.Text = "Nu sa produs modificarea!";
                btnRezultatModif.Foreground = Brushes.Crimson;
                btnRezultatModif.Visibility = Visibility.Visible;
                return; 
            }

            JocCurent.Genre = ModifJoacaGenreTextBox.Text.Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
            JocCurent.Dezvoltatori = ModifJoacaDezvoltatoriTextBox.Text.Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
            JocCurent.Editori = ModifJoacaEditoriTextBox.Text.Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();

            PlatformeDisponibile platformeModif = 0;
            foreach (var obj in lstModifJoacaPlatformeListBox.SelectedItems)
            {
                if (Enum.TryParse(typeof(PlatformeDisponibile), obj.ToString(), true, out object? pltfMod))
                    platformeModif |= (PlatformeDisponibile)pltfMod;
            }

            JocCurent.Platforme = platformeModif;
            AdministrJocuri.UpdateJoc(JocCurent);

            btnRezultatModif.Text = "Joaca a fost cu succes modificata!";
            btnRezultatModif.Foreground = Brushes.Green;
            btnRezultatModif.Visibility = Visibility.Visible;

            StergeInputModificare();
            AfisareJocuri();
            InputModificare(false);
        }

        //metoda de a sterge input-uri vechi
        private void StergeInputAdaugare()
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
        private void StergeInputModificare()
        {
            ModifJoacaPretTextBox.Text                 = "";
            ModifJoacaDenumireaTextBox.Text            = "";
            ModifJoacaDezvoltatoriTextBox.Text         = "";
            ModifJoacaEditoriTextBox.Text              = "";
            ModifJoacaGenreTextBox.Text                = "";
            ModifJoacaRateTextBox.Text                 = "";
            ModifJoacaReleaseData.SelectedDate         = null;
            lstModifJoacaVarstaListBox.SelectedItem    = null;
            ModifJoacaEsteDispobinilCheckBox.IsChecked = false;
            lstModifJoacaPlatformeListBox.SelectedItems.Clear();
        }
        private void InputModificare(bool aprins)
        {
            if(aprins)
            {
                btnModificaXAML.IsEnabled                  = true;
                ModifJoacaPretTextBox.IsEnabled            = true;
                ModifJoacaDenumireaTextBox.IsEnabled       = true;
                ModifJoacaDezvoltatoriTextBox.IsEnabled    = true;
                ModifJoacaEditoriTextBox.IsEnabled         = true;
                ModifJoacaGenreTextBox.IsEnabled           = true;
                ModifJoacaRateTextBox.IsEnabled            = true;
                ModifJoacaReleaseData.IsEnabled            = true;
                lstModifJoacaVarstaListBox.IsEnabled       = true;
                lstModifJoacaPlatformeListBox.IsEnabled    = true;
                ModifJoacaEsteDispobinilCheckBox.IsEnabled = true;
            }
            else
            {
                btnModificaXAML.IsEnabled                  = false;
                ModifJoacaPretTextBox.IsEnabled            = false;
                ModifJoacaDenumireaTextBox.IsEnabled       = false;
                ModifJoacaDezvoltatoriTextBox.IsEnabled    = false;
                ModifJoacaEditoriTextBox.IsEnabled         = false;
                ModifJoacaGenreTextBox.IsEnabled           = false;
                ModifJoacaRateTextBox.IsEnabled            = false;
                ModifJoacaReleaseData.IsEnabled            = false;
                lstModifJoacaVarstaListBox.IsEnabled       = false;
                lstModifJoacaPlatformeListBox.IsEnabled    = false;
                ModifJoacaEsteDispobinilCheckBox.IsEnabled = false;
            }
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