using Galaga.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml.Controls;

public class HighScoreViewModel : INotifyPropertyChanged
{
    private readonly HighScoreManager manager;

    public ObservableCollection<HighScoreEntry> highScores;

    public ObservableCollection<HighScoreEntry> HighScores
    {
        get => highScores;
        set
        {
            highScores = value;
            OnPropertyChanged();
        }
    }

    private string _sortOrder = "Score";
    public string SortOrder
    {
        get => _sortOrder;
        set
        {
            _sortOrder = value;
            SortHighScores();
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public HighScoreViewModel(int score)
    {
        this.manager = new HighScoreManager();
        this.HighScores = new ObservableCollection<HighScoreEntry>(this.manager.HighScores);
        if (score >= 0 && CheckIfTop10(score))
        {
            PromptForNameAndAddScore(score);
        }
    }

    public HighScoreViewModel() : this(-1) { }

    private void SortHighScores()
    {
        var sortedScores = SortOrder switch
        {
            "Name" => HighScores.OrderBy(h => h.PlayerName).ThenByDescending(h => h.Score).ThenByDescending(h => h.Level),
            "Level" => HighScores.OrderByDescending(h => h.Level).ThenByDescending(h => h.Score).ThenBy(h => h.PlayerName),
            _ => HighScores.OrderByDescending(h => h.Score).ThenBy(h => h.PlayerName).ThenByDescending(h => h.Level),
        };

        this.HighScores = new ObservableCollection<HighScoreEntry>(sortedScores);
    }

    private bool CheckIfTop10(int score)
    {
        return this.manager.HighScores.Count < 10 ||
               this.manager.HighScores.Any(entry => score > entry.Score);
    }

    private async void PromptForNameAndAddScore(int score)
    {
        var nameInputDialog = new ContentDialog
        {
            Title = "New High Score!",
            Content = new TextBox
            {
                PlaceholderText = "Enter your name"
            },
            PrimaryButtonText = "Submit",
            CloseButtonText = "Cancel"
        };

        if (await nameInputDialog.ShowAsync() == ContentDialogResult.Primary)
        {
            string playerName = ((TextBox)nameInputDialog.Content).Text;
            manager.AddScore(playerName, score);

            this.HighScores = new ObservableCollection<HighScoreEntry>(this.manager.HighScores);
        }
    }
}
