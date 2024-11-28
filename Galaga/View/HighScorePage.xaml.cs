using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

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
        }

        private void SortSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var viewModel = this.HighScoreViewModel;
            viewModel.SortOrder = ((ComboBoxItem)((ComboBox)sender).SelectedItem).Content.ToString();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            this.Loaded += (sender, args) =>
            {
                var rootGrid = (Grid)this.Content; // Assuming the root is a Grid
                double containerWidth = rootGrid.ActualWidth;
                double containerHeight = rootGrid.ActualHeight;

                // Resize the window to match the container size
                ApplicationView.GetForCurrentView().TryResizeView(new Windows.Foundation.Size(containerWidth, containerHeight));
            };

            this.HighScoreViewModel = new HighScoreViewModel((int)e.Parameter);
            this.DataContext = this.HighScoreViewModel;
        }
    }
}