using System.Collections.Generic;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Galaga.View;

namespace Galaga.Model
{
    /// <summary>
    /// This manages all bullet object
    /// </summary>
    public class BulletManager
    {
        #region Data members
        private const double EnemyBulletSpeed = 5;
        private readonly Canvas canvas;
        private readonly double canvasHeight;
        private readonly IList<Bullet> activePlayerBullets;
        private readonly IList<Bullet> activeEnemyBullets;
        private DispatcherTimer bulletMovementTimer;
        private GameManager gameManager;
        private double velocityX = 0;
        private double enemyVelocityY = 5;
        private double playerVelocityY = -5;
        /// <summary>
        /// this is the max bullets allowed for a single player
        /// </summary>
        public int MaxBulletsAllowed { get; set; }
        /// <summary>
        /// the number of Players on the team firing at a time
        /// </summary>
        public int PlayersFiring { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates the game manager class and init the game
        /// </summary>
        /// <param name="canvas">the canvas passes in</param>
        /// <param name="gameManager">the game manger to report back to</param>
        /// <exception cref="ArgumentNullException"></exception>
        public BulletManager(Canvas canvas, GameManager gameManager)
        {
            this.canvas = canvas;
            this.gameManager = gameManager;
            this.MaxBulletsAllowed = 3;
            this.canvasHeight = canvas.Height;
            this.activePlayerBullets = new List<Bullet>();
            this.activeEnemyBullets = new List<Bullet>();
            this.initializeBulletTimer();
        }
        #endregion

        private void initializeBulletTimer()
        {
            this.bulletMovementTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(16)
            };
            this.bulletMovementTimer.Tick += this.OnBulletTimerTick;
            this.bulletMovementTimer.Start();
        }

        private void OnBulletTimerTick(object sender, object e)
        {
            this.updatePlayerBullets();
            this.updateEnemyBullets();
        }

        private void updatePlayerBullets()
        {
            for (var i = this.activePlayerBullets.Count - 1; i >= 0; i--)
            {
                var bullet = this.activePlayerBullets[i];
                bullet.Move();

                if (bullet.IsOffScreen(this.canvasHeight))
                {
                    this.removePlayerBullet(i);
                }
            }
        }

        private void updateEnemyBullets()
        {
            for (var i = this.activeEnemyBullets.Count - 1; i >= 0; i--)
            {
                var bullet = this.activeEnemyBullets[i];
                bullet.Move();
                if (bullet.IsOffScreen(this.canvasHeight))
                {
                    this.removeEnemyBullet(i);
                }
            }
        }

        private void removePlayerBullet(int index)
        {
            var bullet = this.activePlayerBullets[index];
            if (bullet != null)
            {
                this.canvas.Children.Remove(bullet.Sprite);
                this.activePlayerBullets.RemoveAt(index);
            }
        }

        private void removeEnemyBullet(int index)
        {
            var bullet = this.activeEnemyBullets[index];
            this.canvas.Children.Remove(bullet.Sprite);
            this.activeEnemyBullets.RemoveAt(index);
        }

        /// <summary>
        /// this is how the player fires their fireball, the space bar calls here
        /// </summary>
        public void PlayerFiresBullet(double renderX, double renderY)
        {
            if (this.activePlayerBullets.Count < this.MaxBulletsAllowed * this.PlayersFiring)
            {
                var bullet = BulletFactory.CreateBullet(this.velocityX, this.playerVelocityY, this.gameManager.GameType);
                renderX = renderX - bullet.Sprite.Width / 2;
                renderY = renderY - bullet.Sprite.Height;
                bullet.RenderAt(renderX, renderY);
                this.canvas.Children.Add(bullet.Sprite);
                AudioManager.PlayPlayerShoot(this.gameManager.GameType);
                this.activePlayerBullets.Add(bullet);
            }
        }

        /// <summary>
        /// this is how the enemy fires their fireball
        /// </summary>
        /// <param name="renderX">where to render the x of the sprite</param>
        /// <param name="renderY">where to render the y of the sprite</param>
        /// <param name="playerX"></param>
        /// <param name="playerY"></param>
        /// <param name="aimedAtPlayer"></param>
        public void FireEnemyBullet(double renderX, double renderY, double playerX = 0, double playerY = 0, bool aimedAtPlayer = false)
        {
            var bulletVelocityX = this.velocityX;
            var bulletVelocityY = this.enemyVelocityY;

            if (aimedAtPlayer)
            {
                var deltaX = playerX - renderX;
                var deltaY = playerY - renderY;

                var magnitude = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
                bulletVelocityX = deltaX / magnitude * EnemyBulletSpeed;
                bulletVelocityY = deltaY / magnitude * EnemyBulletSpeed;
            }

            var bullet = BulletFactory.CreateBullet(bulletVelocityX, bulletVelocityY, this.gameManager.GameType);
            renderX = renderX - bullet.Width / 2;
            bullet.RenderAt(renderX, renderY);
            this.canvas.Children.Add(bullet.Sprite);
            AudioManager.PlayEnemyShoot(this.gameManager.GameType);
            this.activeEnemyBullets.Add(bullet);
        }

        /// <summary>
        /// Checks it the ships passed in collide with active  bullets
        /// </summary>
        /// <param name="ship">the ship to check if colliding</param>
        /// <param name="isPlayer">lets the method know if the ship is a plyer to ensure not checking against its own bullets</param>
        /// <returns>a bool of it the sprites collide or not</returns>
        public bool CheckSpriteCollision(GameObject ship, bool isPlayer)
        {
            var bullets = isPlayer ? this.activeEnemyBullets : this.activePlayerBullets;
            for (var i = bullets.Count - 1; i >= 0; i--)
            {
                var bullet = bullets[i];
                if (this.isCollision(bullet, ship))
                {
                    this.canvas.Children.Remove(bullet.Sprite);
                    bullets.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        private bool isCollision(Bullet bullet, GameObject ship)
        {
            var bulletLeft = bullet.X;
            var bulletRight = bullet.X + bullet.Width;
            var bulletTop = bullet.Y;
            var bulletBottom = bullet.Y + bullet.Height;

            var shipLeft = ship.X;
            var shipRight = ship.X + ship.Width;
            var shipTop = ship.Y;
            var shipBottom = ship.Y + ship.Height;

            return bulletBottom >= shipTop && bulletTop <= shipBottom && bulletRight >= shipLeft && bulletLeft <= shipRight;
        }
    }
}