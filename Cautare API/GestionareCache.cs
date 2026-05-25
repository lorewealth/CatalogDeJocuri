using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DespreJoc;

namespace Cautare_API
{
    public class GestionareCache
    {
        private Dictionary<string, CacheJoc> cache = [];
        private readonly TimeSpan expiratie = TimeSpan.FromMinutes(30);
        public bool existaInCache(string denumirea, ref Joc obiect)
        {
            if (cache.TryGetValue(denumirea, out CacheJoc obiectCachat))
            {
                if(DateTime.Now - obiectCachat.DataDeCache < expiratie)
                {
                    obiect = obiectCachat.JoculGasit;
                    return true;
                }
                else cache.Remove(denumirea);
            }
            obiect = null;
            return false;
        }
        public void adaugaInCache(string denumirea, Joc obiect)
        {
            cache[denumirea] = new CacheJoc { JoculGasit = obiect, DataDeCache = DateTime.Now };
        }
    }
}
