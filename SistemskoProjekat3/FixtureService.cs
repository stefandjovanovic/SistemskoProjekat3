using SistemskoProjekat3.Modules;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace SistemskoProjekat3
{
    public class FixtureService
    {
        private readonly HttpClient client = new HttpClient();
        private readonly string apiKey = "ks244zIJciWnf6zn2G9wO7wMrv98NXd02Kq3lWqwwzdVQkhHjsWx1F6Ig8bv";

        public IObservable<Fixtures> FetchFixtureAsync(int matchId)
        {
            var url = $"https://api.sportmonks.com/v2.0/fixtures/{matchId}?api_token={apiKey}&include=lineups.player;lineups.player.country";

            return Observable.FromAsync(() => client.GetStringAsync(url))
                             .SubscribeOn(CurrentThreadScheduler.Instance)
                             .Select(content =>
                             {
                                 var jsonResponse = JObject.Parse(content)?["data"];
                                 if (jsonResponse == null)
                                 {
                                     throw new Exception("Nema podataka o utakmici.");
                                 }

                                 return jsonResponse.Select(fixture => new Fixtures
                                 {
                                     Id = (int)fixture["id"]!,
                                     Name = (string)fixture["name"]!,
                                     StartingAt = (DateTime)fixture["starting_at"]!,
                                     ResultInfo = (string)fixture["result_info"]!,
                                     PlayerList = ((JArray)fixture["lineups"]!).Select(player => new Player
                                     {
                                         FirstName = (string)player["player"]!["firstname"]!,
                                         LastName = (string)player["player"]!["lastname"]!,
                                         DateOfBirth = (DateTime)player["player"]!["date_of_birth"]!,
                                         ShirtNumber = (int)player["jersey_number"]!,
                                         Country = (string)player["player"]!["country"]!["name"]!
                                     }).ToList()
                                 }).First();
                             })
                             .ObserveOn(CurrentThreadScheduler.Instance); 
        }
    }
}
