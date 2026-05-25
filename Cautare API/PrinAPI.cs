using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DespreJoc;
using DespreJoc.Enums;

namespace Cautare_API
{
    public class PrinAPI
    {
        private const string Cheie = "13878b9660475725d2ae6f918bef7a49ff190129";
        static HttpClient Client = new HttpClient();

        public static async Task<Joc> Cauta(string denumireCautarii)
        {
            //cautam intai joaca
            string url = $"https://api.isthereanydeal.com/games/search/v1?key={Cheie}&title={Uri.EscapeDataString(denumireCautarii)}";
            string raspuns = await Client.GetStringAsync(url);
            JsonDocument json = JsonDocument.Parse(raspuns);

            if (json.RootElement.GetArrayLength() == 0) return null;

            //gasit? preluam id-ul
            string idExtern = json.RootElement[0].GetProperty("id").GetString();

            //scoatem informatia detailiata despre joaca
            url = $"https://api.isthereanydeal.com/games/info/v2?key={Cheie}&id={idExtern}";
            raspuns = await Client.GetStringAsync(url);
            json = JsonDocument.Parse(raspuns);

            //titlu
            string titlu = json.RootElement.GetProperty("title").GetString();

            //preiau url a imaginii jocului
            string imgUrl = string.Empty;
            if (json.RootElement.GetProperty("assets").TryGetProperty("boxart", out JsonElement boxart))
                imgUrl = boxart.GetString();
            else if (json.RootElement.GetProperty("assets").TryGetProperty("banner600", out JsonElement banner))
               imgUrl = banner.GetString();

            //adaug genre
            List<string> genre = [];
            JsonElement items = json.RootElement.GetProperty("tags");
            foreach (var item in items.EnumerateArray())
                genre.Add(item.GetString());

            if (genre.Count == 0) genre.Add("Necunoscut");

            //varsta
            // temporar are logica aceasta
            RatingVarsta varsta = (json.RootElement.GetProperty("mature").GetBoolean()) ? RatingVarsta.PEGI18 : RatingVarsta.PEGI16;

            //data de release
            DateTime releaseDate = Convert.ToDateTime(json.RootElement.GetProperty("releaseDate").GetString());

            //este disponibil
            bool esteDisponibil = (releaseDate <= DateTime.Now);

            //dezvoltatori
            List<Dezvoltator> dezvoltatori = [];
            items = json.RootElement.GetProperty("developers");
            foreach (var item in items.EnumerateArray())
                dezvoltatori.Add(new Dezvoltator(item.GetProperty("name").GetString()));

            //editori
            List<Editor> editori = [];
            items = json.RootElement.GetProperty("publishers");
            foreach (var item in items.EnumerateArray())
                editori.Add(new Editor(item.GetProperty("name").GetString()));

            //rate
            double rate = 0;
            if (esteDisponibil)
                rate = Convert.ToDouble(json.RootElement.GetProperty("reviews")[0].GetProperty("score").GetDouble()) / 10;

            //pentru pret si platforme se utilizeaza alt link
            string corp = JsonSerializer.Serialize(new[] { idExtern.ToString() });
            StringContent content = new StringContent(corp, Encoding.UTF8, "application/json");

            url = $"https://api.isthereanydeal.com/games/prices/v3?key={Cheie}&country=RO";
            var raspunsPOST = await Client.PostAsync(url, content);
            raspuns = await raspunsPOST.Content.ReadAsStringAsync();
            json = JsonDocument.Parse(raspuns);

            double pret = 0;
            if (esteDisponibil)
            {
                var dealsArray = json.RootElement[0].GetProperty("deals").EnumerateArray();
                var steamDeal = dealsArray.FirstOrDefault(deal =>
                    deal.GetProperty("shop").GetProperty("name").GetString().Equals("Steam", StringComparison.OrdinalIgnoreCase));

                if (steamDeal.ValueKind == JsonValueKind.Undefined)
                    steamDeal = json.RootElement[0].GetProperty("deals").EnumerateArray().FirstOrDefault();

                if (steamDeal.ValueKind != JsonValueKind.Undefined) 
                    pret = Convert.ToDouble(steamDeal.GetProperty("price").GetProperty("amount").GetDouble());
            }
            
            PlatformeDisponibile platforme = 0;
            foreach (var el in json.RootElement[0].GetProperty("deals").EnumerateArray())
            {
                string numePlatforma = el.GetProperty("shop").GetProperty("name").GetString().Replace(" ", "");
                if (numePlatforma.Equals("EpicGameStore", StringComparison.OrdinalIgnoreCase)) numePlatforma = "EpicGamesStore";
                if (Enum.TryParse(typeof(PlatformeDisponibile), numePlatforma, out object pltf))
                    platforme |= (PlatformeDisponibile)pltf;
            }
            return new Joc(titlu, pret, genre, platforme, editori, dezvoltatori, rate, varsta, releaseDate, esteDisponibil, idExtern, imgUrl);
        }
    }
}