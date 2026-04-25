namespace EnumGestionare
{
    public enum Categorii
    {
        Genre = 1,
        Dezvoltatori = 2,
        Editori = 3,
        Platforme = 4,
        Varsta = 5
    }
    [Flags]
    public enum PlatformeDisponibile
    {
        Steam = 1 << 0,
        Epic = 1 << 1,
        GOG = 1 << 2,
        itchIo = 1 << 3
    }

    public enum RatingVarsta
    {
        PEGI3 = 1,
        PEGI7 = 2,
        PEGI12 = 3,
        PEGI16 = 4,
        PEGI18 = 5
    }
}
