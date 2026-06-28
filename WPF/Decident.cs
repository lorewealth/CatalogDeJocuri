using System;
using StocareJocurilor;
using System.Configuration;
using System.IO;

namespace CatalogDeJocuri
{
    public static class Decident
    {
        private const string FORMAT = "FormatSalvare";
        private const string NUME_FISIER = "NumeFisier";

        public static IStocare PrelucrareaDatelor()
        {
            string formatSalvare = ConfigurationManager.AppSettings[FORMAT] ?? "";//se va prelua valoarea a cheielui FORMAT
            string numeFisier = ConfigurationManager.AppSettings[NUME_FISIER] ?? "";//se ia valoarea de la cheie NUME_FISIER in App.config

            string locatieSolutie = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.Parent?.FullName ?? "";
            string locatieCompleta = locatieSolutie + "//" + numeFisier;

            if (formatSalvare != null)
            {
                switch (formatSalvare)
                {
                    case "memorie":
                        return new GestiuneaJocMemorie();
                    case "txt":
                        return new GestiuneaJocFisierText(locatieCompleta + '.' + formatSalvare);
                }
            }
            return null;
        }

    }
}
