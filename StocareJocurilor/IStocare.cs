using System;
using DespreJoc;

namespace StocareJocurilor
{
    public interface IStocare
    {
        void AddJoc(Joc joc);
        bool RemoveJoc(string denumirea);
        bool RemoveUltJoc();
        List<Joc> GetJocuri();
        List<Joc> GetJocuriCautare(string categorie, string criteriu);
        Joc GetJoc(string tDenumirea);
        bool UpdateJoc(Joc joc);
    }
}