using System;
using DespreJoc;
using DespreJoc.Enums;

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
                    string[] dateArr = date.Split(Joc.SEPARATOR_PRINCIPAL_FISIER);
                    if (den.Equals(dateArr[Joc.DENUMIREA], StringComparison.OrdinalIgnoreCase))
                        return new Joc(date);
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
                    if (!updateSucces && jocTXT.InternalId == joculActualizat.InternalId)
                    {
                        fisier.WriteLine(joculActualizat.FormatareJoculuiInStr());
                        updateSucces = true;
                    }

                    else fisier.WriteLine(jocTXT.FormatareJoculuiInStr());
                }
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
            List<Joc> Jocuri = GetJocuri();
            joc.setInternalId(GetNextIdJoc());
            Jocuri.Add(joc);

            using(StreamWriter fisier = new StreamWriter(denFisier, false))
                foreach(Joc j in Jocuri)
                    fisier.WriteLine(j.FormatareJoculuiInStr());

        }

        public bool RemoveJoc(string denumirea)
        {
            List<Joc> Jocuri = GetJocuri();
            bool sters = false;
            if (Jocuri.Count == 0) return false;
            int id = 1;

            using (StreamWriter fisier = new StreamWriter(denFisier, false))
            {
                foreach (Joc j in Jocuri)
                {
                    if (!sters && j.Denumirea.Equals(denumirea, StringComparison.OrdinalIgnoreCase))
                    {
                        sters = true;
                        continue;
                    }
                    j.setInternalId(id++);
                    fisier.WriteLine(j.FormatareJoculuiInStr());
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
