using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DespreJoc.Enums
{
    [Flags]
    public enum PlatformeDisponibile
    {
        Steam = 1 << 0,
        EpicGamesStore = 1 << 1,
        GOG = 1 << 2,
        MicrosoftStore = 1 << 3,
        UbisoftStore = 1 << 4,
        Blizzard = 1 << 5,
        EAStore = 1 << 6,
    }
}
