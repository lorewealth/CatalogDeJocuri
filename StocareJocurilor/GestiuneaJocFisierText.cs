using System;
using DespreJoc;
using EnumGestionare;

namespace StocareJocurilor
{
    public class GestiuneaJocFisierText : IStocare
    {
        private string denFisier = string.Empty;

        public GestiuneaJocFisierText(string tDenFisier) 
        {
            this.denFisier = tDenFisier;

            Stream streamFisierText = File.Open(denFisier, FileMode.OpenOrCreate);
            streamFisierText.Close();
        }

        public List<Joc> GetJocuri()
        {
            List<Joc> Jocuri = new List<Joc>();

            using(StreamReader fisier = new StreamReader(denFisier))
            {
                string linieStr = string.Empty;
                while ((linieStr = fisier.ReadLine()) != null)
                {
                    Jocuri.Add(new Joc(linieStr));
                }
            }

            return Jocuri;
        }

        public List<Joc> GetJocuriCautare(string categoria, string criteriul)
        {
            List<Joc> JocuriCititeFisier = new List<Joc>();
            List<Joc> JocuriGasiti = new List<Joc>();

            using(StreamReader fisier = new StreamReader(denFisier))
            {
                string linieStr = string.Empty;
                while((linieStr = fisier.ReadLine()) != null)
                {
                    JocuriCititeFisier.Add(new Joc(linieStr));
                }
            }

            switch (categoria)
            {
                case "GENRE":
                    JocuriGasiti = JocuriCititeFisier.Where(joc => joc.Genre.Any(subgen => subgen.Equals(criteriul, StringComparison.OrdinalIgnoreCase))).ToList();
                    break;
                case "DEZVOLTATORI":
                    JocuriGasiti = JocuriCititeFisier.Where(joc => joc.Dezvoltatori.Any(subdez => subdez.Equals(criteriul, StringComparison.OrdinalIgnoreCase))).ToList();
                    break;
                case "EDITORI":
                    JocuriGasiti = JocuriCititeFisier.Where(joc => joc.Editori.Any(subpub => subpub.Equals(criteriul, StringComparison.OrdinalIgnoreCase))).ToList();
                    break;
                case "PLATFORME":
                    if (Enum.TryParse<PlatformeDisponibile>(criteriul, true, out PlatformeDisponibile res))
                    {
                        JocuriGasiti = JocuriCititeFisier.Where(joc => joc.Platforme.HasFlag(res)).ToList();
                    }
                    break;
                case "VARSTA":
                    JocuriGasiti = JocuriCititeFisier.Where(joc => joc.Varsta.ToString().Equals(criteriul, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;
                default: break;
            }
            return JocuriGasiti;

        }

        public Joc GetJoc(string den)
        {
            using(StreamReader fisier = new StreamReader(denFisier))
            {
                string date = string.Empty;
                while((date = fisier.ReadLine()) != null)
                {
                    Joc jocul = new Joc(date);
                    if(jocul.Denumirea.Equals(den, StringComparison.OrdinalIgnoreCase))
                    {
                        return jocul;
                    }
                }
            }
            return null;
        }

        public bool UpdateJoc(Joc joculActualizat)
        {
            List<Joc> JocuriTXT = GetJocuri();
            bool updateSucces = false;

            using(StreamWriter fisier = new StreamWriter(denFisier, false))
            {
                foreach (Joc jocTXT in JocuriTXT)
                {
                    Joc jocul = jocTXT;
                    if (jocTXT.InternalId == joculActualizat.InternalId)
                    {
                        jocul = joculActualizat;
                    }
                    fisier.WriteLine(jocul.FormatareJoculuiInStr());
                }
                updateSucces = true;
            }
            return updateSucces;
        }

        public int GetNextIdJoc()
        {
            List<Joc> Jocuri = GetJocuri();

            if (Jocuri.Count == 0) return 1;

            return Jocuri.Last().InternalId + 1;            
        }

        public void AddJoc(Joc joc)
        {
            joc.setInternalId(GetNextIdJoc());

            using(StreamWriter fisier = new StreamWriter(denFisier, true))
            {
                fisier.WriteLine(joc.FormatareJoculuiInStr());
            }
        }

        public bool RemoveJoc(string denumirea)
        {
            int id = 1;
            bool sters = false;
            List<Joc> Jocuri = GetJocuri();

            if (Jocuri.Count == 0) return false;
            
            using (StreamWriter fisier = new StreamWriter(denFisier, false))
            {
                foreach (Joc j in Jocuri)
                {
                    Joc jocT = j;
                    if (jocT.Denumirea.Equals(denumirea, StringComparison.OrdinalIgnoreCase))
                    {
                        sters = true;
                        continue;
                    }
                    jocT.setInternalId(id++);
                    fisier.WriteLine(jocT.FormatareJoculuiInStr());
                }
            }
            return sters;
        }

        public bool RemoveUltJoc()
        {
            List<Joc> Jocuri = GetJocuri();

            if (Jocuri.Count == 0) return false;

            using (StreamWriter fisier = new StreamWriter(denFisier, false))
            {
                for(int i = 0; i < Jocuri.Count-1; i++)
                {
                    fisier.WriteLine(Jocuri[i].FormatareJoculuiInStr());
                }
                return true;
            }
        }
    }
}
