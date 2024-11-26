using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Galaga.View;

namespace Galaga.Model
{
    public class BonusShipManager
    {
        #region Data Members

        private const int TopOffset = 10;
        private readonly Canvas canvas;
        private readonly BulletManager bulletManager;
        private readonly GameManager gameManager;
        private BonusShip bonusShip;
        private readonly Random random;
        private bool bonusShipActive;
        private const double BonusShipSpeed = 2.0;
        private const int BonusSpawnChance = 5;
        private const int BonusFireChance = 1;
        private const int TimerIntervalMilliseconds = 1000;
        private const int SoundEffectMilliseconds = 500;
        private bool canFire = true;
        private const int FireCooldownMilliseconds = 500;

        /// <summary>
        /// Turns off the spawn of the bonus ship giving control to turn off spawn chance. 
        /// </summary>
        public bool BonusShipSpawn { get; set; }

        #endregion

        #region Constructor
        public BonusShipManager(Canvas canvas, BulletManager bulletManager, GameManager gameManager)
        {
            this.gameManager = gameManager;
            this.canvas = canvas;
            this.bulletManager = bulletManager;
            this.random = new Random();
            this.bonusShipActive = false;
            this.StartBonusShipTimer();
            this.BonusShipSpawn = true;
        }
        #endregion

        #region Methods

        public void TrySpawnBonusShip()
        {
            if (this.bonusShipActive || this.random.Next(0, 100) >= BonusSpawnChance)
            {
                return;
            }
            this.bonusShip = new BonusShip();
            this.canvas.Children.Add(this.bonusShip.Sprite);
            this.bonusShip.X = this.canvas.Width;
            this.bonusShip.Y = TopOffset;
            this.UpdateBonusShipPosition();
            this.bonusShipActive = true;
            this.MoveBonusShip();
            this.StartBonusShipActivityLoop();
        }

        private async void MoveBonusShip()
        {
            while (this.bonusShipActive && this.bonusShip.X + this.bonusShip.Width > 0)
            {
                await Task.Delay(30);
                this.bonusShip.X -= BonusShipSpeed;
                this.UpdateBonusShipPosition();
                if (CheckCollision())
                {
                    HandleBonusShipHit();
                    return;
                }

                if (canFire && this.random.Next(0, 100) < BonusFireChance)
                {
                    this.FireBullet();
                    this.StartFireCooldown();
                }
            }

            if (this.bonusShipActive && this.bonusShip.X + this.bonusShip.Width <= 0)
            {
                this.RemoveBonusShip();
            }
        }

        private async void StartFireCooldown()
        {
            this.canFire = false;
            await Task.Delay(FireCooldownMilliseconds);
            this.canFire = true;
        }

        private void FireBullet()
        {
            if (this.bonusShip == null || !this.BonusShipSpawn)
            {
                return;
            }

            var bulletX = this.bonusShip.X + this.bonusShip.Width / 2;
            var bulletY = this.bonusShip.Y + this.bonusShip.Height;
            this.bulletManager.FireEnemyBullet(bulletX, bulletY);
        }

        private async void StartBonusShipTimer()
        {
            while (true)
            {
                await Task.Delay(TimerIntervalMilliseconds);

                if (this.BonusShipSpawn)
                {
                    this.TrySpawnBonusShip();
                }
            }
        }

        private async void StartBonusShipActivityLoop()
        {
            while (this.bonusShipActive)
            {
                PlayActiveBonusShip();
                await Task.Delay(SoundEffectMilliseconds);
            }
        }

        private static void PlayActiveBonusShip()
        {
            AudioManager.PlayActiveBonusShip();
        }

        private void RemoveBonusShip()
        {
            if (this.bonusShip != null)
            {
                this.canvas.Children.Remove(this.bonusShip.Sprite);
                this.bonusShipActive = false;
                this.bonusShip = null;
            }
        }

        private void UpdateBonusShipPosition()
        {
            Canvas.SetLeft(this.bonusShip.Sprite, this.bonusShip.X);
            Canvas.SetTop(this.bonusShip.Sprite, this.bonusShip.Y);
        }

        /// <summary>
        /// Checks if the bonus ship has collided with any player bullet.
        /// </summary>
        private bool CheckCollision()
        {
            if (this.bonusShip == null)
            {
                return false;
            }

            return this.bulletManager.CheckSpriteCollision(this.bonusShip, false);
        }

        /// <summary>
        /// Handles the case when the bonus ship is hit by a bullet.
        /// </summary>
        private void HandleBonusShipHit()
        {
            RemoveBonusShip();
            this.gameManager.AddLifeToPlayer(); // Add life or any other default behavior.

            // Randomly assign a power-up to the player
            PowerUps randomPowerUp = (PowerUps)this.random.Next(Enum.GetValues(typeof(PowerUps)).Length);
            this.gameManager.playerPowerUp(randomPowerUp);
        }

        #endregion
    }
}
