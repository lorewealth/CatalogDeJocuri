using System;
using EnumGestionare;
using System.IO;

namespace DespreJoc
{
    public class Joc
    {
        //variabile
        public int InternalId { get; private set; }
        public string SteamId { get; private set; } = "---";
        public string Denumirea { get; private set; }
        public double Pret { get; private set; }
        public double Rate { get; private set; }
        public List<string> Genre { get; private set; }
        public PlatformeDisponibile Platforme { get; private set; }
        public List<string> Editori { get; private set; }
        public List<string> Dezvoltatori { get; private set; }
        public RatingVarsta VarstaNecesara { get; private set; }
        private const string VALUTA = "[EURO]"; 

        //constructor
        public Joc(string Denumirea, double Pret, List<string> Genre, PlatformeDisponibile Platforme, List<string> Editori, List<string> Dezvoltatori, double Rate, RatingVarsta VarstaNecesara)
        {
            this.Denumirea = Denumirea;
            this.Pret = Pret;
            this.Genre = Genre;
            this.Platforme = Platforme;
            this.Rate = Rate;
            this.Editori = Editori;
            this.Dezvoltatori = Dezvoltatori;
            this.VarstaNecesara = VarstaNecesara;
        }

        public Joc()
        {
            this.Denumirea = "NECUNOSCUT";
            this.Pret = 0.00;
            this.Genre = new List<string>();
            this.Platforme = PlatformeDisponibile.None;
            this.Rate = 0.0;
            this.Editori = new List<string>();
            this.Dezvoltatori = new List<string>();
            this.VarstaNecesara = RatingVarsta.PEGI0;
        }

        public void setId(int nr) { this.InternalId = nr; }
        public void setSteamId(string SteamId) { this.SteamId = SteamId; }
        public void setDenumirea(string Den) { this.Denumirea = Den; }
        public void setPret(double Pret) { this.Pret = Pret; }
        public void setRate(double Rate) { this.Rate = Rate; }
        public void setGenre(List<string> Genre) { this.Genre = Genre; }
        public void setPlatforme(PlatformeDisponibile platforma) { this.Platforme = platforma; }
        public void setEditori(List<string> Editori) { this.Editori = Editori; }
        public void setDezvoltatori(List<string> Dezvoltatori) { this.Dezvoltatori = Dezvoltatori; }
        public void setVarstaNecesara(RatingVarsta VarstaNecesara) { this.VarstaNecesara = VarstaNecesara; }

        public string GetInfo()
        {
            string info = $"Detalii jocului: {Denumirea}\nPretul: {Pret}{VALUTA}\nRating: {Rate}/10";
                   info += "\nSteamID: " + SteamId;
                   info += "\nGenre:\n\t" + string.Join("\n\t", Genre);
                   info += "\nPlatforme:\n\t" + Platforme.ToString().Replace(", ", "\n\t");
                   info += "\nDezvoltatori:\n\t" + string.Join("\n\t", Dezvoltatori);
                   info += "\nPublicatori:\n\t" + string.Join("\n\t", Editori);
                   info += "\nRating de Varsta:\n\t" + VarstaNecesara.ToString();

            return info;
        }
    }
}