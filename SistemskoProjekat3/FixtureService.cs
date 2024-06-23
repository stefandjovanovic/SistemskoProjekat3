using SistemskoProjekat3.Modules;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Net;

namespace SistemskoProjekat3
{
    public class FixtureService
    {
        private readonly HttpClient client = new HttpClient();
        private readonly string apiKey = "ks244zIJciWnf6zn2G9wO7wMrv98NXd02Kq3lWqwwzdVQkhHjsWx1F6Ig8bv";


        public IObservable<Fixtures> GetFixturesObservable(HttpListenerRequest request)
        {
            string url = request.RawUrl ?? "";
            Console.WriteLine("Primljen je zahtev: " + url);

            string id = url.Split('/')[1];

            return FetchFixtureAsync(id);
        }


        public IObservable<Fixtures> FetchFixtureAsync(string matchId)
        {
            var url = $"https://api.sportmonks.com/v3/football/fixtures/{matchId}?api_token={apiKey}&include=lineups.player;lineups.player.country";

            return Observable.FromAsync(() => client.GetStringAsync(url))
                             .SubscribeOn(CurrentThreadScheduler.Instance)
                             .Select(response =>
                             {
                                 Console.WriteLine("Primljen odgovor sa API-a");
                                 return JObject.Parse(response);
                             })
                             .Select(data => data["data"] ?? throw new Exception("Ne postoji utakmica sa datim ID"))
                             .Select(fixture => 
                             {
                                 var allPlayers = ((JArray)fixture["lineups"]!).Select(player => new Player
                                 {
                                     FirstName = (string)player["player"]!["firstname"]!,
                                     LastName = (string)player["player"]!["lastname"]!,
                                     DateOfBirth = DateTime.Parse((string)player["player"]!["date_of_birth"]!),
                                     ShirtNumber = (int)player["jersey_number"]!,
                                     Country = (string)player["player"]!["country"]!["name"]!,
                                     TeamId = (int)player["team_id"]!
                                 }).ToList();

                                 var groupedPlayers = allPlayers.GroupBy(p => p.TeamId).ToList();

                                 var localTeam = groupedPlayers.First().ToList();
                                 var visitorTeam = groupedPlayers.Skip(1).First().ToList();


                                 return new Fixtures
                                 {
                                     Id = (int)fixture["id"]!,
                                     Name = (string)fixture["name"]!,
                                     StartingAt = DateTime.Parse((string)fixture["starting_at"]!),
                                     ResultInfo = (string)fixture["result_info"]!,
                                     LocalTeamPlayers = localTeam,
                                     VisitorTeamPlayers = visitorTeam
                                 };

                             }); ;
        }
    }
}
