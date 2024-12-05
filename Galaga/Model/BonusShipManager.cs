using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Galaga.View;

namespace Galaga.Model
{
    public class BonusShipManager
    {
        #region Data Members

        private const int TopOffset = 40;
        private readonly Canvas canvas;
        private readonly BulletManager bulletManager;
        private readonly GameManager gameManager;
        private BonusShip bonusShip;
        private readonly Random random;
        private bool bonusShipActive;
        private const double BonusShipSpeed = 2.0;
        private const int BonusSpawnChance = 100;
        private const int BonusFireChance = 1;
        private const int TimerIntervalMilliseconds = 1000;
        private const int SoundEffectMilliseconds = 500;
        private const int FireCooldownMilliseconds = 500;
        private const int MinimumSpawnDelayMilliseconds = 25000; // 15 seconds

        private bool canFire = true;
        private DateTime lastSpawnTime; // Tracks the last spawn time

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
            this.lastSpawnTime = DateTime.MinValue; // Initialize to a very old time
            this.StartBonusShipTimer();
            this.BonusShipSpawn = true;
        }
        #endregion

        #region Methods

        public void TrySpawnBonusShip()
        {
            // Check if enough time has passed since the last spawn
            if (this.bonusShipActive ||
                this.random.Next(0, 100) >= BonusSpawnChance ||
                (DateTime.Now - this.lastSpawnTime).TotalMilliseconds < MinimumSpawnDelayMilliseconds)
            {
                return;
            }

            this.bonusShip = ShipFactory.CreateBonusShip(this.gameManager.gameType);
            this.canvas.Children.Add(this.bonusShip.Sprite);
            this.bonusShip.X = this.canvas.Width;
            this.bonusShip.Y = TopOffset;
            this.UpdateBonusShipPosition();
            this.bonusShipActive = true;
            this.lastSpawnTime = DateTime.Now; // Update the last spawn time
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
                    this.FireBullet(this.gameManager.playerManager.players[0]);
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

        private void FireBullet(Player player)
        {
            if (this.bonusShip == null || !this.BonusShipSpawn)
            {
                return;
            }

            var bulletX = this.bonusShip.X + this.bonusShip.Width / 2;
            var bulletY = this.bonusShip.Y + this.bonusShip.Height;

            this.bulletManager.FireEnemyBullet(bulletX, bulletY, player.X + player.Width / 2, player.Y + player.Height / 2, true);
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
                AudioManager.PlayActiveBonusShip(this.gameManager.gameType);
                await Task.Delay(SoundEffectMilliseconds);
            }
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
            AudioManager.PlayEnemyBlowUp(this.gameManager.gameType);
            if (this.gameManager.CurrentGameLevel() > 1)
            {
                this.gameManager.ClonePlayerShip();
            }
            else
            {
                this.gameManager.AddLifeToPlayer();
            }
            PowerUps randomPowerUp = (PowerUps)this.random.Next(Enum.GetValues(typeof(PowerUps)).Length);
            this.gameManager.playerPowerUp(randomPowerUp);
        }

        #endregion
    }
}
