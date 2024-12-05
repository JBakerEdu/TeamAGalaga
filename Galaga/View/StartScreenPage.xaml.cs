using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Galaga.View
{
    public sealed partial class StartScreenPage
    {
        public StartScreenPage()
        {
            this.InitializeComponent();
        }

        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            var isHolidayMode = this.HolidayModeCheckBox.IsChecked ?? false;
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
