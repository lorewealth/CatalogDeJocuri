using System;
using System.Collections.Generic;

namespace ProiectCatalogDeJocuri
{
    class Program
    {
        static void Main(string[] args)
        {
            int nrDeGenre = 0;
            int nrDePlatforme = 0;
            int nrDeDezvoltatori = 0;
            int nrDePublicatori = 0;
            string denumirea;
            int id = 0;
            double pret = 0.0;
            double rate = 0.0;
            List<string> Genre = new List<string>();
            List<string> Platforme = new List<string>();
            List<string> Dezvoltatori = new List<string>();
            List<string> Publicatori = new List<string>();

            Console.Write("Denumirea jocului: ");
            denumirea = Console.ReadLine();

            Console.Write("Id jocului: ");
            int.TryParse(Console.ReadLine(), out id);

            Console.Write("Pretul jocului: ");
            double.TryParse(Console.ReadLine(), out pret);

            Console.Write("Rata(Rating) jocului: ");
            double.TryParse(Console.ReadLine(), out rate);

            Console.Write("Cate genre vreti sa adaugati: ");
            int.TryParse(Console.ReadLine(), out nrDeGenre);
            for (int i = 0; i < nrDeGenre; i++)
            {
                Console.Write($"Genrul[{i+1}]: ");
                Genre.Add(Console.ReadLine());
            }

            Console.Write("Pe cate platforme joaca este disponibila: ");
            int.TryParse(Console.ReadLine(), out nrDePlatforme);
            for(int i = 0; i < nrDePlatforme; i++)
            {
                Console.Write($"Platforma[{i+1}]: ");
                Platforme.Add(Console.ReadLine());
            }

            Console.Write("Cate dezvoltatori au dezvoltat joaca: ");
            int.TryParse(Console.ReadLine(), out nrDeDezvoltatori);
            for(int i = 0; i < nrDeDezvoltatori; i++)
            {
                Console.Write($"Dezvoltator[{i+1}]: ");
                Dezvoltatori.Add(Console.ReadLine());
            }
            Console.Write("Cate publicatori au publicat joaca: ");
            int.TryParse(Console.ReadLine(), out nrDePublicatori);
            for(int i = 0; i < nrDePublicatori; i++)
            {
                Console.Write($"Publicator[{i+1}]: ");
                Publicatori.Add(Console.ReadLine());
            }


            Joc Test = new Joc(id, denumirea, pret, Genre, Platforme, Publicatori, Dezvoltatori, rate);

            Test.getInfo();

        }
    }
}