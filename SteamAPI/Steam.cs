using System;
using System.Text.Json;
using DespreJoc;
using EnumGestionare;


namespace SteamAPI
{
    public class SteamAPIs
    {
        public Joc JocCautat { get; private set; }

        public SteamAPIs()
        {
            JocCautat = new Joc();
        }
        public void ResetJocCautat()
        {
            JocCautat = new Joc();
        }

        public async Task CautareaJoculuiSteamAPI(string denumirea)
        {
            string url = $"https://store.steampowered.com/api/storesearch/?term={denumirea}&l=english&cc=RO";
            using HttpClient client = new HttpClient();

            var raspuns = await client.GetAsync(url);
            if (!raspuns.IsSuccessStatusCode) return;

            var JSONRaspuns = await raspuns.Content.ReadAsStringAsync();
            using var DocRaspuns = JsonDocument.Parse(JSONRaspuns);
            var obiecte = DocRaspuns.RootElement.GetProperty("items");

            if (obiecte.GetArrayLength() == 0) return;

            var primulRes = obiecte[0];
            string externalId = primulRes.GetProperty("id").ToString();
            string gasit = primulRes.GetProperty("name").GetString();

            string descUrl = $"https://store.steampowered.com/api/appdetails?appids={externalId}";
            var descriptiunea = await client.GetAsync(descUrl);

            if (!descriptiunea.IsSuccessStatusCode) return;

            var descJSON = await descriptiunea.Content.ReadAsStringAsync();
            using var descDoc = JsonDocument.Parse(descJSON);

            var joaca = descDoc.RootElement.GetProperty(externalId);
            if (joaca.GetProperty("success").GetBoolean())
            {
                var data = joaca.GetProperty("data");

                string denum = data.GetProperty("name").ToString().Replace("™", "");
                var dezvoltatorJSON = data.GetProperty("developers");
                var editoriJSON = data.GetProperty("publishers");
                var genreJSON = data.GetProperty("genres");

                List<string> dezvoltatori = new List<string>();
                foreach (var dezvoltator in dezvoltatorJSON.EnumerateArray())
                {
                    dezvoltatori.Add(dezvoltator.GetString());
                }

                List<string> editori = new List<string>();
                foreach (var editor in editoriJSON.EnumerateArray())
                {
                    editori.Add(editor.GetString());
                }

                List<string> genre = new List<string>();
                foreach (var genr in genreJSON.EnumerateArray())
                {
                    string descGenr = genr.GetProperty("description").GetString();
                    genre.Add(descGenr);
                }

                bool faraPret = data.GetProperty("is_free").GetBoolean();
                double pret;
                if (faraPret)
                {
                    pret = 0.0;
                }
                else if (data.TryGetProperty("price_overview", out var pretFrm))
                {
                    int pretCuZecimale = pretFrm.GetProperty("final").GetInt32();
                    pret = pretCuZecimale / 100.0;
                }
                else
                {
                    pret = -1;//in caz daca joaca e anuntata dar pret nu, il setez la -1
                }

                double rata = 0.0;
                if (data.TryGetProperty("metacritic", out var rateJSON))
                {
                    rata = rateJSON.GetProperty("score").GetInt32();
                    rata /= 10;
                }

                RatingVarsta PEGI = 0;

                if (data.TryGetProperty("required_age", out var VarstaNecesara))
                {

                    int varsta = Convert.ToInt32(VarstaNecesara.ToString());

                    if (varsta == 3) { PEGI = (RatingVarsta)3; }
                    else if (varsta == 6) { PEGI = (RatingVarsta)2; }
                    else if(varsta >= 17) { PEGI = (RatingVarsta)5; }
                    else { PEGI = (RatingVarsta)0; }
                    
                }

                JocCautat.setDenumirea(denum);
                JocCautat.setSteamId(externalId);
                JocCautat.setPret(pret);
                JocCautat.setGenre(genre);
                JocCautat.setPlatforme(PlatformeDisponibile.Steam);
                JocCautat.setEditori(editori);
                JocCautat.setDezvoltatori(dezvoltatori);
                JocCautat.setRate(rata);
                JocCautat.setVarstaNecesara(PEGI);

            }
        }
    };
}
