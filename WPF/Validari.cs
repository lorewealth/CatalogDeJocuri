using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using EnumGestionare;

namespace WPF
{
    class Validari
    {

        public static ErrCod ValidareDenumire(string denumirea)
        {
            if (string.IsNullOrWhiteSpace(denumirea)) return ErrCod.ESTE_GOL;
            return ErrCod.OK;
        }
        public static ErrCod ValidarePret(string pret)
        {
            if (string.IsNullOrWhiteSpace(pret)) return ErrCod.ESTE_GOL;
            if (!double.TryParse(pret, out var pretD)) return ErrCod.NU_CONTINE_CIFRE;
            if (pretD < 0) return ErrCod.VAL_NEGATIVA;
            return ErrCod.OK;
        }
        public static ErrCod ValidareRate(string rate) 
        { 
            if (string.IsNullOrWhiteSpace(rate)) return ErrCod.ESTE_GOL;
            if (!double.TryParse(rate, out double rateD)) return ErrCod.NU_CONTINE_CIFRE;
            if (rateD < 1 || rateD > 10) return ErrCod.DUPA_DIAPOZON;
            return ErrCod.OK;
        }
        public static ErrCod ValidareGenre(string genre)
        {
            if (string.IsNullOrWhiteSpace(genre)) return ErrCod.ESTE_GOL;

            string[] genreArr = genre.Split(',');
            HashSet<string> unic = new HashSet<string>();

            if(genreArr.Any(gen => !unic.Add(gen.Trim().ToLower()))) return ErrCod.NU_ESTE_UNIC;

            return ErrCod.OK;

        }
        public static ErrCod ValidarePlatforma(ItemCollection colectiaPlatforme)
        {
            int selectat = 0;
            foreach(var elem in colectiaPlatforme)
            {
                if (elem is CheckBox checkboxul && checkboxul.IsChecked == true) selectat++;
            }
            if (selectat == 0) return ErrCod.NU_ATI_SELECTAT;

            return ErrCod.OK;
        }   
        public static ErrCod ValidareEditori(string editori)
        {
            if (string.IsNullOrWhiteSpace(editori)) return ErrCod.ESTE_GOL;
            return ErrCod.OK;
        }

        public static ErrCod ValidareDezvoltatori(string dezvoltatori)
        {
            if (string.IsNullOrWhiteSpace(dezvoltatori)) return ErrCod.ESTE_GOL;
            return ErrCod.OK;
        }

        public static ErrCod ValidareVarsta(string varsta)
        {
            if (!Enum.IsDefined(typeof(RatingVarsta), varsta)) return ErrCod.NU_EXISTA;

            return ErrCod.OK;
        }
    }
}
