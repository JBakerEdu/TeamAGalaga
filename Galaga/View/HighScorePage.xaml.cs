using System.Diagnostics;
using Windows.Foundation;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Galaga.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HighScorePage : Page
    {
        HighScoreViewModel HighScoreViewModel;

        public HighScorePage()
        {
            this.InitializeComponent();

            this.Loaded += OnPageLoaded;
        }

        private void SortSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            HighScoreViewModel.SortOrder = ((ComboBoxItem)((ComboBox)sender).SelectedItem).Content.ToString();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            CheckNavigationSource(e);

            this.DataContext = this.HighScoreViewModel;
        }

        private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            var containerWidth = this.ActualWidth;
            var containerHeight = this.ActualHeight;

            ApplicationView.GetForCurrentView()
                .TryResizeView(new Size(containerWidth, containerHeight));
        }

        private void BackToStart_Click(object sender, RoutedEventArgs e)
        {
            (Window.Current.Content as Frame)?.Navigate(typeof(StartScreenPage));
        }

        private void CheckNavigationSource(NavigationEventArgs e)
        {
            if (this.Frame.BackStackDepth > 0)
            {
                var previousPage = this.Frame.BackStack[this.Frame.BackStack.Count - 1];
                var sourcePageType = previousPage.SourcePageType;

                if (sourcePageType == typeof(StartScreenPage))
                {
                    HandleNavigationFromStartScreen();
                }
                else if (sourcePageType == typeof(GameCanvas))
                {
                    HandleNavigationFromGameCanvas(e);
                }
                else
                {
                    HandleUnknownNavigationSource();
                }
            }
            else
            {
                HandleFirstNavigation();
            }
        }

        private void HandleNavigationFromStartScreen()
        {
            this.HighScoreViewModel = new HighScoreViewModel();
        }

        private void HandleNavigationFromGameCanvas(NavigationEventArgs e)
        {
            if (e.Parameter is int score)
            {
                this.HighScoreViewModel = new HighScoreViewModel(score);
            }
            else
            {
                Debug.WriteLine("Invalid parameter from GameCanvas. Defaulting ViewModel.");
                this.HighScoreViewModel = new HighScoreViewModel();
            }
        }

        private void HandleUnknownNavigationSource()
        {
            Debug.WriteLine("Unknown navigation source");
        }

        private void HandleFirstNavigation()
        {
            this.HighScoreViewModel = new HighScoreViewModel();
        }
    }
}
