namespace AboutGame.Enums
{
    public enum AgeRatingsCategory : byte
    {
        ESRB = 1,
        PEGI = 2,
        USK = 3,
        CERO = 4,
        GRAC = 5,
        CLASS_IND = 6,
        ACB = 7,
    }
    public enum AgeRatingsValue : ushort
    {
        // ESRB (U.S. & Canada)
        ESRB_E = 1,    // Everyone
        ESRB_E10 = 2,  // Everyone 10+
        ESRB_T = 3,    // Teen
        ESRB_M = 4,    // Mature 17+
        ESRB_AO = 6,   // Adults Only
        ESRB_RP = 7,   // Rating Pending
        ESRB_RP_LM = 8, // Rating Pending but Likely Mature

        // PEGI (Europe)
        PEGI3 = 9,
        PEGI7 = 10,
        PEGI12 = 11,
        PEGI16 = 12,
        PEGI18 = 13,

        // CERO (Japan)
        CERO_A = 14,  // All ages
        CERO_B = 15,  // 12+
        CERO_C = 16,  // 15+
        CERO_D = 17,  // 17+
        CERO_Z = 18,  // 18+ only

        // USK (Germany)
        USK0 = 19,
        USK6 = 20,
        USK12 = 21,
        USK16 = 22,
        USK18 = 23,

        // GRAC (Korea)
        GRAC_All = 24,
        GRAC_12 = 25,
        GRAC_15 = 26,
        GRAC_19 = 27,

        // ClassInd (Brazil)
        CLASS_IND_L = 28,   // Livre (Free/General)
        CLASS_IND_10 = 29,
        CLASS_IND_12 = 30,
        CLASS_IND_14 = 31,
        CLASS_IND_16 = 32,
        CLASS_IND_18 = 33,

        // ACB (Australia)
        ACB_G = 34,
        ACB_PG = 35,
        ACB_M = 36,
        ACB_MA15 = 37,
        ACB_R18 = 38
    } 
}
