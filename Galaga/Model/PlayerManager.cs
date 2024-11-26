using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;

namespace Galaga.Model
{
    /// <summary>
    /// This is the manager for the player ship
    /// </summary>
    public class PlayerManager
    {
        #region Data members

        private const double PlayerOffsetFromBottom = 30;
        private readonly Canvas canvas;
        private readonly double canvasHeight;
        private readonly double canvasWidth;
        private Player player;
        private readonly UiTextManager uiTextManager;
        private readonly BulletManager bulletManager;
        private DateTime lastFireTime;
        private readonly TimeSpan fireCooldown = TimeSpan.FromMilliseconds(200);
        private const int CollisionCheckIntervalMs = 50;
        private int playerLives;
        private DispatcherTimer collisionCheckTimer;

        /// <summary>
        /// calls objects moveLeft
        /// </summary>
        public void MoveLeft() => this.player.MoveLeft();

        /// <summary>
        /// calls objects moveRight
        /// </summary>
        public void MoveRight() => this.player.MoveRight(this.canvasWidth);
        #endregion

        #region Constructors

        /// <summary>
        /// Constructs the player manager with all needed information 
        /// </summary>
        /// <param name="lives"> the lives a player has</param>
        /// <param name="canvas"> the canvas that the player will be added onto</param>
        /// <param name="bulletManager"></param>
        /// <param name="uiTextManager"></param>
        public PlayerManager(int lives, Canvas canvas, BulletManager bulletManager, UiTextManager uiTextManager)
        {
            this.canvas = canvas;
            this.canvasHeight = canvas.Height;
            this.canvasWidth = canvas.Width;
            this.playerLives = lives;
            this.uiTextManager = uiTextManager;
            this.createAndPlacePlayer();
            this.bulletManager = bulletManager;
            this.lastFireTime = DateTime.Now - this.fireCooldown;
            this.initializeCollisionCheckTimer();
        }

        #endregion

        #region Methods

        private void createAndPlacePlayer()
        {
            this.player = ShipFactory.CreatePlayerShip();
            this.canvas.Children.Add(this.player.Sprite);
            this.placePlayerNearBottom();
        }

        private void placePlayerNearBottom()
        {
            this.player.X = this.canvasWidth / 2 - this.player.Width / 2.0;
            this.player.Y = this.canvasHeight - this.player.Height - PlayerOffsetFromBottom;
            this.updatePlayerPosition();
        }

        private void updatePlayerPosition()
        {
            Canvas.SetLeft(this.player.Sprite, this.player.X);
            Canvas.SetTop(this.player.Sprite, this.player.Y);
        }

        private void handlePlayerHit()
        {
            this.playerLives--;
            this.uiTextManager.UpdatePlayerLives(this.playerLives);
            if (this.playerLives <= 0)
            {
                this.canvas.Children.Remove(this.player.Sprite);
                this.uiTextManager.EndGame(false);
            }
        }

        private void initializeCollisionCheckTimer()
        {
            this.collisionCheckTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(CollisionCheckIntervalMs)
            };
            this.collisionCheckTimer.Tick += (sender, args) => this.CheckCollision();
            this.collisionCheckTimer.Start();
        }

        private void CheckCollision()
        {
            if (this.bulletManager.CheckSpriteCollision(this.player, true))
            {
                this.handlePlayerHit();
            }
        }

        public void addLife()
        {
            this.playerLives++;
            this.uiTextManager.UpdatePlayerLives(this.playerLives);
        }

        /// <summary>
        /// This is how the player fires their bullet; the space bar calls here.
        /// </summary>
        public void FireBullet()
        {
            if (!this.uiTextManager.GameOver)
            {
                DateTime currentTime = DateTime.Now;
                if (currentTime - this.lastFireTime >= this.fireCooldown)
                {
                    double renderX = this.player.X + this.player.Width / 2;
                    double renderY = this.canvasHeight - PlayerOffsetFromBottom;

                    this.bulletManager.PlayerFiresBullet(renderX, renderY);
                    this.lastFireTime = currentTime;
                }
            }
        }

        #endregion
    }
}
