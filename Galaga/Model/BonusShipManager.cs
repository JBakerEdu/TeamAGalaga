using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Galaga.Model
{
    /// <summary>
    /// This is the manager for the bonus ships
    /// </summary>
    public class BonusShipManager
    {
        #region Data Members

        private const int TopOffset = 10;
        private readonly Canvas canvas;
        private readonly BulletManager bulletManager;
        private BonusShip bonusShip;
        private readonly Random random;
        private bool bonusShipActive;
        private const double BonusShipSpeed = 2.0;
        private const int BonusSpawnChance = 5;
        private const int BonusFireChance = 1;
        private const int TimerIntervalMilliseconds = 1000;
        public static bool EnableBonusShipTimer = true;
        private bool canFire = true;
        private const int FireCooldownMilliseconds = 500;

        #endregion

        #region Constructor
        /// <summary>
        /// Constructs the BonusShipManager
        /// </summary>
        /// <param name="canvas">The game canvas</param>
        /// <param name="bulletManager">The bullet manager for managing enemy bullets</param>
        public BonusShipManager(Canvas canvas, BulletManager bulletManager)
        {
            this.canvas = canvas;
            this.bulletManager = bulletManager;
            this.random = new Random();
            this.bonusShipActive = false;

            StartBonusShipTimer();
        }
        #endregion

        #region Methods

        /// <summary>
        /// Attempts to spawn a bonus ship randomly.
        /// </summary>
        public void TrySpawnBonusShip()
        {
            if (bonusShipActive || random.Next(0, 100) >= BonusSpawnChance)
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
        }

        /// <summary>
        /// Moves the bonus ship from right to left.
        /// </summary>
        private async void MoveBonusShip()
        {
            while (this.bonusShipActive && this.bonusShip.X + this.bonusShip.Width > 0)
            {
                await Task.Delay(16);
                this.bonusShip.X -= BonusShipSpeed;
                this.UpdateBonusShipPosition();

                if (canFire && this.random.Next(0, 100) < BonusFireChance)
                {
                    this.FireBullet();
                    StartFireCooldown();
                }
            }

            if (this.bonusShipActive && this.bonusShip.X + this.bonusShip.Width <= 0)
            {
                this.RemoveBonusShip();
            }
        }

        /// <summary>
        /// Starts the cooldown period after firing a shot.
        /// </summary>
        private async void StartFireCooldown()
        {
            canFire = false;
            await Task.Delay(FireCooldownMilliseconds);
            canFire = true;
        }



        /// <summary>
        /// Fires a bullet from the bonus ship using the BulletManager.
        /// </summary>
        private void FireBullet()
        {
            if (this.bonusShip == null)
            {
                return;
            }

            var bulletX = this.bonusShip.X + this.bonusShip.Width / 2;
            var bulletY = this.bonusShip.Y + this.bonusShip.Height;
            this.bulletManager.FireEnemyBullet(bulletX, bulletY);
        }

        /// <summary>
        /// Timer loop that runs as long as EnableBonusShipTimer is true.
        /// </summary>
        private async void StartBonusShipTimer()
        {
            while (true)
            {
                await Task.Delay(TimerIntervalMilliseconds);

                if (EnableBonusShipTimer)
                {
                    TrySpawnBonusShip();
                }
            }
        }

        /// <summary>
        /// Removes the bonus ship from the canvas and deactivates it.
        /// </summary>
        private void RemoveBonusShip()
        {
            if (this.bonusShip != null)
            {
                this.canvas.Children.Remove(this.bonusShip.Sprite);
                this.bonusShipActive = false;
                this.bonusShip = null;
            }
        }

        /// <summary>
        /// Updates the bonus ship's position on the canvas.
        /// </summary>
        private void UpdateBonusShipPosition()
        {
            Canvas.SetLeft(this.bonusShip.Sprite, this.bonusShip.X);
            Canvas.SetTop(this.bonusShip.Sprite, this.bonusShip.Y);
        }
        #endregion
    }
}
