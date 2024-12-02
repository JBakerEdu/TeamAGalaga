using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Galaga.View;
using System.Threading.Tasks;
using Galaga.View.Sprites;
using System.Collections.Generic;

namespace Galaga.Model
{
    /// <summary>
    /// This is the manager for the player ships
    /// </summary>
    public class PlayerManager
    {
        #region Data members

        private const double PlayerOffsetFromBottom = 30;
        private readonly Canvas canvas;
        private readonly double canvasHeight;
        private readonly double canvasWidth;
        public List<Player> players { get; private set; }  // Changed to a list of players
        private readonly UiTextManager uiTextManager;
        private readonly BulletManager bulletManager;
        private DateTime lastFireTime;
        private readonly TimeSpan fireCooldown = TimeSpan.FromMilliseconds(200);
        private const int CollisionCheckIntervalMs = 50;
        private List<int> playerLives;  // List to store the lives for each player
        private DispatcherTimer collisionCheckTimer;
        private const string NoCurrentPowerUp = "No Current Power-Up";
        private bool shieldActive = false;
        private DateTime powerUpEndTime;
        private const int SpeedBoostMultiplier = 2;  // Double speed boost
        private const int BulletCountMultiplier = 3;  // Triple bullets
        private DispatcherTimer powerUpTimer;  // Timer for power-up expiration

        /// <summary>
        /// Calls objects moveLeft for each player
        /// </summary>
        public void MoveLeft()
        {
            foreach (var player in players)
            {
                player.MoveLeft();
            }
        }

        /// <summary>
        /// Calls objects moveRight for each player
        /// </summary>
        public void MoveRight()
        {
            foreach (var player in players)
            {
                player.MoveRight(this.canvasWidth);
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs the player manager with all needed information 
        /// </summary>
        /// <param name="lives"> the lives each player has</param>
        /// <param name="canvas"> the canvas that the players will be added onto</param>
        /// <param name="bulletManager"></param>
        /// <param name="uiTextManager"></param>
        public PlayerManager(int lives, Canvas canvas, BulletManager bulletManager, UiTextManager uiTextManager)
        {
            this.canvas = canvas;
            this.canvasHeight = canvas.Height;
            this.canvasWidth = canvas.Width;
            this.uiTextManager = uiTextManager;
            this.uiTextManager.SetPowerUpText(NoCurrentPowerUp);
            this.bulletManager = bulletManager;
            this.lastFireTime = DateTime.Now - this.fireCooldown;
            this.playerLives = new List<int>();  // Initialize list for player lives
            this.players = new List<Player>();  // Initialize the list of players

            this.playerLives.Add(lives);
            this.createAndPlacePlayer();
            

            this.initializeCollisionCheckTimer();
            this.InitializePowerUpTimer();
        }

        #endregion

        #region Methods

        private void createAndPlacePlayer()
        {
            Player newPlayer = ShipFactory.CreatePlayerShip();
            this.players.Add(newPlayer);
            this.canvas.Children.Add(newPlayer.Sprite);
            this.placePlayerNearBottom(newPlayer);
        }

        private void placePlayerNearBottom(Player player)
        {
            player.X = this.canvasWidth / 2 - player.Width / 2.0;
            player.Y = this.canvasHeight - player.Height - PlayerOffsetFromBottom;
            this.updatePlayerPosition(player);
        }

        private void updatePlayerPosition(Player player)
        {
            Canvas.SetLeft(player.Sprite, player.X);
            Canvas.SetTop(player.Sprite, player.Y);
        }

        private void handlePlayerHit(int playerIndex)
        {
            this.playerLives[playerIndex]--;
            this.uiTextManager.UpdatePlayerLives(this.playerLives[playerIndex]);

            if (this.playerLives[playerIndex] >= 0)
            {
                AudioManager.PlayPlayerBlowUp();
            }

            if (this.playerLives[playerIndex] == 0)
            {
                var explosionX = this.players[playerIndex].X;
                var explosionY = this.players[playerIndex].Y;
                this.canvas.Children.Remove(this.players[playerIndex].Sprite);
                this.uiTextManager.EndGame(false);
                _ = ExplosionAnimationManager.Play(this.canvas, explosionX, explosionY);
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
            for (int i = 0; i < players.Count; i++)
            {
                if (!this.shieldActive && this.bulletManager.CheckSpriteCollision(this.players[i], true))
                {
                    this.handlePlayerHit(i);
                }
            }
        }

        public void addLife()
        {
            // Add a life to all players (or you can specify a playerIndex)
            for (int i = 0; i < players.Count; i++)
            {
                this.playerLives[i]++;
                this.uiTextManager.UpdatePlayerLives(this.playerLives[i]);
            }
        }

        /// <summary>
        /// This is how each player fires their bullet; the space bar calls here.
        /// </summary>
        public void FireBullet()
        {
            if (!this.uiTextManager.GameOver)
            {
                DateTime currentTime = DateTime.Now;
                if (currentTime - this.lastFireTime >= this.fireCooldown)
                {
                    foreach (var player in players)
                    {
                        double renderX = player.X + player.Width / 2;
                        double renderY = this.canvasHeight - PlayerOffsetFromBottom;
                        this.bulletManager.PlayerFiresBullet(renderX, renderY);
                    }
                    this.lastFireTime = currentTime;
                }
            }
        }

        #endregion

        #region BonusPowerUps

        public void ApplyPowerUp(PowerUps powerUp)
        {
            AudioManager.PlayActivePowerUp();
            this.uiTextManager.SetPowerUpText(powerUp.ToString());
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
            for (int i = 0; i < players.Count; i++)
            {
                this.playerLives[i]++;
                this.uiTextManager.UpdatePlayerLives(this.playerLives[i]);
            }
        }

        private void ApplySpeedBoost()
        {
            foreach (var player in players)
            {
                player.SpeedX *= SpeedBoostMultiplier;
            }
            SetPowerUpEndTime(() => ResetSpeedBoost());
        }

        private void ActivateShield()
        {
            this.shieldActive = true;
            SetPowerUpEndTime(() => ResetShield());
        }

        private void EnableTripleBullet()
        {
            this.bulletManager.maxBulletsAllowed *= BulletCountMultiplier;
            SetPowerUpEndTime(() => ResetTripleBullet());
        }

        private void SetPowerUpEndTime(Action resetMethod)
        {
            powerUpEndTime = DateTime.Now.AddSeconds(12);
            this.resetMethod = resetMethod;
        }

        private Action resetMethod;

        private void InitializePowerUpTimer()
        {
            this.powerUpTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            this.powerUpTimer.Tick += (sender, args) => UpdatePowerUps();
            this.powerUpTimer.Start();
        }

        private void UpdatePowerUps()
        {
            if (DateTime.Now >= powerUpEndTime && resetMethod != null)
            {
                resetMethod.Invoke();
                resetMethod = null;
            }
        }

        private void ResetSpeedBoost()
        {
            foreach (var player in players)
            {
                player.SpeedX /= SpeedBoostMultiplier;
            }
            this.resetUIPowerUpText();
        }

        private void ResetShield()
        {
            this.shieldActive = false;
            this.resetUIPowerUpText();
        }

        private void ResetTripleBullet()
        {
            this.bulletManager.maxBulletsAllowed /= BulletCountMultiplier;
            this.resetUIPowerUpText();
        }

        private void resetUIPowerUpText()
        {
            this.uiTextManager.SetPowerUpText(NoCurrentPowerUp);
        }

        #endregion
    }
}
