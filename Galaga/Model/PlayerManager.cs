using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Galaga.View;
using System.Threading.Tasks;

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

        private bool shieldActive = false;
        private DateTime powerUpEndTime;
        private const int SpeedBoostMultiplier = 2;  // Double speed boost
        private const int BulletCountMultiplier = 3;  // Triple bullets
        private DispatcherTimer powerUpTimer;  // Timer for power-up expiration

        /// <summary>
        /// Calls objects moveLeft
        /// </summary>
        public void MoveLeft() => this.player.MoveLeft();

        /// <summary>
        /// Calls objects moveRight
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
            this.InitializePowerUpTimer();
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
            AudioManager.PlayPlayerBlowUp();
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
            if (!this.shieldActive && this.bulletManager.CheckSpriteCollision(this.player, true))
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

        #region BonusPowerUps

        public void ApplyPowerUp(PowerUps powerUp)
        {
            switch (powerUp)
            {
                case PowerUps.ExtraLife:
                    AddLife();
                    break;
                case PowerUps.SpeedBoost:
                    ApplySpeedBoost();
                    break;
                case PowerUps.Shield:
                    ActivateShield();
                    break;
                case PowerUps.TripleBulletCap:
                    EnableTripleBullet();
                    break;
            }
        }

        private void AddLife()
        {
            this.playerLives++;
            this.uiTextManager.UpdatePlayerLives(this.playerLives);
        }

        private void ApplySpeedBoost()
        {
            if (this.player != null)
            {
                this.player.SpeedX *= SpeedBoostMultiplier;
                SetPowerUpEndTime(() => ResetSpeedBoost());  // Start timer, and reset speed when done
            }
        }

        private void ActivateShield()
        {
            this.shieldActive = true;
            SetPowerUpEndTime(() => ResetShield());  // Start timer, and reset shield when done
        }

        private void EnableTripleBullet()
        {
            this.bulletManager.maxBulletsAllowed *= BulletCountMultiplier;  // Enable triple bullets
            SetPowerUpEndTime(() => ResetTripleBullet());  // Start timer, and reset bullet count when done
        }

        private void SetPowerUpEndTime(Action resetMethod)
        {
            // Set power-up duration (example: 10 seconds)
            powerUpEndTime = DateTime.Now.AddSeconds(10);
            // Store the reset method to call when the time is up
            this.resetMethod = resetMethod;
        }

        private Action resetMethod;

        private void InitializePowerUpTimer()
        {
            this.powerUpTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)  // Update every second
            };
            this.powerUpTimer.Tick += (sender, args) => UpdatePowerUps();
            this.powerUpTimer.Start();
        }

        private void UpdatePowerUps()
        {
            if (DateTime.Now >= powerUpEndTime && resetMethod != null)
            {
                // Call the reset method for this power-up
                resetMethod.Invoke();
                resetMethod = null;  // Clear the reset method to avoid repeated calls
            }
        }

        // Reset methods for each power-up

        private void ResetSpeedBoost()
        {
            if (this.player != null)
            {
                this.player.SpeedX /= SpeedBoostMultiplier;  // Reset speed boost
            }
        }

        private void ResetShield()
        {
            this.shieldActive = false;  // Deactivate shield
        }

        private void ResetTripleBullet()
        {
            this.bulletManager.maxBulletsAllowed /= BulletCountMultiplier;  // Reset to normal bullet count
        }

        #endregion
    }
}
