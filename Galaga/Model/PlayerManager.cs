using System;
using Windows.UI.Xaml.Controls;

namespace Galaga.Model
{
    /// <summary>
    /// This is the manager for the player ship
    /// </summary>
    public class PlayerManager
    {
        #region Data members
        private const double OffsetFromBottom = 30;
        private readonly Canvas canvas;
        private readonly double canvasHeight;
        private readonly double canvasWidth;
        private Player player;
        private readonly UiTextManager uiTextManager;
        private readonly BulletManager bulletManger;
        private DateTime lastFireTime;
        private readonly TimeSpan fireCooldown = TimeSpan.FromMilliseconds(200);
        private int lives;

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
        /// <param name="bulletManger"></param>
        /// <param name="uiTextManager"></param>
        public PlayerManager(int lives, Canvas canvas, BulletManager bulletManger, UiTextManager uiTextManager)
        {
            this.canvas = canvas;
            this.canvasHeight = canvas.Height;
            this.canvasWidth = canvas.Width;
            this.lives = lives;
            this.uiTextManager = uiTextManager;
            this.createAndPlacePlayer();
            this.bulletManger = bulletManger;
            this.lastFireTime = DateTime.Now - this.fireCooldown;
        }

        #endregion

        private void createAndPlacePlayer()
        {
            this.player = ShipFactory.CreatePlayerShip();
            this.canvas.Children.Add(this.player.Sprite);
            this.placePlayerNearBottom();
        }

        private void placePlayerNearBottom()
        {
            this.player.X = this.canvasWidth / 2 - this.player.Width / 2.0;
            this.player.Y = this.canvasHeight - this.player.Height - OffsetFromBottom;
            this.updatePlayerPosition();
        }

        private void updatePlayerPosition()
        {
            Canvas.SetLeft(this.player.Sprite, this.player.X);
            Canvas.SetTop(this.player.Sprite, this.player.Y);
        }

        private void handlePlayerHit()
        {
            this.lives--;
            this.uiTextManager.UpdatePlayerLives(this.lives);
            if (this.lives <= 0)
            {
                this.canvas.Children.Remove(this.player.Sprite);
                this.uiTextManager.EndGame(false);
            }
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
                    double renderY = this.canvasHeight - OffsetFromBottom;

                    this.bulletManger.PlayerFiresBullet(renderX, renderY);
                    this.lastFireTime = currentTime;
                }
            }
        }

        /// <summary>
        /// Checks if a passed in bullet hits the players ship
        /// </summary>
        /// <param name="enemyBullet"></param>
        /// <returns></returns>
        public bool CheckCollision(Bullet enemyBullet)
        {
            bool isHit = this.isCollision(enemyBullet, this.player);
            if (enemyBullet != null && isHit)
            { 
                this.handlePlayerHit();
            }
            return isHit;
        }

        private bool isCollision(Bullet bullet, GameObject ship)
        {
            var bulletLeft = bullet.X;
            var bulletRight = bullet.X + bullet.Width;
            var bulletTop = bullet.Y;
            var bulletBottom = bullet.Y + bullet.Height;

            var enemyLeft = ship.X;
            var enemyWidth = ship.Width;
            var enemyHeight = ship.Height;

            var enemyRight = ship.X + enemyWidth;
            var enemyTop = ship.Y;
            var enemyBottom = ship.Y + enemyHeight;
            if (enemyWidth <= 0 || enemyHeight <= 0)
            {
                return false;
            }
            if (bulletBottom >= enemyTop && bulletTop <= enemyBottom)
            {
                return bulletRight > enemyLeft && bulletLeft < enemyRight;
            }

            return false;
        }
    }
}