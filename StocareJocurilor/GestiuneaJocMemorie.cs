using DespreJoc;
using EnumGestionare;

namespace StocareJocurilor
{
    public class GestiuneaJocMemorie : IStocare
    {
        public List<Joc> Jocuri { get; private set; }

        public GestiuneaJocMemorie()
        {
            Jocuri = new List<Joc>();
        }

        public int GetNextIdJoc()
        {
            if (Jocuri.Count == 0) return 1;
            return Jocuri.Last().InternalId + 1;
        }

        public void AddJoc(Joc joc)
        {
            joc.setInternalId(GetNextIdJoc());
            Jocuri.Add(joc);
        }

        public Joc GetJoc(string tDenumirea)
        {
            return Jocuri.Find(jocul => jocul.Denumirea.Equals(tDenumirea, StringComparison.OrdinalIgnoreCase));
        }

        public bool UpdateJoc(Joc jocActualizat)
        {

            int index = Jocuri.IndexOf(Jocuri.Find(joc => joc.InternalId == jocActualizat.InternalId));
            if (index == -1) return false;

            Jocuri[index] = jocActualizat;

            return true;
        }

        public List<Joc> GetJocuri()
        {
            return Jocuri;
        }

        public List<Joc> GetJocuriCautare(string categoria, string criteriul)
        {
            List<Joc> JocuriGasiti = new List<Joc>();

            switch (categoria) 
            {
                case "GENRE":
                    JocuriGasiti = Jocuri.Where(joc => joc.Genre.Any(subgen => subgen.Equals(criteriul, StringComparison.OrdinalIgnoreCase))).ToList();
                    break;
                case "DEZVOLTATORI":
                    JocuriGasiti = Jocuri.Where(joc => joc.Dezvoltatori.Any(subdez => subdez.Equals(criteriul, StringComparison.OrdinalIgnoreCase))).ToList();
                    break;
                case "EDITORI":
                    JocuriGasiti = Jocuri.Where(joc => joc.Editori.Any(subpub => subpub.Equals(criteriul, StringComparison.OrdinalIgnoreCase))).ToList();
                    break;
                case "PLATFORME":
                    if (Enum.TryParse<PlatformeDisponibile>(criteriul, true, out PlatformeDisponibile res))
                    {
                        JocuriGasiti = Jocuri.Where(joc => joc.Platforme.HasFlag(res)).ToList();
                    }
                    break;
                case "VARSTA":
                    JocuriGasiti = Jocuri.Where(joc => joc.Varsta.ToString().Equals(criteriul, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;
                default: break;
            }
            return JocuriGasiti;
        }
    }
}
