using System;

namespace Galaga.Model
{
    /// <summary>
    /// This where the high score entries are set
    /// </summary>
    [Serializable]
    public class HighScoreEntry
    {
        /// <summary>
        /// the Players name that is input by the user
        /// </summary>
        public string PlayerName { get; set; }
        /// <summary>
        /// the score the user gets by playing the game
        /// </summary>
        public int Score { get; set; }
        /// <summary>
        /// the level the player gets to in the game
        /// </summary>
        public int Level { get; set; }
    }

}
