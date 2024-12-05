using System;

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
