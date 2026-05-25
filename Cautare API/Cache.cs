using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DespreJoc;

namespace Cautare_API
{
    internal struct CacheJoc
    {
        public Joc JoculGasit { get; set; }
        public DateTime DataDeCache { get; set; }
    }
}
