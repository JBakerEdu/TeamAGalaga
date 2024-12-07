using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Galaga.View;
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
        private const double ClonesOffset = 30;
        private const int MaxPlayerClones = 2;
        private readonly Canvas canvas;
        private readonly double canvasHeight;
        private readonly double canvasWidth;
        private readonly UiTextManager uiTextManager;
        private readonly GameManager gameManager;
        private readonly BulletManager bulletManager;
        private DateTime lastFireTime;
        private readonly TimeSpan fireCooldown = TimeSpan.FromMilliseconds(200);
        private const int CollisionCheckIntervalMs = 50;
        private int playerLives;
        private DispatcherTimer collisionCheckTimer;
        private const string NoCurrentPowerUp = "No Current Power-Up";
        private bool shieldActive;
        private DateTime powerUpEndTime;
        private const int SpeedBoostMultiplier = 2;
        private const int BulletCountMultiplier = 3;
        private DispatcherTimer powerUpTimer;
        /// <summary>
        /// the list of Players 
        /// </summary>
        public List<Player> Players { get; private set; }

        /// <summary>
        /// Calls objects moveLeft for each player, ensuring they don't move closer to each other
        /// </summary>
        public void MoveLeft()
        {
            for (var i = 0; i < this.Players.Count; i++)
            {
                if (this.Players[i].X > 0)
                {
                    var targetX = this.Players[i].X - this.Players[i].SpeedX;
                    if (this.canMoveToPosition(i, targetX, this.Players[i].Width))
                    {
                        this.Players[i].MoveLeft();
                    }
                }
            }
        }

        /// <summary>
        /// Calls objects moveRight for each player, ensuring they don't move closer to each other
        /// </summary>
        public void MoveRight()
        {
            for (var i = 0; i < this.Players.Count; i++)
            {
                if (this.Players[i].X + this.Players[i].Width < this.canvasWidth)
                {
                    var targetX = this.Players[i].X + this.Players[i].SpeedX;
                    if (this.canMoveToPosition(i, targetX, this.Players[i].Width))
                    {
                        this.Players[i].MoveRight(this.canvasWidth);
                    }
                }
            }
        }

        private bool canMoveToPosition(int currentPlayerIndex, double targetX, double targetWidth)
        {
            for (var i = 0; i < this.Players.Count; i++)
            {
                if (i != currentPlayerIndex)
                {
                    var otherPlayerX = this.Players[i].X;
                    var otherPlayerWidth = this.Players[i].Width;
                    if (Math.Abs(targetX - (otherPlayerX + otherPlayerWidth)) < ClonesOffset ||
                        Math.Abs(targetX + targetWidth - otherPlayerX) < ClonesOffset)
                    {
                        return false;
                    }
                }
            }
            return true;
        }



        #endregion

        #region Constructors

        /// <summary>
        /// Constructs the player manager with all needed information 
        /// </summary>
        /// <param name="lives"> the lives each player has</param>
        /// <param name="canvas"> the canvas that the Players will be added onto</param>
        /// <param name="bulletManager">the bullets manager that is used</param>
        /// <param name="gameManager">the game manger to report back to</param>
        /// <param name="uiTextManager">the Ui text manager that will control the Ui</param>
        public PlayerManager(int lives, Canvas canvas, BulletManager bulletManager, GameManager gameManager, UiTextManager uiTextManager)
        {
            this.canvas = canvas;
            this.canvasHeight = canvas.Height;
            this.canvasWidth = canvas.Width;
            this.gameManager = gameManager;
            this.uiTextManager = uiTextManager;
            this.uiTextManager.SetPowerUpText(NoCurrentPowerUp);
            this.bulletManager = bulletManager;
            this.lastFireTime = DateTime.Now - this.fireCooldown;
            this.playerLives = lives;
            this.Players = new List<Player>();
            this.createAndPlacePlayer();
            this.initializeCollisionCheckTimer();
            this.initializePowerUpTimer();
        }

        #endregion

        #region Methods

        private void createAndPlacePlayer()
        {
            var newPlayer = ShipFactory.CreatePlayerShip(this.gameManager.GameType);
            this.Players.Add(newPlayer);
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
            this.playerLives--;
            this.uiTextManager.UpdatePlayerLives(this.playerLives);

            if (this.Players.Count > 1)
            {
                var mostRecentPlayer = this.Players[this.Players.Count - 1];
                var explosionX = mostRecentPlayer.X;
                var explosionY = mostRecentPlayer.Y;
                this.canvas.Children.Remove(mostRecentPlayer.Sprite);
                this.Players.Remove(mostRecentPlayer);
                AudioManager.PlayPlayerBlowUp(this.gameManager.GameType);
                _ = ExplosionAnimationManager.Play(this.canvas, explosionX, explosionY, this.gameManager.GameType);
            }

            if (this.playerLives >= 0)
            {
                AudioManager.PlayPlayerBlowUp(this.gameManager.GameType);
            }

            if (this.playerLives == 0)
            {
                var explosionX = this.Players[playerIndex].X;
                var explosionY = this.Players[playerIndex].Y;
                this.canvas.Children.Remove(this.Players[playerIndex].Sprite);
                this.uiTextManager.EndGame(false);
                _ = ExplosionAnimationManager.Play(this.canvas, explosionX, explosionY, this.gameManager.GameType);
            }
        }

        private void initializeCollisionCheckTimer()
        {
            this.collisionCheckTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(CollisionCheckIntervalMs)
            };
            this.collisionCheckTimer.Tick += (sender, args) => this.checkCollision();
            this.collisionCheckTimer.Start();
        }

        private void checkCollision()
        {
            for (var i = 0; i < this.Players.Count; i++)
            {
                if (!this.shieldActive && this.bulletManager.CheckSpriteCollision(this.Players[i], true))
                {
                    this.handlePlayerHit(i);
                }
            }
        }

        /// <summary>
        /// add a life to the player
        /// </summary>
        public void AddLife()
        {
            for (var i = 0; i < this.Players.Count; i++)
            {
                this.playerLives++;
                this.uiTextManager.UpdatePlayerLives(this.playerLives);
            }
        }

        /// <summary>
        /// This is how each player fires their bullet; the space bar calls here.
        /// </summary>
        public void FireBullet()
        {
            if (!this.uiTextManager.GameOver)
            {
                var currentTime = DateTime.Now;
                if (currentTime - this.lastFireTime >= this.fireCooldown)
                {
                    this.bulletManager.PlayersFiring = this.Players.Count;
                    foreach (var player in this.Players)
                    {
                        var renderX = player.X + player.Width / 2;
                        var renderY = this.canvasHeight - PlayerOffsetFromBottom;
                        this.bulletManager.PlayerFiresBullet(renderX, renderY);
                    }
                    this.lastFireTime = currentTime;
                }
            }
        }

        #endregion

        #region BonusPowerUps

        /// <summary>
        /// applies a power up the Players team
        /// </summary>
        /// <param name="powerUp"> the type of power up to add</param>
        public void ApplyPowerUp(PowerUps powerUp)
        {
            AudioManager.PlayActivePowerUp(this.gameManager.GameType);
            this.uiTextManager.SetPowerUpText(powerUp.ToString());
            switch (powerUp)
            {
                case PowerUps.ExtraLife:
                    this.AddLife();
                    break;
                case PowerUps.SpeedBoost:
                    this.applySpeedBoost();
                    break;
                case PowerUps.Shield:
                    this.activateShield();
                    break;
                case PowerUps.TripleBulletCap:
                    this.enableTripleBullet();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(powerUp), powerUp, null);
            }
        }

        private void applySpeedBoost()
        {
            foreach (var player in this.Players)
            {
                player.SpeedX *= SpeedBoostMultiplier;
            }
            this.setPowerUpEndTime(() => this.resetSpeedBoost());
        }

        private void activateShield()
        {
            this.shieldActive = true;
            this.setPowerUpEndTime(() => this.resetShield());
        }

        private void enableTripleBullet()
        {
            this.bulletManager.MaxBulletsAllowed *= BulletCountMultiplier;
            this.setPowerUpEndTime(() => this.resetTripleBullet());
        }

        private void setPowerUpEndTime(Action restartMethod)
        {
            this.powerUpEndTime = DateTime.Now.AddSeconds(12);
            this.resetMethod = restartMethod;
        }

        private Action resetMethod;

        private void initializePowerUpTimer()
        {
            this.powerUpTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            this.powerUpTimer.Tick += (sender, args) => this.updatePowerUps();
            this.powerUpTimer.Start();
        }

        private void updatePowerUps()
        {
            if (DateTime.Now >= this.powerUpEndTime && this.resetMethod != null)
            {
                this.resetMethod.Invoke();
                this.resetMethod = null;
            }
        }

        private void resetSpeedBoost()
        {
            foreach (var player in this.Players)
            {
                player.SpeedX /= SpeedBoostMultiplier;
            }
            this.resetUiPowerUpText();
        }

        private void resetShield()
        {
            this.shieldActive = false;
            this.resetUiPowerUpText();
        }

        private void resetTripleBullet()
        {
            this.bulletManager.MaxBulletsAllowed /= BulletCountMultiplier;
            this.resetUiPowerUpText();
        }

        private void resetUiPowerUpText()
        {
            this.uiTextManager.SetPowerUpText(NoCurrentPowerUp);
        }

        /// <summary>
        /// creates another player when called
        /// </summary>
        public void CreateClonePlayer()
        {
            if (this.Players.Count < MaxPlayerClones)
            {
                var clonePlayer = ShipFactory.CreatePlayerShip(this.gameManager.GameType);
                clonePlayer.X = this.Players[this.Players.Count - 1].X + clonePlayer.Width + ClonesOffset;
                clonePlayer.Y = this.canvasHeight - clonePlayer.Height - PlayerOffsetFromBottom;
                this.Players.Add(clonePlayer);
                this.canvas.Children.Add(clonePlayer.Sprite);
                this.updatePlayerPosition(clonePlayer);
            }
        }

        #endregion
    }
}
