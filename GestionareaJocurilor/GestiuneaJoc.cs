using System;
using DespreJoc;
using EnumGestionare;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GestionareaJocurilor
{
    public class GestiuneaJoc
    {
        public List<Joc> Jocuri { get; private set; }
        public int CurrentId { get; private set; } = 1;
        public Joc JocNou { get; private set; }

        public GestiuneaJoc()
        {
            Jocuri = new List<Joc>();
        }
        public void IncrementInternalId()
        {
            CurrentId++;
        }
        public void AddJoc(Joc joc)
        {
            joc.setId(CurrentId++);
            Jocuri.Add(joc);
        }

        public void ResetJocNou()
        {
            JocNou = null;
        }

        public void SetJocNou(Joc joc)
        {
            JocNou = joc;
        }

        public Joc GetJoc(string tDenumirea)
        {
            return Jocuri.Find(jocul => jocul.Denumirea.Equals(tDenumirea, StringComparison.OrdinalIgnoreCase));
        }
        public List<Joc> GetJocuri(string categoria, string criteriul)
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
                    JocuriGasiti = Jocuri.Where(joc => joc.VarstaNecesara.ToString().Equals(criteriul, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;
                default: break;
            }
            return JocuriGasiti;
        }
    }
}
