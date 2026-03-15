using System;

namespace DespreJoc
{
    public class Joc
    {
        //variabile
        public int Id { get; private set; }
        public string Denumirea { get; private set; }
        public double Pret { get; private set; }
        public double Rate { get; private set; }
        public List<string> Genre = new List<string>();
        public List<string> Platforme = new List<string>();
        public List<string> Publicatori = new List<string>();
        public List<string> Dezvoltatori = new List<string>();

        //constructor
        public Joc(int Id, string Denumirea, double Pret, List<string> Genre, List<string> Platforme, List<string> Publicatori, List<string> Dezvoltatori, double Rate)
        {
            this.Id = Id;
            this.Denumirea = Denumirea;
            this.Pret = Pret;
            this.Genre = Genre;
            this.Platforme = Platforme;
            this.Rate = Rate;
            this.Publicatori = Publicatori;
            this.Dezvoltatori = Dezvoltatori;
        }

        public void getGenre()
        {
            if (Genre.Count > 0)
            {
                foreach (var el in Genre)
                {
                    Console.WriteLine($"    {el}");
                }
            }
        }

        public void getPlatforme()
        {
            if (Platforme.Count > 0)
            {
                foreach (var el in Platforme)
                {
                    Console.WriteLine($"    {el}");
                }
            }
        }

        public void getPublicatori()
        {
            if (Publicatori.Count > 0)
            {
                foreach (var el in Publicatori)
                {
                    Console.WriteLine($"    {el}");
                }
            }
            else
            {
                Console.WriteLine("Nu a fost initializat publicatorii!");
            }
        }

        public void getDezvoltatori()
        {
            if (Dezvoltatori.Count > 0)
            {
                foreach (var el in Dezvoltatori)
                {
                    Console.WriteLine($"    {el}");
                }
            }
            else
            {
                Console.WriteLine("Nu a fost initializat dezvolatorii!");
            }
        }

        public string getInfo()
        {
            string info = $"Detalii jocului: {Denumirea}\nPretul: {Pret}\nRating: {Rate}/10\nGenre: \n";

            if (Genre.Count != 0 && Platforme.Count != 0 && Publicatori.Count != 0 && Dezvoltatori.Count != 0)
            {
                foreach (string el in Genre)
                {
                    info += "    " + el + "\n";
                }

                info += "Platforme:\n";
                foreach(string el in Platforme)
                {
                    info += "    " + el + "\n";
                }

                info += "Publicatori:\n";
                foreach(string el in Publicatori)
                {
                    info += "    " + el + "\n";
                }

                info += "Dezvoltatori:\n";
                foreach(string el in Dezvoltatori)
                {
                    info += "    " + el + "\n";
                }
            }
            else
            {
                info = "Nu ati introdus nici o joaca";
            }
            
            return info;
        }

    }
}
