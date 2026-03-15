using DespreJoc;

namespace GestionareaJocurilor
{
    public class GestiuneaJoc
    {
        public List<Joc> Jocuri;
        public Joc JocNou = null;

        public GestiuneaJoc()
        {
            Jocuri = new List<Joc>();
        }

        public Joc GetJoc()
        {
            Console.Write("Denumirea jocului cautat: ");
            string tDenumirea = Console.ReadLine().ToUpper();
            return Jocuri.Find(jocul => jocul.Denumirea.ToUpper() == tDenumirea);
        }
        public List<Joc> GetJocuri()
        {
            Console.Write("Sunt disponbile aceste criterii: [Genre], [Dezvoltatori], [Publicatori], [Platforme]\nDupa ce criteriu vreti sa cautati jocul: ");
            string criteriu = Console.ReadLine().ToUpper();
            List<Joc> JocuriGasiti = new List<Joc>();
            switch (criteriu) 
            {
                case "GENRE":
                    Console.Write("Dupa care genru vreti sa gasiti: ");
                    string genrul = Console.ReadLine().ToUpper();

                    foreach (Joc elem in Jocuri)
                    {
                        foreach (string genr in elem.Genre)
                        {
                            if(genr.ToUpper() == genrul)
                            {
                                JocuriGasiti.Add(elem);
                            }
                        }
                    }
                    break;
                case "DEZVOLTATORI":
                    Console.Write("Dupa care dezvoltator vreti sa gasiti: ");
                    string dezvoltator = Console.ReadLine().ToUpper();

                    foreach (Joc elem in Jocuri)
                    {
                        foreach (string dezv in elem.Dezvoltatori)
                        {
                            if (dezv.ToUpper() == dezvoltator)
                            {
                                JocuriGasiti.Add(elem);
                            }
                        }
                    }
                    break;
                case "PUBLICATORI":
                    Console.Write("Dupa care publicator vreti sa gasiti: ");
                    string publicator = Console.ReadLine().ToUpper();

                    foreach (Joc elem in Jocuri)
                    {
                        foreach (string publ in elem.Publicatori)
                        {
                            if (publ.ToUpper() == publicator)
                            {
                                JocuriGasiti.Add(elem);
                            }
                        }
                    }
                    break;
                case "PLATFORME":
                    Console.Write("Dupa care platforma vreti sa gasiti: ");
                    string platforma = Console.ReadLine().ToUpper();

                    foreach (Joc elem in Jocuri)
                    {
                        foreach (string platf in elem.Platforme)
                        {
                            if (platf.ToUpper() == platforma)
                            {
                                JocuriGasiti.Add(elem);
                            }
                        }
                    }
                    break;
                default:
                    Console.WriteLine("Nu ati ales optiunea corecta!");
                    break;
            }
            return JocuriGasiti;
        }

        
    }
}
