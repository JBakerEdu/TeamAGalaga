using Galaga.View;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Galaga.Model.ViewModel
{
    /// <summary>
    /// The view model for the high scores
    /// </summary>
    public class HighScoreViewModel : INotifyPropertyChanged
    {
        private readonly HighScoreManager manager;

        private readonly INavigationService navigationService;

        private bool canNavigateBack;

        /// <summary>
        /// Gets or sets a value indicating whether navigation back is allowed.
        /// </summary>
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

        /// <summary>
        /// Gets the command used to navigate back in the navigation stack.
        /// </summary>
        public RelayCommand NavigateBackCommand { get; }

        /// <summary>
        /// Stores the collection of high scores.
        /// </summary>
        private ObservableCollection<HighScoreEntry> highScores;

        /// <summary>
        /// Gets or sets the collection of high scores displayed in the application.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the sort order for the high scores.
        /// </summary>
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

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event for the specified property.
        /// </summary>
        /// <param name="propertyName"> The name of the property that changed </param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HighScoreViewModel"/> class.
        /// </summary>
        /// <param name="service">The navigation service used for handling page navigation.</param>
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

        /// <summary>
        /// Performs initialization for the view model based on the provided parameter.
        /// </summary>
        /// <param name="parameter">The parameter passed during navigation, used to initialize the view model.</param>
        public void Initialize(object parameter)
        {
            var previousPage = this.navigationService.GetPreviousPageType();
            if (previousPage == typeof(GameCanvas))
            {
                this.handleNavigationFromGameCanvas(parameter);
            }
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