using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AboutGame.Enums
{
    [Flags]
    public enum SupportedOS : ushort
    {
        None = 0,
        Windows = 1 << 0,
        MacOS = 1 << 1,
        Linux = 1 << 2,
        SteamDeck = 1 << 3,
        All = Windows | MacOS | Linux | SteamDeck,
    }
}
