using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Galaga.Model;

namespace Galaga.View
{
    /// <summary>
    /// Represents the start screen page of the application.
    /// </summary>
    public sealed partial class StartScreenPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StartScreenPage"/> class.
        /// </summary>
        public StartScreenPage()
        {
            this.InitializeComponent();
        }

        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            var isHolidayMode = this.holidayModeCheckBox.IsChecked ?? false;
            (Window.Current.Content as Frame)?.Navigate(typeof(GameCanvas), isHolidayMode);
        }

        private void ViewHighScores_Click(object sender, RoutedEventArgs e)
        {
            (Window.Current.Content as Frame)?.Navigate(typeof(HighScorePage));
        }

        private void ResetHighScores_Click(object sender, RoutedEventArgs e)
        {
            var manager = new HighScoreManager();
            manager.HighScores.Clear();
            manager.SaveHighScores();

            var dialog = new ContentDialog
            {
                Title = "Reset Complete",
                Content = "High scores have been reset.",
                CloseButtonText = "OK"
            };
            _ = dialog.ShowAsync();
        }
    }
}
