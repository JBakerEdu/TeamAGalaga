using System.Diagnostics;
using Windows.UI.Xaml.Navigation;
using Galaga.Model;
using Galaga.Model.ViewModel;

namespace Galaga.View
{
    /// <summary>
    /// Represents the page that displays the high scores in the application.
    /// </summary>
    public sealed partial class HighScorePage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HighScorePage"/> class.
        /// </summary>
        public HighScorePage()
        {
            this.InitializeComponent();

            var navigationService = new NavigationService();
            var viewModel = new HighScoreViewModel(navigationService);
            DataContext = viewModel;

            Debug.WriteLine(DataContext?.GetType().FullName);
        }

        /// <summary>
        /// Called when the page is navigated to. Initializes the view model with the provided navigation parameter.
        /// </summary>
        /// <param name="e">The event data that contains the navigation parameters and other related information.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            (DataContext as HighScoreViewModel)?.Initialize(e.Parameter);
        }
    }
}