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
        public RatingVarsta RatingVarsta { get; private set; }

        //constructor
        public Joc(int InternalId, string Denumirea, double Pret, List<string> Genre, PlatformeDisponibile Platforme, List<string> Editori, List<string> Dezvoltatori, double Rate, RatingVarsta RatingVarsta)
        {
            this.InternalId = InternalId;
            this.Denumirea = Denumirea;
            this.Pret = Pret;
            this.Genre = Genre;
            this.Platforme = Platforme;
            this.Rate = Rate;
            this.Editori = Editori;
            this.Dezvoltatori = Dezvoltatori;
            this.RatingVarsta = RatingVarsta;
        }

        public string GetGenre()
        {
            return string.Join(" ", Genre);
        }

        public string GetPlatforme()
        {
            return Platforme.ToString();
        }

        public string GetEditori()
        {
            return string.Join(" ", Editori);
        }

        public string GetDezvoltatori()
        {            
            return string.Join(" ", Dezvoltatori);
        }

        public string GetRatingVarsta()
        {
            return RatingVarsta.ToString();
        }

        public string GetInfo()
        {
            string info = $"Detalii jocului: {Denumirea}\nPretul: {Pret}\nRating: {Rate}/10";
                info += "\nGenre:\n\t" + string.Join("\n\t", Genre);
                info += "\nPlatforme:\n\t" + Platforme.ToString().Replace(", ", "\n\t");
                info += "\nDezvoltatori:\n\t" + string.Join("\n\t", Dezvoltatori);
                info += "\nPublicatori:\n\t" + string.Join("\n\t", Editori);
                info += "\nRating de Varsta:\n\t" + RatingVarsta.ToString();
            return info;
        }
    }
}