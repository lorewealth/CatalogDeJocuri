using System;
using System.Globalization;
using System.IO;
using DespreJoc.Enums;

namespace DespreJoc
{
    public class Joc
    {
        //variabile
        public int InternalId { get; private set; }
        public string ExternalId { get; private set; }
        public string Denumirea { get; set; }
        public double Pret { get; set; }
        public string PretStr
        {
            get => Pret.ToString(); 
            set
            {
                double.TryParse(value.Replace(',', '.'), CultureInfo.InvariantCulture, out double pret);
                Pret = pret;
            }
        }
        public double Rate { get; set; }
        public string RateStr
        { 
            get => Rate.ToString();
            set
            {
                if (!double.TryParse(value.Replace(',', '.'), CultureInfo.InvariantCulture, out double rate)) Rate = 1;
                else Rate = rate;
            }
        }
        public List<string> Genre { get; set; }
        public string GenreStr
        {
            get => string.Join(", ", Genre);
            set
            {
                List<string> genreStr = [];
                foreach(string gen in value.Split(", "))
                {
                    genreStr.Add(gen);
                }
                Genre = genreStr;
            }
        }
        public PlatformeDisponibile Platforme { get; set; }
        public List<Editor> Editori { get; set; } = [];
        public string EditoriStr
        {
            get => string.Join(", ", Editori.Select(ed => ed.Denumirea));
            set
            {
                List<Editor> editori = [];
                foreach (string edi in value.Split(", "))
                    editori.Add(new Editor(edi));

                Editori = editori;
            }
        }
        public List<Dezvoltator> Dezvoltatori { get; set; } = [];
        public string DezvoltatoriStr
        {
            get => string.Join(", ", Dezvoltatori.Select(dv => dv.Denumirea));
            set
            {
                List<Dezvoltator> dezvol = [];
                foreach (string dezv in value.Split(", "))
                    dezvol.Add(new Dezvoltator(dezv));

                Dezvoltatori = dezvol;
            }
        }
        public RatingVarsta Varsta { get; set; }
        public DateTime ReleaseData { get; set; }
        public bool EsteDisponibil { get; set; }
        public string ImgUrl { get; set; }

        //constante
        public const char SEPARATOR_PRINCIPAL_FISIER = ';';
        private const char SEPARATOR_SECUNDAR_FISIER = '|';
        private const int INTERNAL_ID = 0;
        private const int EXTERNAL_ID = 1;
        public const int DENUMIREA = 2;
        private const int PRETUL = 3;
        private const int RATING = 4;
        private const int GENRE = 5;
        private const int PLATFORME = 6;
        private const int DEZVOLTATORI = 7;
        private const int EDITORI = 8;
        private const int VARSTA = 9;
        private const int RELEASE_DATA = 10;
        private const int ESTE_DISPONIBIL = 11;
        private const int IMG_URL = 12;

        public const double RATE_MIN = 1;
        public const double RATE_MAX = 10;
        public const int RELEASE_DATA_ANUL_MIN = 1958;
        public const int RELEASE_DATA_ANUL_MAX = 2050;

        //metode
        public void setInternalId(int id) { InternalId = id; }

        //constructor
        public Joc(string Denumirea, double Pret, List<string> Genre, PlatformeDisponibile Platforme, List<Editor> Editori, List<Dezvoltator> Dezvoltatori, double Rate, RatingVarsta RatingVarsta, DateTime ReleaseData, bool EsteDisponibil = true, string ExternalId = "null", string ImgUrl = "null")
        {
            this.Denumirea = Denumirea;
            this.Pret = Pret;
            this.Genre = Genre;
            this.Platforme = Platforme;
            this.Rate = Rate;
            this.Editori = Editori;
            this.Dezvoltatori = Dezvoltatori;
            this.Varsta = RatingVarsta;
            this.ReleaseData = ReleaseData;
            this.EsteDisponibil = EsteDisponibil;
            this.ExternalId = ExternalId;
            this.ImgUrl = ImgUrl;
        }

        public Joc(string linieStr)
        {
            List<string> date = linieStr.Split(SEPARATOR_PRINCIPAL_FISIER).ToList();

            this.InternalId = Convert.ToInt32(date[INTERNAL_ID]);
            this.ExternalId = date[EXTERNAL_ID].Trim();
            this.Denumirea = date[DENUMIREA].Trim();
            this.Pret = Convert.ToDouble(date[PRETUL]);
            this.Rate = Convert.ToDouble(date[RATING]);
            this.Genre = date[GENRE].Split(SEPARATOR_SECUNDAR_FISIER).ToList();
            this.ImgUrl = date[IMG_URL].Trim();

            foreach(string dezv in date[DEZVOLTATORI].Split(SEPARATOR_SECUNDAR_FISIER))
                this.Dezvoltatori.Add(new Dezvoltator(dezv));

            foreach(string edit in date[EDITORI].Split(SEPARATOR_SECUNDAR_FISIER))
                this.Editori.Add(new Editor(edit));

            foreach (string platforma in date[PLATFORME].Split(SEPARATOR_SECUNDAR_FISIER))
                if (Enum.TryParse(platforma, true, out PlatformeDisponibile Platforma)) { this.Platforme |= Platforma; }

            if (Enum.TryParse(date[VARSTA], true, out RatingVarsta Varsta)) { this.Varsta = Varsta; }

            this.ReleaseData = Convert.ToDateTime(date[RELEASE_DATA]);
            this.EsteDisponibil = Convert.ToBoolean(date[ESTE_DISPONIBIL]);
        }

        public string FormatareJoculuiInStr()
        {
            string sGenre = string.Join(SEPARATOR_SECUNDAR_FISIER, Genre); 
            string sDezvoltatori = string.Join(SEPARATOR_SECUNDAR_FISIER, Dezvoltatori.Select(dezv => dezv.Denumirea)); 
            string sEditori = string.Join(SEPARATOR_SECUNDAR_FISIER, Editori.Select(edit => edit.Denumirea));
            List<string> platformeFormatate = Platforme.ToString().Split(", ").ToList();

            string sPlatforme = string.Join(SEPARATOR_SECUNDAR_FISIER, platformeFormatate); 
            
            string formatFinal = string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}{0}{9}{0}{10}{0}{11}{0}{12}{0}{13}", SEPARATOR_PRINCIPAL_FISIER,
                                                InternalId.ToString(),//{1}
                                                ExternalId.ToString(),
                                                Denumirea ?? "NECUNOSCUT",
                                                Pret.ToString() ?? "0.0",
                                                Rate.ToString() ?? "0.0",
                                                sGenre ?? "NECUNOSCUT",
                                                sPlatforme ?? "NECUNOSCUT",
                                                sDezvoltatori ?? "NECUNOSCUT",
                                                sEditori ?? "NECUNOSCUT",
                                                Varsta.ToString() ?? "PEGI3",
                                                ReleaseData.ToString("yyyy.MM.dd") ?? "NECUNOSCUT",
                                                EsteDisponibil,
                                                ImgUrl);//{13}
            return formatFinal;
        }

        public string GetInfo()
        {
            string info = $"Detalii jocului: {Denumirea}\nPretul: {Pret}\nRating: {Rate}/10";
            info += "\nGenre:\n\t" + string.Join("\n\t", Genre);
            info += "\nPlatforme:\n\t" + Platforme.ToString().Replace(", ", "\n\t");
            info += "\nDezvoltatori:\n\t" + string.Join("\n\t", Dezvoltatori.Select(dezv => dezv.Denumirea));
            info += "\nEditori:\n\t" + string.Join("\n\t", Editori.Select(edit => edit.Denumirea));
            info += "\nRating de Varsta:\n\t" + Varsta.ToString();
            info += "\nReleaseDate:\n\t" + ReleaseData;
            info += "\nEsteDisponibil:\n\t" + EsteDisponibil;
                
            return info;
        }
    }
}