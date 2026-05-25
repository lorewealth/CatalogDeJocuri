using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CatalogDeJocuri;
using Cautare_API;
using DespreJoc;
using DespreJoc.Enums;
using StocareJocurilor;
using WPF.Validatori;
using static System.Net.Mime.MediaTypeNames;

namespace WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        IStocare AdministrJocuri = Decident.PrelucrareaDatelor();
        List<Joc> Jocuri;
        ObservableCollection<Editor> EditorileLST = [];
        ObservableCollection<Dezvoltator> DezvoltatoriLST = []; 
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

        static GestionareCache grCache = new();

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
            DezvEditListBoxInitializare();
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

                //pun selectat elementele de la platforme, dezvoltatori si editori a jocului deja existent
                foreach (PlatformeDisponibile pltf in Enum.GetValues(typeof(PlatformeDisponibile)))
                    if (JocCurent.Platforme.HasFlag(pltf))
                        ModifJoacaPlatformeListBox.SelectedItems.Add(pltf);

                foreach(Editor ed in JocCurent.Editori)
                {
                    Editor edi = EditorileLST.FirstOrDefault(edlst => edlst.Denumirea.Equals(ed.Denumirea, StringComparison.OrdinalIgnoreCase));
                    if (edi != null) ModifJoacaEditoriListBox.SelectedItems.Add(edi);
                }

                foreach(Dezvoltator dz in JocCurent.Dezvoltatori)
                {
                    Dezvoltator dzv = DezvoltatoriLST.FirstOrDefault(dzlst => dzlst.Denumirea.Equals(dz.Denumirea, StringComparison.OrdinalIgnoreCase));
                    if (dzv != null) ModifJoacaDezvoltatoriListBox.SelectedItems.Add(dzv);
                }

                InputModificare(true);
            }
        }

        private void DezvEditListBoxInitializare()
        {
            EditorileLST.Clear();
            DezvoltatoriLST.Clear();

            if (Jocuri.Count > 0)
            {
                var editoriUnici = Jocuri.SelectMany(joc => joc.EditoriStr.Split(", ")).Distinct();
                var dezvUnici    = Jocuri.SelectMany(joc => joc.DezvoltatoriStr.Split(", ")).Distinct();

                foreach (var editori in editoriUnici)
                    EditorileLST.Add(new Editor(editori));
                foreach (var dezv in dezvUnici)
                    DezvoltatoriLST.Add(new Dezvoltator(dezv));
            }
            AdaugaDezvoltatoriListBox.ItemsSource = DezvoltatoriLST;
            AdaugaEditoriListBox.ItemsSource = EditorileLST;
        }

        //click metode pentru butoane
        private void btnMeniuAdministrJocuri_Click(object sender, RoutedEventArgs e)
        {
            CautareJoculPanel.Visibility = Visibility.Collapsed;
            AdministJocPanel.Visibility = Visibility.Visible;
            dgJocuri.Visibility = Visibility.Visible;
            ModificareMeniuPanel.Visibility = Visibility.Collapsed;
            Rezultat.Visibility = Visibility.Collapsed;
            CautareOnlineMeniuPanel.Visibility = Visibility.Collapsed;

            AfisareJocuri();
            StergeInputAdaugare();
            DezvEditListBoxInitializare();
            ErrRezOnliCaut.Visibility = Visibility.Collapsed;
        }

        private void btnMeniuCautareJocuri_Click(object sender, RoutedEventArgs e)
        {
            AdministJocPanel.Visibility = Visibility.Collapsed;
            dgJocuri.Visibility= Visibility.Collapsed;
            ModificareMeniuPanel.Visibility = Visibility.Collapsed;
            CautareOnlineMeniuPanel.Visibility = Visibility.Collapsed;
            CautareJoculPanel.Visibility = Visibility.Visible;

            ErrRezOnliCaut.Visibility = Visibility.Collapsed;
        }

        private void btnMeniuModificareJoc_Click(object sender, RoutedEventArgs e)
        {
            AdministJocPanel.Visibility = Visibility.Collapsed;
            dgJocuri.Visibility = Visibility.Visible;
            dgJocuriGasiti.Visibility = Visibility.Collapsed;
            CautareJoculPanel.Visibility = Visibility.Collapsed;
            ModificareMeniuPanel.Visibility = Visibility.Visible;
            CautareOnlineMeniuPanel.Visibility = Visibility.Collapsed;

            ModifJoacaVarstaListBox.ItemsSource = Enum.GetValues(typeof(RatingVarsta));
            ModifJoacaPlatformeListBox.ItemsSource = Enum.GetValues(typeof(PlatformeDisponibile));
            ModifJoacaDezvoltatoriListBox.ItemsSource = DezvoltatoriLST;
            ModifJoacaEditoriListBox.ItemsSource = EditorileLST;
            btnRezultatModif.Visibility = Visibility.Collapsed;

            AfisareJocuri();
            StergeInputModificare();
            DezvEditListBoxInitializare();
            InputModificare(false);
            ErrRezOnliCaut.Visibility = Visibility.Collapsed;
        }

        private void btnMeniuCautareOnlineJoc_Click(object sender, RoutedEventArgs e)
        {
            AdministJocPanel.Visibility = Visibility.Collapsed;
            dgJocuriGasiti.Visibility = Visibility.Collapsed;
            CautareJoculPanel.Visibility = Visibility.Collapsed;
            ModificareMeniuPanel.Visibility = Visibility.Collapsed;
            CautareOnlineMeniuPanel.Visibility = Visibility.Visible;
            ResetOutputCautareOnline();
        }

        private void btnMeniuAfisareaPreturilor_Click(object sender, RoutedEventArgs e)
        {
            // de impl.
        }

        private void btnAdaugaJoc_Click(object sender, RoutedEventArgs e)
        {
            //valideaza input a userului
            if (!JocInput.ValidareJocInput(Denumirea, Pret, Rate, Genre, SelectatorPlatforme, AdaugaEditoriListBox, AdaugaDezvoltatoriListBox, SelectatorVarsta, ReleaseData,
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
                if (elem is CheckBox checboxul && checboxul.IsChecked == true) 
                {
                    string platformeText = checboxul.Tag.ToString().Trim();
                    if (!string.IsNullOrWhiteSpace(platformeText) && Enum.TryParse(typeof(PlatformeDisponibile), platformeText, out object pltf))
                        Platformele |= (PlatformeDisponibile)pltf;
                }
            }

            List<Editor> EditoriForm = AdaugaEditoriListBox.SelectedItems.Cast<Editor>().ToList();
            List<Dezvoltator> DezvoltatoriForm = AdaugaDezvoltatoriListBox.SelectedItems.Cast<Dezvoltator>().ToList();

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

            Rezultat.Visibility = Visibility.Visible;
            Rezultat.Text = "Joaca a fost sters cu succes!";
            Rezultat.Foreground = Brushes.White;
            Rezultat.Background = Brushes.DarkGreen;

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
            if (!JocInput.ValidareJocInput(ModifJoacaDenumireaTextBox, ModifJoacaPretTextBox, ModifJoacaRateTextBox, ModifJoacaGenreTextBox, ModifJoacaPlatformeListBox, ModifJoacaEditoriListBox, ModifJoacaDezvoltatoriListBox, ModifJoacaVarstaListBox, ModifJoacaReleaseDataListBox,
                               ErrModifDenumirea, ErrModifPret, ErrModifRate, ErrModifGenre, ErrModifPlatforme, ErrModifEditori, ErrModifDezvoltatori, ErrModifRatingVarsta, ErrModifReleaseData))
            {
                btnRezultatModif.Text = "Nu sa produs modificarea!";
                btnRezultatModif.Foreground = Brushes.Crimson;
                btnRezultatModif.Visibility = Visibility.Visible;
                return;
            }

            JocCurent.Genre = ModifJoacaGenreTextBox.Text.Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();

            PlatformeDisponibile platformeModif = 0;
            foreach (var obj in ModifJoacaPlatformeListBox.SelectedItems)
            {
                if (Enum.TryParse(typeof(PlatformeDisponibile), obj.ToString(), true, out object? pltfMod))
                    platformeModif |= (PlatformeDisponibile)pltfMod;
            }

            List<Editor> editori = []; 
            foreach(Editor obj in ModifJoacaEditoriListBox.SelectedItems)
                editori.Add(obj);

            List<Dezvoltator> dezvolatori = [];
            foreach(Dezvoltator obj in ModifJoacaDezvoltatoriListBox.SelectedItems)
                dezvolatori.Add(obj);

            JocCurent.Platforme = platformeModif;
            JocCurent.Dezvoltatori = dezvolatori;
            JocCurent.Editori = editori;

            AdministrJocuri.UpdateJoc(JocCurent);

            btnRezultatModif.Text = "Joaca a fost cu succes modificata!";
            btnRezultatModif.Foreground = Brushes.Green;
            btnRezultatModif.Visibility = Visibility.Visible;

            StergeInputModificare();
            AfisareJocuri();
            InputModificare(false);
            JocCurent = null;
        }

        private void btnAdaugaEditorListBox_Click(object sender, RoutedEventArgs e)
        {
            if (!AddDezvEditListBoxVal.Validare(AdaugaEditoriTextBox, ErrEditori, EditorileLST, edit => edit.Denumirea, "editori")) return;
            EditorileLST.Add(new Editor(AdaugaEditoriTextBox.Text));
            AdaugaEditoriTextBox.Text = "";
        }

        private void btnAdaugaDezvoltListBox_Click(object sender, RoutedEventArgs e)
        {
            if (!AddDezvEditListBoxVal.Validare(AdaugaDezvoltatoriTextBox, ErrDezvoltatori, DezvoltatoriLST, dez => dez.Denumirea, "dezvoltator")) return;
            DezvoltatoriLST.Add(new Dezvoltator(AdaugaDezvoltatoriTextBox.Text));
            AdaugaDezvoltatoriTextBox.Text = "";
        }

        private async void btnCautareOnlineJoc_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidatorCautareOnline.ValidareCautare(OnlineDenumireaJocTextBox, ErrRezOnliCaut, JocCurent)) return;

            Joc jocCache = null;
            if (grCache.existaInCache(OnlineDenumireaJocTextBox.Text, ref jocCache))
                JocCurent = jocCache;
            else
            {
                Joc jocDinLista = Jocuri.Find(jc => jc.Denumirea.Equals(OnlineDenumireaJocTextBox.Text, StringComparison.OrdinalIgnoreCase));
                if (jocDinLista != null)
                {
                    JocCurent = jocDinLista;
                    grCache.adaugaInCache(OnlineDenumireaJocTextBox.Text, jocDinLista);
                }
                else
                {
                    JocCurent = await PrinAPI.Cauta(OnlineDenumireaJocTextBox.Text.ToLower());
                    if (!ValidatorCautareOnline.ValidareCautare(OnlineDenumireaJocTextBox, ErrRezOnliCaut, JocCurent, true)) return;
                    grCache.adaugaInCache(OnlineDenumireaJocTextBox.Text, JocCurent);
                }
            }
            OnlineGasitJocDockPanel.Visibility = Visibility.Visible;

            CautareOnlineDenumTextBlock.Text    = "Denumirea: " + JocCurent.Denumirea;
            CautareOnlineDezvTextBlock.Text     = "Dezvoltatori: " + JocCurent.DezvoltatoriStr;
            CautareOnlineEditorTextBlock.Text   = "Editori: " + JocCurent.EditoriStr;
            CautareOnlineGenreTextBlock.Text    = "Genre: " + JocCurent.GenreStr;
            CautareOnlinePlatfTextBlock.Text    = "Platforme: " + JocCurent.Platforme.ToString();
            CautareOnlinePretTextBlock.Text     = "Pret: " + JocCurent.Pret.ToString();
            CautareOnlineIDTextBlock.Text       = "ID: " + JocCurent.ExternalId;
            CautareOnlineRateTextBlock.Text     = "Rate: " + JocCurent.Rate.ToString();
            CautareOnlineVarstaTextBlock.Text   = "Rating Varsta: " + JocCurent.Varsta.ToString();

            if (!string.IsNullOrEmpty(JocCurent.ImgUrl))
            {
                BitmapImage imag = new BitmapImage();
                imag.BeginInit();
                imag.UriSource = new Uri(JocCurent.ImgUrl);
                imag.CacheOption = BitmapCacheOption.OnLoad;
                imag.EndInit();
                OnlineImagineJocImage.Source = imag;
            }
            else OnlineImagineJocImage.Source = null;
        }

        private void btnAdaugareOnlineJoc_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidatorCautareOnline.ValidareAdaugare(JocCurent, Jocuri, ErrRezOnliCaut)) return;

            AdministrJocuri.AddJoc(JocCurent);
            ErrRezOnliCaut.Text = "Joaca a fost adaugata cu succes!";
            ErrRezOnliCaut.Foreground = Brushes.Green;
            ErrRezOnliCaut.Visibility = Visibility.Visible;

            AfisareJocuri();
            ResetOutputCautareOnline();
            JocCurent = null;

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
            AdaugaEditoriTextBox.Text = "";
            AdaugaDezvoltatoriListBox.SelectedItems.Clear();
            AdaugaEditoriListBox.SelectedItems.Clear();
            AdaugaDezvoltatoriTextBox.Text = "";
            SelectatorVarsta.SelectedIndex = 0;
            ReleaseData.SelectedDate = null;
            eDisponibilCheckBox.IsChecked = false;
        }
        private void StergeInputModificare()
        {
            ModifJoacaPretTextBox.Text                 = "";
            ModifJoacaDenumireaTextBox.Text            = "";
            ModifJoacaGenreTextBox.Text                = "";
            ModifJoacaRateTextBox.Text                 = "";
            ModifJoacaReleaseDataListBox.SelectedDate  = null;
            ModifJoacaVarstaListBox.SelectedItem       = null;
            ModifJoacaEsteDispobinilCheckBox.IsChecked = false;
            ModifJoacaPlatformeListBox.SelectedItems.Clear();
            ModifJoacaDezvoltatoriListBox.SelectedItems.Clear();
            ModifJoacaEditoriListBox.SelectedItems.Clear();

        }
        private void ResetOutputCautareOnline()
        {
            OnlineGasitJocDockPanel.Visibility = Visibility.Collapsed;
            OnlineDenumireaJocTextBox.Text      = "";
            CautareOnlineIDTextBlock.Text       = "ID: ";
            CautareOnlineDenumTextBlock.Text    = "Denumirea: ";
            CautareOnlinePretTextBlock.Text     = "Pret: ";
            CautareOnlineRateTextBlock.Text     = "Rate: ";
            CautareOnlineGenreTextBlock.Text    = "Genre: ";
            CautareOnlineDezvTextBlock.Text     = "Dezvoltatori: ";
            CautareOnlineEditorTextBlock.Text   = "Editori: ";
            CautareOnlinePlatfTextBlock.Text    = "Platforme: ";
            CautareOnlineVarstaTextBlock.Text   = "Varsta: ";
            OnlineImagineJocImage.Source        = null;
        }
        private void InputModificare(bool aprins)
        {
            btnModificaXAML.IsEnabled                  = aprins;
            ModifJoacaPretTextBox.IsEnabled            = aprins;
            ModifJoacaDenumireaTextBox.IsEnabled       = aprins;
            ModifJoacaDezvoltatoriListBox.IsEnabled    = aprins;
            ModifJoacaEditoriListBox.IsEnabled         = aprins;
            ModifJoacaGenreTextBox.IsEnabled           = aprins;
            ModifJoacaRateTextBox.IsEnabled            = aprins;
            ModifJoacaReleaseDataListBox.IsEnabled     = aprins;
            ModifJoacaVarstaListBox.IsEnabled          = aprins;
            ModifJoacaPlatformeListBox.IsEnabled       = aprins;
            ModifJoacaEsteDispobinilCheckBox.IsEnabled = aprins;
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
