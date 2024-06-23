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
        public List<Player> ?PlayerList { get; set; }

        public override string ToString()
        {
            var result = $"Fixture ID: {Id}\nTeams: {Name}\nStarting At: {StartingAt}\n";

            if (!string.IsNullOrEmpty(ResultInfo))
            {
                result += $"Result: {ResultInfo}\n";
            }

            result += "Players: \n";

            if (PlayerList != null)
            {
                foreach (var player in PlayerList)
                {
                    result += $"{player.FirstName} {player.LastName}, " +
                        $"Shirt number: {player.ShirtNumber}, " +
                        $"Date of birth: {player.DateOfBirth.ToShortDateString()}, " +
                        $"Countr: {player.Country}\n";
                }
            }

            return result;
        }

    }
}
