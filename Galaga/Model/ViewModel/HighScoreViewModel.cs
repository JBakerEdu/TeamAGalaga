using Galaga.Model;
using Galaga.View;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

public class HighScoreViewModel : INotifyPropertyChanged
{
    private readonly HighScoreManager manager;

    private readonly INavigationService _navigationService;

    private bool _canNavigateBack;
    public bool CanNavigateBack
    {
        get => _canNavigateBack;
        set
        {
            _canNavigateBack = true;
            NavigateBackCommand.RaiseCanExecuteChanged();
        }
    }

    public RelayCommand NavigateBackCommand { get; }

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

    public HighScoreViewModel(NavigationService service)
    {
        this.NavigateBackCommand = new RelayCommand(
            execute: () => NavigateBack(),
            canExecute: () => this.CanNavigateBack);

        this.CanNavigateBack = true;

        this._navigationService = service;

        this.manager = new HighScoreManager();
        this.HighScores = new ObservableCollection<HighScoreEntry>(this.manager.HighScores);
    }

    public void Initialize(object parameter)
    {
        var previousPage = _navigationService.GetPreviousPageType();

        if (previousPage == typeof(StartScreenPage))
        {
            HandleNavigationFromStartScreen();
        }
        else if (previousPage == typeof(GameCanvas))
        {
            HandleNavigationFromGameCanvas(parameter);
        }
        else if (_navigationService.BackStackDepth == 0)
        {
            HandleFirstNavigation();
        }
        else
        {
            HandleUnknownNavigationSource();
        }
    }

    private void HandleNavigationFromStartScreen()
    {
        LoadDefaultHighScores();
    }

    private void HandleNavigationFromGameCanvas(object parameter)
    {
        if (parameter is int score && score >= 0)
        {
            if (CheckIfTop10(score))
            {
                PromptForNameAndAddScore(score);
            }
        }
        else
        {
            Debug.WriteLine("Invalid parameter from GameCanvas. Defaulting to default scores.");
            LoadDefaultHighScores();
        }
    }

    private void HandleUnknownNavigationSource()
    {
        Debug.WriteLine("Unknown navigation source");
        LoadDefaultHighScores();
    }

    private void HandleFirstNavigation()
    {
        Debug.WriteLine("First navigation: Loading default scores.");
        LoadDefaultHighScores();
    }

    private void LoadDefaultHighScores()
    {
        // Logic to initialize the high scores list
    }

    private void NavigateBack()
    {
        var frame = Window.Current.Content as Frame;
        frame?.Navigate(typeof(StartScreenPage));
    }

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
