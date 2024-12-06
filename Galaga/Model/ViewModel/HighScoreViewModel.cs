using Galaga.View;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Galaga.Model.ViewModel
{

    public class HighScoreViewModel : INotifyPropertyChanged
    {
        private readonly HighScoreManager manager;

        private readonly INavigationService navigationService;

        private bool canNavigateBack;
        public bool CanNavigateBack
        {
            get => this.canNavigateBack;
            set
            {
                this.canNavigateBack = value;
                this.canNavigateBack = true;
                this.NavigateBackCommand.RaiseCanExecuteChanged();
            }
        }

        public RelayCommand NavigateBackCommand { get; }

        public ObservableCollection<HighScoreEntry> highScores;

        public ObservableCollection<HighScoreEntry> HighScores
        {
            get => this.highScores;
            set
            {
                this.highScores = value;
                this.OnPropertyChanged();
            }
        }

        private string sortOrder = "Score";
        public string SortOrder
        {
            get => this.sortOrder;
            set
            {
                this.sortOrder = value;
                this.sortHighScores();
                this.OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public HighScoreViewModel(NavigationService service)
        {
            this.NavigateBackCommand = new RelayCommand(
                execute: () => this.navigateBack(),
                canExecute: () => this.CanNavigateBack);

            this.CanNavigateBack = true;

            this.navigationService = service;

            this.manager = new HighScoreManager();
            this.HighScores = new ObservableCollection<HighScoreEntry>(this.manager.HighScores);
        }

        public void Initialize(object parameter)
        {
            var previousPage = this.navigationService.GetPreviousPageType();

            if (previousPage == typeof(StartScreenPage))
            {
                this.handleNavigationFromStartScreen();
            }
            else if (previousPage == typeof(GameCanvas))
            {
                this.handleNavigationFromGameCanvas(parameter);
            }
            else if (this.navigationService.BackStackDepth == 0)
            {
                this.handleFirstNavigation();
            }
            else
            {
                this.handleUnknownNavigationSource();
            }
        }

        private void handleNavigationFromStartScreen()
        {
            this.LoadDefaultHighScores();
        }

        private void handleNavigationFromGameCanvas(object parameter)
        {
            if (parameter is int score && score >= 0)
            {
                if (this.checkIfTop10(score))
                {
                    this.promptForNameAndAddScore(score);
                }
            }
            else
            {
                Debug.WriteLine("Invalid parameter from GameCanvas. Defaulting to default scores.");
                this.LoadDefaultHighScores();
            }
        }

        private void handleUnknownNavigationSource()
        {
            Debug.WriteLine("Unknown navigation source");
            this.LoadDefaultHighScores();
        }

        private void handleFirstNavigation()
        {
            Debug.WriteLine("First navigation: Loading default scores.");
            this.LoadDefaultHighScores();
        }

        private void LoadDefaultHighScores()
        {
            // Logic to initialize the high scores list
        }

        private void navigateBack()
        {
            var frame = Window.Current.Content as Frame;
            frame?.Navigate(typeof(StartScreenPage));
        }

        private void sortHighScores()
        {
            var sortedScores = this.SortOrder switch
            {
                "Name" => this.HighScores.OrderBy(h => h.PlayerName).ThenByDescending(h => h.Score).ThenByDescending(h => h.Level),
                "Level" => this.HighScores.OrderByDescending(h => h.Level).ThenByDescending(h => h.Score).ThenBy(h => h.PlayerName),
                _ => this.HighScores.OrderByDescending(h => h.Score).ThenBy(h => h.PlayerName).ThenByDescending(h => h.Level)
            };

            this.HighScores = new ObservableCollection<HighScoreEntry>(sortedScores);
        }

        private bool checkIfTop10(int score)
        {
            return this.manager.HighScores.Count < 10 ||
                   this.manager.HighScores.Any(entry => score > entry.Score);
        }

        private async void promptForNameAndAddScore(int score)
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
                var playerName = ((TextBox)nameInputDialog.Content).Text;
                this.manager.AddScore(playerName, score);

                this.HighScores = new ObservableCollection<HighScoreEntry>(this.manager.HighScores);
            }
        }
    }

}