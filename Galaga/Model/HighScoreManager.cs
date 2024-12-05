using Galaga.Model;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Windows.Storage;

public class HighScoreManager
{
    private const string FileName = "HighScores.xml";
    private readonly StorageFolder _storageFolder = ApplicationData.Current.LocalFolder;

    public ObservableCollection<HighScoreEntry> HighScores { get; set; }

    public HighScoreManager()
    {
        this.HighScores = this.LoadHighScores();
    }

    private ObservableCollection<HighScoreEntry> LoadHighScores()
    {
        try
        {
            var filePath = Path.Combine(this._storageFolder.Path, FileName);
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

    public void SaveHighScores()
    {
        try
        {
            var filePath = Path.Combine(this._storageFolder.Path, FileName);
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

    public void ResetHighScores()
    {
        this.HighScores.Clear();
        this.SaveHighScores();
    }

}
