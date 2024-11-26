using System.Collections.Generic;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Galaga.View;
using Galaga.View.Sprites;

namespace Galaga.Model
{
    /// <summary>
    /// This manages all bullet object
    /// </summary>
    public class BulletManager
    {
        #region Data members

        public int maxBulletsAllowed { get; set; }
        private readonly Canvas canvas;
        private readonly double canvasHeight;

        private readonly IList<Bullet> activePlayerBullets;
        private readonly IList<Bullet> activeEnemyBullets;
        private DispatcherTimer bulletMovementTimer;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates the game manager class and init the game
        /// </summary>
        /// <param name="canvas">the canvas passes in</param>
        /// <exception cref="ArgumentNullException"></exception>
        public BulletManager(Canvas canvas)
        {
            this.canvas = canvas;
            this.maxBulletsAllowed = 3;
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
                bullet.MoveUp();

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
                bullet.MoveDown();
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
            if (this.activePlayerBullets.Count < this.maxBulletsAllowed)
            {
                var bullet = new Bullet(new BulletSprite(), 0, 5);
                renderX = renderX - bullet.Width / 2;
                renderY = renderY - bullet.Height;
                bullet.RenderAt(renderX, renderY);
                this.canvas.Children.Add(bullet.Sprite);
                AudioManager.PlayPlayerShoot();
                this.activePlayerBullets.Add(bullet);
            }
        }

        /// <summary>
        /// this is how the enemy fires their fireball
        /// </summary>
        /// <param name="renderX">where to render the x of the sprite</param>
        /// <param name="renderY">where to render the y of the sprite</param>
        public void FireEnemyBullet(double renderX, double renderY)
        {
            var bullet = new Bullet(new BulletSprite(), 0, 5);
            renderX = renderX - bullet.Width / 2;
            bullet.RenderAt(renderX, renderY);
            this.canvas.Children.Add(bullet.Sprite);
            AudioManager.PlayEnemyShoot();
            this.activeEnemyBullets.Add(bullet);
        }

        public bool CheckSpriteCollision(GameObject ship, bool isPlayer)
        {
            var bullets = isPlayer ? this.activeEnemyBullets : this.activePlayerBullets;
            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                var bullet = bullets[i];
                if (IsCollision(bullet, ship))
                {
                    this.canvas.Children.Remove(bullet.Sprite);
                    bullets.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        private bool IsCollision(Bullet bullet, GameObject ship)
        {
            var bulletLeft = bullet.X;
            var bulletRight = bullet.X + bullet.Width;
            var bulletTop = bullet.Y;
            var bulletBottom = bullet.Y + bullet.Height;

            var shipLeft = ship.X;
            var shipRight = ship.X + ship.Width;
            var shipTop = ship.Y;
            var shipBottom = ship.Y + ship.Height;

            return bulletBottom >= shipTop &&
                   bulletTop <= shipBottom &&
                   bulletRight >= shipLeft &&
                   bulletLeft <= shipRight;
        }
    }
}