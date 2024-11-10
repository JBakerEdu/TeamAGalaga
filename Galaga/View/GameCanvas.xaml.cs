using Galaga.Model;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using System.Collections.Generic;
using System;

namespace Galaga.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GameCanvas
    {
        private readonly GameManager gameManager;
        private readonly HashSet<VirtualKey> pressedKeys = new HashSet<VirtualKey>();
        private DispatcherTimer movementTimer;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameCanvas"/> class.
        /// </summary>
        public GameCanvas()
        {
            this.InitializeComponent();

            Width = this.canvas.Width;
            Height = this.canvas.Height;
            ApplicationView.PreferredLaunchViewSize = new Size { Width = Width, Height = Height };
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(Width, Height));

            Window.Current.CoreWindow.KeyDown += this.coreWindowOnKeyDown;
            Window.Current.CoreWindow.KeyUp += this.coreWindowOnKeyUp;

            this.gameManager = new GameManager(this.canvas);

            this.movementTimer = new DispatcherTimer();
            this.movementTimer.Interval = TimeSpan.FromMilliseconds(16);
            this.movementTimer.Tick += this.OnMovementTimerTick;
            this.movementTimer.Start();
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
    }
}
