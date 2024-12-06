using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Windows.Storage;

namespace Galaga.Model
{
    /// <summary>
    /// manages the high scores of the game
    /// </summary>
    public class HighScoreManager
    {
        private const string FileName = "HighScores.xml";
        private readonly StorageFolder storageFolder = ApplicationData.Current.LocalFolder;

        /// <summary>
        /// the collection of high scores
        /// </summary>
        public ObservableCollection<HighScoreEntry> HighScores { get; set; }

        /// <summary>
        /// sets the high scores from the loaded scores
        /// </summary>
        public HighScoreManager()
        {
            this.HighScores = this.loadHighScores();
        }

        private ObservableCollection<HighScoreEntry> loadHighScores()
        {
            try
            {
                var filePath = Path.Combine(this.storageFolder.Path, FileName);
                Debug.Write(filePath);
                if (File.Exists(filePath))
                {
                    using (var stream = File.OpenRead(filePath))
                    {
                        var serializer = new XmlSerializer(typeof(ObservableCollection<HighScoreEntry>));
                        return (ObservableCollection<HighScoreEntry>)serializer.Deserialize(stream);
                    }
                }
            }
            catch
            {
                throw new System.Exception("Failed to load high scores");
            }
            return new ObservableCollection<HighScoreEntry>();
        }

        /// <summary>
        /// saves the high scores to a file
        /// </summary>
        /// <exception cref="System.Exception">if it fails to save high scores</exception>
        public void SaveHighScores()
        {
            try
            {
                var filePath = Path.Combine(this.storageFolder.Path, FileName);
                Debug.Write(filePath);
                using (var stream = File.Create(filePath))
                {
                    var serializer = new XmlSerializer(typeof(ObservableCollection<HighScoreEntry>));
                    serializer.Serialize(stream, this.HighScores);
                }
            }
            catch
            {
                throw new System.Exception("Failed to save high scores");
            }
        }

        /// <summary>
        /// Adds a new score entry to the high scores list, sorts the list by score (descending), 
        /// player name (ascending), and level (descending), and ensures the list contains only the top 10 entries. 
        /// The updated high scores are then saved persistently.
        /// </summary>
        /// <param name="playerName">The name of the player to associate with the score.</param>
        /// <param name="score">The score to add to the high scores list.</param>
        public void AddScore(string playerName, int score)
        {
            this.HighScores.Add(new HighScoreEntry { PlayerName = playerName, Score = score });
            this.HighScores = new ObservableCollection<HighScoreEntry>(this.HighScores
                .OrderByDescending(h => h.Score)
                .ThenBy(h => h.PlayerName)
                .ThenByDescending(h => h.Level)
                .Take(10));
            this.SaveHighScores();
        }

        /// <summary>
        /// clears the file and reset the high scores
        /// </summary>
        public void ResetHighScores()
        {
            this.HighScores.Clear();
            this.SaveHighScores();
        }

    }
}