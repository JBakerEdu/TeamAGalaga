using System.Diagnostics;
using Windows.UI.Xaml.Navigation;

namespace Galaga.View
{
    public sealed partial class HighScorePage
    {
        public HighScorePage()
        {
            this.InitializeComponent();

            var navigationService = new NavigationService();
            var viewModel = new HighScoreViewModel(navigationService);
            DataContext = viewModel;

            Debug.WriteLine(DataContext?.GetType().FullName);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            (DataContext as HighScoreViewModel)?.Initialize(e.Parameter);
        }
    }
}