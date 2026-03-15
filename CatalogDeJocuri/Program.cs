using System;
using DespreJoc;
using GestionareaJocurilor;

namespace ProiectCatalogDeJocuri
{
    class Program
    {
        static void Main(string[] args)
        {
            string optiunea;
            GestiuneaJoc Catalog1 = new GestiuneaJoc();

            do
            {
                Console.WriteLine("----------------MENU----------------");
                Console.WriteLine("C. Citirea jocului");
                Console.WriteLine("A. Afisarea jocului");
                Console.WriteLine("S. Salvarea jocului");
                Console.WriteLine("L. Afisarea listei jocurilor");
                Console.WriteLine("F. Cautarea jocului");
                Console.WriteLine("M. Cautarea jocurilor dupa un criteriu");
                Console.WriteLine("X. Iesirea din aplicatia");
                Console.WriteLine("------------------------------------");

                Console.Write("Selectati: ");
                optiunea = Console.ReadLine().ToUpper();

                switch (optiunea)
                {
                    case "C":
                        Catalog1.JocNou = Citirea();
                        break;
                    case "A":
                        if(Catalog1.JocNou == null)
                        {
                            Console.WriteLine("Nu ati introdus nici o joaca");
                            break;
                        }
                        Console.WriteLine(Catalog1.JocNou.getInfo());
                        break;
                    case "S":
                        if (Catalog1.JocNou == null)
                        {
                            Console.WriteLine("Nu ati introdus un joc nou");
                            break;
                        }

                        Catalog1.Jocuri.Add(Catalog1.JocNou);
                        Console.WriteLine("Joaca a fost salvat cu succes");

                        Catalog1.JocNou = null;
                        break;
                    case "L":
                        if(Catalog1.Jocuri.Count == 0)
                        {
                            Console.WriteLine("Nu ati introdus nici un joc");
                            break;
                        }

                        foreach(Joc jocul in Catalog1.Jocuri)
                        {
                            Console.WriteLine(jocul.Denumirea);
                        }

                        break;
                    case "F":
                        Joc tJoc = Catalog1.GetJoc();
                        if(tJoc == null)
                        {
                            Console.WriteLine("Nu a fost gasit acest joc");
                            break;
                        }
                        Console.WriteLine("Joaca a fost gasita: ");
                        Console.WriteLine(tJoc.getInfo());
                        break;
                    case "M":
                        List<Joc> tJocuriGasite = Catalog1.GetJocuri();
                        if(tJocuriGasite.Count == 0)
                        {
                            Console.WriteLine("Nu a fost gasit nici o joaca dupa acest criteriul");
                            break;
                        }
                        Console.WriteLine("Au fost gasite aceste jocuri: ");
                        foreach (Joc elem in tJocuriGasite)
                        {
                            Console.WriteLine(elem.Denumirea);
                        }
                        break;
                    case "X":
                        Console.WriteLine("Iesirea din aplicatie...");
                        break;
                    default:
                        Console.WriteLine("Nu ati selectat corect!");
                        break;
                }
            }
            while (optiunea != "X");
        }


        static Joc Citirea()
        {
            int id = 0;
            string denumirea;
            double pret = 0.0;
            double rate = 0.0;
            List<string> genre = new List<string>();
            List<string> platforme = new List<string>();
            List<string> publicatori = new List<string>();
            List<string> dezvoltatori = new List<string>(); 
            int nrDeGenre = 0;
            int nrDePlatforme = 0;
            int nrDeDezvoltatori = 0;
            int nrDePublicatori = 0;

            Console.Write("Denumirea jocului: ");
            denumirea = Console.ReadLine();

            bool corectId = false;
            do
            {
                Console.Write("Id jocului: ");
                corectId = int.TryParse(Console.ReadLine(), out id);
            }
            while (id < 0 || corectId == false);

            bool corectPret = false;
            do
            {
                Console.Write("Pretul jocului: ");
                corectPret = double.TryParse(Console.ReadLine(), out pret);
            }
            while (pret < 0 || corectPret == false);

            bool corectRate = false;
            do
            {
                Console.Write("Rata(Rating) jocului de la 1-10: ");
                corectRate = double.TryParse(Console.ReadLine(), out rate);
            }
            while (rate < 1 || rate > 10 || corectRate == false);

            do
            {
                Console.Write("Cate genre vreti sa adaugati: ");
                int.TryParse(Console.ReadLine(), out nrDeGenre);
            }
            while (nrDeGenre <= 0);

            for (int i = 0; i < nrDeGenre; i++)
            {
                Console.Write($"Genrul[{i + 1}]: ");
                genre.Add(Console.ReadLine());
            }

            do
            {
                Console.Write("Pe cate platforme joaca este disponibila: ");
                int.TryParse(Console.ReadLine(), out nrDePlatforme);
            }
            while (nrDePlatforme <= 0);

            for (int i = 0; i < nrDePlatforme; i++)
            {
                Console.Write($"Platforma[{i + 1}]: ");
                platforme.Add(Console.ReadLine());
            }

            do
            {
                Console.Write("Cate dezvoltatori au dezvoltat joaca: ");
                int.TryParse(Console.ReadLine(), out nrDeDezvoltatori);
            }
            while (nrDeDezvoltatori <= 0);

            for (int i = 0; i < nrDeDezvoltatori; i++)
            {
                Console.Write($"Dezvoltator[{i + 1}]: ");
                dezvoltatori.Add(Console.ReadLine());
            }

            do
            {
                Console.Write("Cate publicatori au publicat joaca: ");
                int.TryParse(Console.ReadLine(), out nrDePublicatori);
            }
            while (nrDePublicatori <= 0);

            for (int i = 0; i < nrDePublicatori; i++)
            {
                Console.Write($"Publicator[{i + 1}]: ");
                publicatori.Add(Console.ReadLine());
            }

            return new Joc(id, denumirea, pret, genre, platforme, publicatori, dezvoltatori, rate);
        }
    }
}