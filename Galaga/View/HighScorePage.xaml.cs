using System.Diagnostics;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Galaga.View
{
    public sealed partial class HighScorePage : Page
    {
        public HighScorePage()
        {
            InitializeComponent();

            var navigationService = new NavigationService();
            var viewModel = new HighScoreViewModel(navigationService);
            this.DataContext = viewModel;

            Debug.WriteLine(DataContext?.GetType().FullName);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            (DataContext as HighScoreViewModel)?.Initialize(e.Parameter);
        }
    }
}