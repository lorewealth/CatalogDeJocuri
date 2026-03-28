using System;
using EnumGestionare;
using System.IO;

namespace DespreJoc
{
    public class Joc
    {
        //variabile
        public int InternalId { get; private set; }
        //public int ExternalId { get; private set; }
        public string Denumirea { get; private set; }
        public double Pret { get; private set; }
        public double Rate { get; private set; }
        public List<string> Genre { get; private set; }
        public PlatformeDisponibile Platforme { get; private set; }
        public List<string> Editori { get; private set; }
        public List<string> Dezvoltatori { get; private set; }
        public RatingVarsta Varsta { get; private set; }

        private const char SEPARATOR_PRINCIPAL_FISIER = ';';
        private const char SEPARATOR_SECUNDAR_FISIER = '|';
        private const int INTERNAL_ID = 0;
        private const int DENUMIREA = 1;
        private const int PRETUL = 2;
        private const int RATING = 3;
        private const int GENRE = 4;
        private const int PLATFORME = 5;
        private const int DEZVOLTATORI = 6;
        private const int EDITORI = 7;
        private const int VARSTA = 8;

        public void setInternalId(int id) { InternalId = id; }

        //constructor
        public Joc(string Denumirea, double Pret, List<string> Genre, PlatformeDisponibile Platforme, List<string> Editori, List<string> Dezvoltatori, double Rate, RatingVarsta RatingVarsta)
        {
            this.Denumirea = Denumirea;
            this.Pret = Pret;
            this.Genre = Genre;
            this.Platforme = Platforme;
            this.Rate = Rate;
            this.Editori = Editori;
            this.Dezvoltatori = Dezvoltatori;
            this.Varsta = RatingVarsta;
        }

        public Joc(string linieStr)
        {
            List<string> date = linieStr.Split(SEPARATOR_PRINCIPAL_FISIER).ToList();

            this.InternalId = Convert.ToInt32(date[INTERNAL_ID]);
            this.Denumirea = date[DENUMIREA].Trim();
            this.Pret = Convert.ToDouble(date[PRETUL]);
            this.Rate = Convert.ToDouble(date[RATING]);
            this.Genre = date[GENRE].Split(SEPARATOR_SECUNDAR_FISIER).ToList();
            this.Dezvoltatori = date[DEZVOLTATORI].Split(SEPARATOR_SECUNDAR_FISIER).ToList();
            this.Editori = date[EDITORI].Split(SEPARATOR_SECUNDAR_FISIER).ToList();

            foreach (string platforma in date[PLATFORME].Split(SEPARATOR_SECUNDAR_FISIER))
            {
                if (Enum.TryParse(platforma, true, out PlatformeDisponibile Platforma)) { this.Platforme |= Platforma; }
            }
            if (Enum.TryParse(date[VARSTA], true, out RatingVarsta Varsta)) { this.Varsta = Varsta; }
        }

        public string FormatareJoculuiInStr()
        {
            string sGenre = string.Join(SEPARATOR_SECUNDAR_FISIER, Genre); 
            string sDezvoltatori = string.Join(SEPARATOR_SECUNDAR_FISIER, Dezvoltatori); 
            string sEditori = string.Join(SEPARATOR_SECUNDAR_FISIER, Editori);
            List<string> platformeFormatate = Platforme.ToString().Split(", ").ToList();            

            string sPlatforme = string.Join(SEPARATOR_SECUNDAR_FISIER, platformeFormatate); 
            
            string formatFinal = string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}{0}{9}", SEPARATOR_PRINCIPAL_FISIER,
                                                InternalId.ToString(),//{1}
                                                Denumirea ?? "NECUNOSCUT",
                                                Pret.ToString() ?? "0.0",
                                                Rate.ToString() ?? "0.0",
                                                sGenre ?? "NECUNOSCUT",
                                                sPlatforme ?? "NECUNOSCUT",
                                                sDezvoltatori ?? "NECUNOSCUT",
                                                sEditori ?? "NECUNOSCUT",
                                                Varsta.ToString() ?? "PEGI3");//{9}
            return formatFinal;
        }

        public string GetInfo()
        {
            string info = $"Detalii jocului: {Denumirea}\nPretul: {Pret}\nRating: {Rate}/10";
                info += "\nGenre:\n\t" + string.Join("\n\t", Genre);
                info += "\nPlatforme:\n\t" + Platforme.ToString().Replace(", ", "\n\t");
                info += "\nDezvoltatori:\n\t" + string.Join("\n\t", Dezvoltatori);
                info += "\nEditori:\n\t" + string.Join("\n\t", Editori);
                info += "\nRating de Varsta:\n\t" + Varsta.ToString();
            return info;
        }
    }
}