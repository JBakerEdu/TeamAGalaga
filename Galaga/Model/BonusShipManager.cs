using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Galaga.View;

namespace Galaga.Model
{
    /// <summary>
    /// Manages the Bonus Ships
    /// </summary>
    public class BonusShipManager
    {
        #region Data Members

        private const int TopOffset = 40;
        private const int RandomMaxValue = 100;
        private const int MaxIterations = 500;
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
        private const int FireCooldownMilliseconds = 500;
        private const int MinimumSpawnDelayMilliseconds = 25000;
        private bool canFire = true;
        private DateTime lastSpawnTime;

        /// <summary>
        /// Turns off the spawn of the bonus ship giving control to turn off spawn chance. 
        /// </summary>
        public bool BonusShipSpawn { get; set; }

        #endregion

        #region Constructor
        /// <summary>
        /// Constructs a bonus ship manager to be manipulated
        /// </summary>
        /// <param name="canvas"> the canvas to add to</param>
        /// <param name="bulletManager">the bullet manager a bonus ship will use</param>
        /// <param name="gameManager">the game manager the bonusShipManager will report back to</param>
        public BonusShipManager(Canvas canvas, BulletManager bulletManager, GameManager gameManager)
        {
            this.gameManager = gameManager;
            this.canvas = canvas;
            this.bulletManager = bulletManager;
            this.random = new Random();
            this.bonusShipActive = false;
            this.lastSpawnTime = DateTime.MinValue;
            this.startBonusShipTimer();
            this.BonusShipSpawn = true;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Attempts to spawn a bonus ship in the game.
        /// </summary>
        public void TrySpawnBonusShip()
        {
            if (this.bonusShipActive || this.random.Next(0, RandomMaxValue) >= BonusSpawnChance || (DateTime.Now - this.lastSpawnTime).TotalMilliseconds < MinimumSpawnDelayMilliseconds)
            {
                return;
            }

            this.bonusShip = ShipFactory.CreateBonusShip(this.gameManager.GameType);
            this.canvas.Children.Add(this.bonusShip.Sprite);
            this.bonusShip.X = this.canvas.Width;
            this.bonusShip.Y = TopOffset;
            this.updateBonusShipPosition();
            this.bonusShipActive = true;
            this.lastSpawnTime = DateTime.Now;
            this.moveBonusShip();
            this.startBonusShipActivityLoop();
        }

        private async void moveBonusShip()
        {
            while (this.bonusShipActive && this.bonusShip.X + this.bonusShip.Width > 0)
            {
                await Task.Delay(30);
                this.bonusShip.X -= BonusShipSpeed;
                this.updateBonusShipPosition();
                if (this.checkCollision())
                {
                    this.handleBonusShipHit();
                    return;
                }

                if (this.canFire && this.random.Next(0, RandomMaxValue) < BonusFireChance)
                {
                    this.fireBullet(this.gameManager.PlayerManager.Players[0]);
                    this.startFireCooldown();
                }
            }

            if (this.bonusShipActive && this.bonusShip.X + this.bonusShip.Width <= 0)
            {
                this.removeBonusShip();
            }
        }

        private async void startFireCooldown()
        {
            this.canFire = false;
            await Task.Delay(FireCooldownMilliseconds);
            this.canFire = true;
        }

        private void fireBullet(Player player)
        {
            if (this.bonusShip == null || !this.BonusShipSpawn)
            {
                return;
            }

            var bulletX = this.bonusShip.X + this.bonusShip.Width / 2;
            var bulletY = this.bonusShip.Y + this.bonusShip.Height;

            this.bulletManager.FireEnemyBullet(bulletX, bulletY, player.X + player.Width / 2, player.Y + player.Height / 2, true);
        }


        private async void startBonusShipTimer()
        {
            for (var i = 0; i < MaxIterations; i++)
            {
                await Task.Delay(TimerIntervalMilliseconds);

                if (this.BonusShipSpawn)
                {
                    this.TrySpawnBonusShip();
                }
            }
        }

        private async void startBonusShipActivityLoop()
        {
            while (this.bonusShipActive)
            {
                AudioManager.PlayActiveBonusShip(this.gameManager.GameType);
                await Task.Delay(SoundEffectMilliseconds);
            }
        }

        private void removeBonusShip()
        {
            if (this.bonusShip != null)
            {
                this.canvas.Children.Remove(this.bonusShip.Sprite);
                this.bonusShipActive = false;
                this.bonusShip = null;
            }
        }

        private void updateBonusShipPosition()
        {
            Canvas.SetLeft(this.bonusShip.Sprite, this.bonusShip.X);
            Canvas.SetTop(this.bonusShip.Sprite, this.bonusShip.Y);
        }

        /// <summary>
        /// Checks if the bonus ship has collided with any player bullet.
        /// </summary>
        private bool checkCollision()
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
        private void handleBonusShipHit()
        {
            this.removeBonusShip();
            AudioManager.PlayEnemyBlowUp(this.gameManager.GameType);
            if (this.gameManager.CurrentGameLevel() > 1)
            {
                this.gameManager.ClonePlayerShip();
            }
            else
            {
                this.gameManager.AddLifeToPlayer();
            }
            var randomPowerUp = (PowerUps)this.random.Next(Enum.GetValues(typeof(PowerUps)).Length);
            this.gameManager.PlayerPowerUp(randomPowerUp);
        }

        #endregion
    }
}
