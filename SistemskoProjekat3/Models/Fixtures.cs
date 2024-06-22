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
    }
}
