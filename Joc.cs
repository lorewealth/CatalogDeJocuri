using System;
using System.Collections.Generic;

namespace ProiectCatalogDeJocuri
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
            if (Rate >= 0.0 && Rate <= 10.0) 
            {
                this.Rate = Rate;
            }
            else
            {
                Console.WriteLine("Rate este setat incorect!");
                Rate = 0.0;
            }
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
            if(Publicatori.Count > 0)
            {
                foreach(var el in Publicatori)
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
            if(Dezvoltatori.Count > 0)
            {
                foreach(var el in Dezvoltatori)
                {
                    Console.WriteLine($"    {el}");
                }
            }
            else
            {
                Console.WriteLine("Nu a fost initializat dezvolatorii!");
            }
        }

        public void getInfo()
        {
            Console.WriteLine($"Detalii jocului: {Denumirea}");
            Console.WriteLine($"Pretul: {Pret}");
            Console.WriteLine($"Rating: {Rate}/10");
            Console.WriteLine("Genre: ");
            getGenre();
            Console.WriteLine("Platforme: ");
            getPlatforme();
            Console.WriteLine("Publicatori: ");
            getPublicatori();
            Console.WriteLine("Dezvoltatori: ");
            getDezvoltatori();
        }

    }
}