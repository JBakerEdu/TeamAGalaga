using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galaga.Model
{
    [Serializable]
    public class HighScoreEntry
    {
        public string PlayerName { get; set; }
        public int Score { get; set; }
        public int Level { get; set; }
    }

}
