using System;
using DespreJoc;
using StocareJocurilor;
using EnumGestionare;
using CatalogDeJocuri;

namespace ProiectCatalogDeJocuri
{
    class Program
    {
        static void Main(string[] args)
        {
            string optiunea;
            IStocare Catalog = Decident.PrelucrareaDatelor();

            Joc JocNou = null;
            List<Joc> Jocuri = new List<Joc>();

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
                try
                {
                    switch (optiunea)
                    {
                        case "C":
                            JocNou = Citirea();
                            break;
                        case "A":
                            if (JocNou == null)
                            {
                                throw new Exception("Nu ati introdus nici o joaca");
                            }
                            Console.WriteLine(JocNou.GetInfo());
                            break;
                        case "S":
                            if (JocNou == null)
                            {
                                throw new Exception("Nu ati introdus un joc nou");
                            }

                            Catalog.AddJoc(JocNou);
                            Console.WriteLine("Joaca a fost salvat cu succes");

                            JocNou = null;
                            break;
                        case "L":
                            Jocuri = Catalog.GetJocuri();
                            if (Jocuri.Count == 0)
                            {
                                throw new Exception("Nu ati introdus nici un joc");
                            }

                            foreach (Joc jocul in Jocuri)
                            {
                                Console.WriteLine($"[{jocul.InternalId}] - {jocul.Denumirea}");
                            }

                            break;
                        case "F":
                            Console.Write("Denumirea jocului cautat: ");
                            Joc tJoc = Catalog.GetJoc(Console.ReadLine().Trim().ToUpper());

                            if (tJoc == null)
                            {
                                throw new Exception("Joaca nu a fost gasita");
                            }

                            Console.WriteLine("Joсul a fost gasit: ");
                            Console.WriteLine(tJoc.GetInfo());
                            break;
                        case "M":
                            var categoriiDisponibile = Enum.GetValues<Categorii>();
                            int categoriaSelectata = 0;

                            Console.WriteLine("Sunt disponbile aceste categorii: ");
                            foreach (var Categoria in categoriiDisponibile)
                            {
                                Console.WriteLine($"{(int)Categoria} - [{Categoria}]");
                            }

                            do
                            {
                                Console.Write("Selectati categoria de la 1-5: ");
                                int.TryParse(Console.ReadLine(), out categoriaSelectata);
                            }
                            while (categoriaSelectata <= 0 || categoriaSelectata > 5);
                            
                            string categoriaStr = ((Categorii)categoriaSelectata).ToString().ToUpper();

                            Console.Write($"In {categoriaStr} dupa ce criteriu sa caute: ");
                            string criteriul = Console.ReadLine().Trim().ToUpper();
                            
                            List<Joc> tJocuriGasite = Catalog.GetJocuriCautare(categoriaStr, criteriul);

                            if (tJocuriGasite.Count == 0)
                            {
                                throw new Exception($"Nu a fost gasit joaca in {categoriaStr} cu criteriul {criteriul}");
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
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
        }
            while (optiunea != "X");
        }


        static Joc Citirea()
        {
            string denumirea = string.Empty;
            double pret = 0.0;
            double rate = 0.0;
            List<string> genre = new List<string>();
            PlatformeDisponibile platforme = 0;
            List<string> editori = new List<string>();
            List<string> dezvoltatori = new List<string>(); 
            int nrDeGenre = 0;
            int nrDePlatforme = 0;
            int nrDeDezvoltatori = 0;
            int nrDeEditori = 0;
            RatingVarsta varsta = 0;

            do
            {
                Console.Write("Denumirea jocului: ");
                denumirea = Console.ReadLine();
            } 
            while (denumirea == string.Empty) ;

            bool corectPret = false;
            do
            {
                Console.Write("Pretul jocului: ");
                corectPret = double.TryParse(Console.ReadLine(), out pret);
            }
            while (pret < 0 || corectPret == false);

            do
            {
                Console.Write("Rata(Rating) jocului de la 1-10: ");
                double.TryParse(Console.ReadLine(), out rate);
            }
            while (rate < 1 || rate > 10);

            do
            {
                Console.Write("Cate genre vreti sa adaugati: ");
                int.TryParse(Console.ReadLine(), out nrDeGenre);
            }
            while (nrDeGenre <= 0);

            for (int i = 0; i < nrDeGenre; i++)
            {
                string str = string.Empty;
                do
                {
                    Console.Write($"Genrul[{i + 1}]: ");
                    str = Console.ReadLine();
                }
                while (genre.Contains(str));

                genre.Add(str);
            }

            var PlatformeDisponibileList = Enum.GetValues<PlatformeDisponibile>();
            Console.WriteLine("Sunt disponibile aceste platforme: ");

            int contor = 1;
            foreach (var elem in PlatformeDisponibileList)
            {
                Console.WriteLine($"{contor++} - {elem}");
            }

            do
            {
                Console.Write("Pe cate platforme joaca este disponibila: ");
                int.TryParse(Console.ReadLine(), out nrDePlatforme);
            }
            while (nrDePlatforme <= 0 || nrDePlatforme > 4);

            Console.WriteLine("Selectati de la 1 la 4: ");
            for (int i = 0; i < nrDePlatforme; i++)
            {
                int platformaSelectata = 0;
                do
                {
                    Console.Write($"Platforma[{i + 1}]: ");
                    int.TryParse(Console.ReadLine(), out platformaSelectata);
                }
                while (platformaSelectata <= 0 || platformaSelectata > 4);

                PlatformeDisponibile Platforma = (PlatformeDisponibile) (1 << (platformaSelectata - 1));
                platforme |= Platforma;
            }

            do
            {
                Console.Write("Cate dezvoltatori au dezvoltat joaca: ");
                int.TryParse(Console.ReadLine(), out nrDeDezvoltatori);
            }
            while (nrDeDezvoltatori <= 0);

            for (int i = 0; i < nrDeDezvoltatori; i++)
            {
                string dezvoltator = string.Empty;
                do
                {
                    Console.Write($"Dezvoltator[{i + 1}]: ");
                    dezvoltator = Console.ReadLine();
                }
                while (dezvoltator == string.Empty || dezvoltatori.Contains(dezvoltator));

                dezvoltatori.Add(dezvoltator);
            }
            do
            {
                Console.Write("Cati editori au publicat joaca: ");
                int.TryParse(Console.ReadLine(), out nrDeEditori);
            }
            while (nrDeEditori <= 0);

            for (int i = 0; i < nrDeEditori; i++)
            {
                string editor = string.Empty;
                do
                {
                    Console.Write($"Editor[{i + 1}]: ");
                    editor = Console.ReadLine();
                }
                while (editor == string.Empty || editori.Contains(editor));
                
                editori.Add(editor);
            }

            int varstaSelectata = 0;
            var RatingVarstaList = Enum.GetValues<RatingVarsta>();
            Console.WriteLine("Sunt disponibile aceste rating-uri de varsta");
            foreach(RatingVarsta rating in RatingVarstaList)
            {
                Console.WriteLine($"{(int)rating} - [{rating}]");
            }

            do
            {
                Console.Write("Ce Rating de varsta are joaca: ");
                int.TryParse(Console.ReadLine(), out varstaSelectata);
            }
            while (!Enum.IsDefined(typeof(RatingVarsta), varstaSelectata));

            varsta = (RatingVarsta)varstaSelectata;

            return new Joc(denumirea, pret, genre, platforme, editori, dezvoltatori, rate, varsta);
        }
    }
}