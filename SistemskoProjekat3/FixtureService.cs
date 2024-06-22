using SistemskoProjekat3.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;


namespace SistemskoProjekat3
{
    public class FixtureService
    {
        private readonly HttpClient client = new HttpClient();
        private readonly string apiKey = "ks244zIJciWnf6zn2G9wO7wMrv98NXd02Kq3lWqwwzdVQkhHjsWx1F6Ig8bv";

        public async Task<IEnumerable<Fixtures>> FetchFixtureAsync(int matchId)
        {
            var url = $"https://api.sportmonks.com/v2.0/fixtures/18535517?api_token={apiKey}&include=lineups.player;lineups.player.country";
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var jsonResponse = JObject.Parse(content)["data"];

            return jsonResponse?.Select(fixture => new Fixtures
            {
                Id = (int)fixture["id"]!,
                Name = (string)fixture["name"]!,
                StartingAt = (DateTime)fixture["starting_at"]!,
                ResultInfo = (string)fixture["result_info"]!,
                PlayerList = ((JArray)fixture["lineup"]!).Select(player => new Player
                {
                    FirstName = (string)player["player"]!["data"]!["firstname"]!,
                    LastName = (string)player["player"]!["data"]!["lastname"]!,
                    BirthYear = DateTime.Parse((string)player["player"]!["data"]!["birthdate"]!).Year,
                    ShirtNumber = (int)player["number"]!,
                    Nationality = (string)player["player"]!["data"]!["nationality"]!
                }).ToList()
            }) ?? Enumerable.Empty<Fixtures>();
        }
    }
}
