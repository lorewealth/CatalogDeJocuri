namespace AboutGame.Enums
{
    [Flags]
    public enum AvailableStores : ushort
    {
        None = 0,
        Steam = 1 << 0,
        EpicGamesStore = 1 << 1,
        GOG = 1 << 2,
        MicrosoftStore = 1 << 3,
        UbisoftStore = 1 << 4,
        Blizzard = 1 << 5,
        EAStore = 1 << 6,
        All = Steam | EpicGamesStore | GOG | MicrosoftStore | UbisoftStore | Blizzard | EAStore
    }
}
