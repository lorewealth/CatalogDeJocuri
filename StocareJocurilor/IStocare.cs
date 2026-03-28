using System;
using DespreJoc;

namespace StocareJocurilor
{
    public interface IStocare
    {
        void AddJoc(Joc joc);
        List<Joc> GetJocuri();
        List<Joc> GetJocuriCautare(string categorie, string criteriu);
        Joc GetJoc(string tDenumirea);
        bool UpdateJoc(Joc joc);
    }
}
