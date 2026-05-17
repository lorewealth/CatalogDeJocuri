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

        public List<Joc> GetJocuriCautare(string categoria, string criteriu)
        {
            List<Joc> Jocuri = new List<Joc>();
            List<Joc> joculGasit = new List<Joc>();

            using(StreamReader fisier = new StreamReader(denFisier))
            {
                string linieStr = string.Empty;
                while((linieStr = fisier.ReadLine()) != null)
                {
                    Jocuri.Add(new Joc(linieStr));
                }
            }

            switch (categoria)
            {
                case "denumirea":
                case "genre":
                case "platforme":
                case "dezvoltatori":
                case "editori":
                case "varsta":
                    string[] strArrRest = criteriu.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                    foreach (string str in strArrRest)
                    {
                        if (categoria == "denumirea")       joculGasit = Jocuri.Where(joc => joc.Denumirea.Equals(str, StringComparison.OrdinalIgnoreCase)).ToList();
                        if (categoria == "genre")           joculGasit = Jocuri.Where(joc => joc.Genre.Any(genrul => genrul.Equals(str, StringComparison.OrdinalIgnoreCase))).ToList();
                        if (categoria == "platforme" && Enum.TryParse<PlatformeDisponibile>(str, true, out PlatformeDisponibile res)) 
                                                            joculGasit = Jocuri.Where(joc => joc.Platforme.HasFlag(res)).ToList();
                        if (categoria == "editori")         joculGasit = Jocuri.Where(joc => joc.Editori.Any(editor => editor.Equals(str, StringComparison.OrdinalIgnoreCase))).ToList();
                        if (categoria == "dezvoltatori")    joculGasit = Jocuri.Where(joc => joc.Dezvoltatori.Any(dezvoltator => dezvoltator.Equals(str, StringComparison.OrdinalIgnoreCase))).ToList();
                        if (categoria == "varsta")          joculGasit = Jocuri.Where(joc => joc.Varsta.ToString().Equals(str, StringComparison.OrdinalIgnoreCase)).ToList();
                    }

                    break;
                case "pret":
                case "rate":
                case "releasedata":
                    string[] strArrPRA = criteriu.Split('-', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                    if (categoria == "releasedata")
                    {
                        //---temporar
                        // Concatenez la primul si al doilea element(care sunt anii) luna, ziua si ora,
                        // pentru a putea compara cu data selectata
                        //---
                        DateTime inceputT = Convert.ToDateTime($"{strArrPRA[0]}.01.01");
                        DateTime sfarsitT = Convert.ToDateTime($"{strArrPRA[1]}.01.01");

                        if (inceputT > sfarsitT)
                            (inceputT, sfarsitT) = (sfarsitT, inceputT);

                        joculGasit = Jocuri.Where(joc => joc.ReleaseData >= inceputT && joc.ReleaseData <= sfarsitT).ToList();
                        break;
                    }
                    double inceput = Convert.ToDouble(strArrPRA[0]);
                    double sfarsit = Convert.ToDouble(strArrPRA[1]);

                    if (inceput > sfarsit)
                        (inceput, sfarsit) = (sfarsit, inceput);

                    if (categoria == "pret") joculGasit = Jocuri.Where(joc => joc.Pret >= inceput && joc.Pret <= sfarsit).ToList();
                    if (categoria == "rate") joculGasit = Jocuri.Where(joc => joc.Rate >= inceput && joc.Rate <= sfarsit).ToList();

                    break;
                default:
                    break;
            }
            return joculGasit;

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
