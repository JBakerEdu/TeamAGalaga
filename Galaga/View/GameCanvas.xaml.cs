using Galaga.Model;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using System.Collections.Generic;
using System;
using Windows.UI.Xaml.Navigation;

namespace Galaga.View
{
    public sealed partial class GameCanvas
    {
        private GameManager gameManager;
        private readonly HashSet<VirtualKey> pressedKeys = new HashSet<VirtualKey>();
        private DispatcherTimer movementTimer;
        private bool isHolidayMode;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameCanvas"/> class.
        /// </summary>
        public GameCanvas()
        {
            this.InitializeComponent();
            Loaded += this.OnPageLoaded;

            Width = this.canvas.Width;
            Height = this.canvas.Height;
            ApplicationView.PreferredLaunchViewSize = new Size { Width = Width, Height = Height };
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(Width, Height));

            Window.Current.CoreWindow.KeyDown += this.coreWindowOnKeyDown;
            Window.Current.CoreWindow.KeyUp += this.coreWindowOnKeyUp;

            this.movementTimer = new DispatcherTimer();
            this.movementTimer.Interval = TimeSpan.FromMilliseconds(16);
            this.movementTimer.Tick += this.OnMovementTimerTick;
            this.movementTimer.Start();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter != null)
            {
                this.isHolidayMode = (bool)e.Parameter;
            }

            this.gameManager = new GameManager(this.canvas, this.isHolidayMode);
        }

        private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            var containerWidth = ActualWidth;
            var containerHeight = ActualHeight;

            ApplicationView.GetForCurrentView()
                .TryResizeView(new Size(containerWidth, containerHeight));
        }

        private void coreWindowOnKeyDown(CoreWindow sender, KeyEventArgs args)
        {
            if (!this.pressedKeys.Contains(args.VirtualKey))
            {
                this.pressedKeys.Add(args.VirtualKey);
            }
            if (args.VirtualKey == VirtualKey.Space)
            {
                this.gameManager.FireBullet();
            }
        }

        private void coreWindowOnKeyUp(CoreWindow sender, KeyEventArgs args)
        {
            this.pressedKeys.Remove(args.VirtualKey);
        }

        private void OnMovementTimerTick(object sender, object e)
        {
            if (this.pressedKeys.Contains(VirtualKey.Left))
            {
                this.gameManager.MovePlayerLeft();
            }
            if (this.pressedKeys.Contains(VirtualKey.Right))
            {
                this.gameManager.MovePlayerRight();
            }
        }

        private void showHighScoreBoard()
        {
            Frame.Navigate(typeof(HighScorePage)); // Ensure HighScorePage.xaml is set up with the ViewModel binding
        }
    }
}
