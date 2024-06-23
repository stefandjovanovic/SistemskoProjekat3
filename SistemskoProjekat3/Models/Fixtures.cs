using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemskoProjekat3.Modules
{
    public class Fixtures
    {
        public int Id { get; set; }
        public string ?Name { get; set; }
        public DateTime StartingAt { get; set; }
        public string ?ResultInfo { get; set; }
        public List<Player> ? LocalTeamPlayers { get; set; }
        public List<Player>? VisitorTeamPlayers { get; set; }



        public override string ToString()
        {
            var result = $"Fixture ID: {Id}\nTeams: {Name}\nStarting At: {StartingAt}\n";

            if (!string.IsNullOrEmpty(ResultInfo))
            {
                result += $"Result: {ResultInfo}\n";
            }

            

            if (LocalTeamPlayers != null && VisitorTeamPlayers != null)
            {
                result += "Local team players: \n\n";

                foreach (var player in LocalTeamPlayers)
                {
                    result += $"{player.FirstName} {player.LastName}, " +
                        $"Shirt number: {player.ShirtNumber}, " +
                        $"Date of birth: {player.DateOfBirth.ToShortDateString()}, " +
                        $"Country: {player.Country}\n";
                }

                result += "\nVisitor team players: \n\n";

                foreach (var player in VisitorTeamPlayers)
                {
                    result += $"{player.FirstName} {player.LastName}, " +
                        $"Shirt number: {player.ShirtNumber}, " +
                        $"Date of birth: {player.DateOfBirth.ToShortDateString()}, " +
                        $"Country: {player.Country}\n";
                }
            }

            return result;
        }

    }
}
