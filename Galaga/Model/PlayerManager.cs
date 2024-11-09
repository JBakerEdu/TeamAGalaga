using System.Collections.Generic;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Galaga.View.Sprites;
using System.Reflection;

namespace Galaga.Model
{
    public class PlayerManager
    {

        #region Data members

        private const double PlayerOffsetFromBottom = 30;
        private readonly Canvas canvas;
        private readonly double canvasHeight;
        private readonly double canvasWidth;
        private Player player;
        private int lifeCount;
        private UITextManager uiTextManager;
        private BulletManager bulletManger;
        private DateTime lastFireTime;
        private readonly TimeSpan fireCooldown = TimeSpan.FromMilliseconds(200);

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

        public PlayerManager(Canvas canvas, int playerLives, BulletManager bulletManger, UITextManager uiTextManager)
        {
            this.canvas = canvas;
            this.canvasHeight = canvas.Height;
            this.canvasWidth = canvas.Width;
            this.lifeCount = playerLives;
            this.uiTextManager = uiTextManager;
            this.createAndPlacePlayer();
            this.bulletManger = bulletManger;
            lastFireTime = DateTime.Now - fireCooldown;
        }

        #endregion

        private void createAndPlacePlayer()
        {
            this.player = new Player();
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
            this.lifeCount--;
            this.uiTextManager.updatePlayerLives(this.lifeCount);
            if (this.lifeCount <= 0)
            {
                this.canvas.Children.Remove(this.player.Sprite);
                this.uiTextManager.endGame(false);
            }
        }


        /// <summary>
        /// This is how the player fires their bullet; the space bar calls here.
        /// </summary>
        public void FireBullet()
        {
            if (!this.uiTextManager.gameOver)
            {
                DateTime currentTime = DateTime.Now;
                if (currentTime - lastFireTime >= fireCooldown)
                {
                    double renderX = this.player.X + this.player.Width / 2;
                    double renderY = this.canvasHeight - PlayerOffsetFromBottom;

                    this.bulletManger.playerFiresBullet(renderX, renderY);
                    lastFireTime = currentTime; // Update the last fire time
                }
            }
        }

        public bool checkCollision(Bullet enemyBullet)
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