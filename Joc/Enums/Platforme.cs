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
        Epic = 1 << 1,
        GOG = 1 << 2,
        itchIo = 1 << 3
    }
}
