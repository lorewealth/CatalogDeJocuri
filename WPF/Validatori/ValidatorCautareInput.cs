using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DespreJoc;

namespace WPF.Validatori
{
    class Cautare
    {
        public static bool CautareInputValidator(string categoria, string criteriu)
        {
            if (string.IsNullOrWhiteSpace(categoria) || string.IsNullOrWhiteSpace(criteriu)) return false;

            string[] strArrPRA = criteriu.Split('-', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            string[] strArrRest = criteriu.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            switch (categoria)
            {
                case "pret":
                case "rate":
                case "releasedata":
                    if(strArrPRA.Length <= 1 || strArrPRA.Length > 2) return false;
                    foreach (string item in strArrPRA) 
                    {  
                        if(!double.TryParse(item, out double val)) return false;

                        if (categoria == "pret" && val < 0) return false;
                        if (categoria == "rate" && (val < Joc.RATE_MIN || val > Joc.RATE_MAX)) return false;
                        if (categoria == "releasedata" && ((int)val < Joc.RELEASE_DATA_ANUL_MIN || (int)val > Joc.RELEASE_DATA_ANUL_MAX)) return false;
                    }
                    return true;
                default: //pentru restul: dezvoltatori, editori, denumire
                    if (strArrRest.Length < 1) return false;
                    return !Regex.IsMatch(strArrRest[0], @"^\W");
            }
        }
    }
}
