using System.Collections.Generic;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Galaga.View.Sprites;

namespace Galaga.Model
{
    public class BulletManager
    {
        #region Data members

        private const int MaxBulletsAllowed = 3;
        private const double BulletSpeed = 10;
        private readonly Canvas canvas;
        private readonly double canvasHeight;
        private readonly double canvasWidth;
        private const int FireIntervalMin = 30;
        private const int FireIntervalMax = 100;
        private readonly Random random = new Random();
        private readonly List<Bullet> activePlayerBullets;
        private readonly List<Bullet> activeEnemyBullets;
        private DispatcherTimer bulletMovementTimer;
        public PlayerManager PlayerManager { get; set; }
        public EnemyManager EnemyManager { get; set; }

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
            this.canvasHeight = canvas.Height;
            this.canvasWidth = canvas.Width;
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
                    this.removeBullet(i);
                }
                else
                {
                    this.checkPlayerBulletCollisions(i);
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
                else
                {
                    this.checkEnemyBulletCollisions(i);
                }
            }
        }


        private void removeBullet(int index)
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
        public void playerFiresBullet(double renderX, double renderY)
        {
            if (this.activePlayerBullets.Count < MaxBulletsAllowed)
            {
                var bullet = new Bullet(new BulletSprite(), 0, 5);
                renderX = renderX - bullet.Width / 2;
                renderY = renderY - bullet.Height;
                bullet.RenderAt(renderX, renderY);
                this.canvas.Children.Add(bullet.Sprite);
                this.activePlayerBullets.Add(bullet);
            }
        }

        /// <summary>
        /// this is how the enemy fires their fireball
        /// </summary>
        /// <param name="enemy">which enemy is firing</param>
        public void FireEnemyBullet(double renderX, double renderY)
        {
            var bullet = new Bullet(new BulletSprite(), 0, 5);
            renderX = renderX - bullet.Width / 2;
            bullet.RenderAt(renderX, renderY);
            this.canvas.Children.Add(bullet.Sprite);
            this.activeEnemyBullets.Add(bullet);
        }

        public void checkPlayerBulletCollisions(int index)
        {
            var playerBullet = this.activePlayerBullets[index];
            if (playerBullet != null && this.EnemyManager.checkCollision(playerBullet))
            {
                this.removeBullet(index);
            }
        }

        public void checkEnemyBulletCollisions(int index)
        {
            var enemyBullet = this.activeEnemyBullets[index];
            if (enemyBullet != null && this.PlayerManager.checkCollision(enemyBullet))
            {
                this.removeEnemyBullet(index);
            }
        }

    }
}