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
        private const double ClonesOffset = 30;
        private readonly int MaxPlayerClones = 2;
        private readonly Canvas canvas;
        private readonly double canvasHeight;
        private readonly double canvasWidth;
        public List<Player> players { get; private set; }
        private readonly UiTextManager uiTextManager;
        private readonly GameManager gameManager;
        private readonly BulletManager bulletManager;
        private DateTime lastFireTime;
        private readonly TimeSpan fireCooldown = TimeSpan.FromMilliseconds(200);
        private const int CollisionCheckIntervalMs = 50;
        private int playerLives;
        private DispatcherTimer collisionCheckTimer;
        private const string NoCurrentPowerUp = "No Current Power-Up";
        private bool shieldActive = false;
        private DateTime powerUpEndTime;
        private const int SpeedBoostMultiplier = 2;
        private const int BulletCountMultiplier = 3;
        private DispatcherTimer powerUpTimer;

        /// <summary>
        /// Calls objects moveLeft for each player, ensuring they don't move closer to each other
        /// </summary>
        public void MoveLeft()
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].X > 0)
                {
                    double targetX = players[i].X - players[i].SpeedX;
                    if (CanMoveToPosition(i, targetX, players[i].Width))
                    {
                        players[i].MoveLeft();
                    }
                }
            }
        }

        /// <summary>
        /// Calls objects moveRight for each player, ensuring they don't move closer to each other
        /// </summary>
        public void MoveRight()
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].X + players[i].Width < this.canvasWidth)
                {
                    double targetX = players[i].X + players[i].SpeedX;
                    if (CanMoveToPosition(i, targetX, players[i].Width))
                    {
                        players[i].MoveRight(this.canvasWidth);
                    }
                }
            }
        }

        private bool CanMoveToPosition(int currentPlayerIndex, double targetX, double targetWidth)
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (i != currentPlayerIndex)
                {
                    double otherPlayerX = players[i].X;
                    double otherPlayerWidth = players[i].Width;
                    if (Math.Abs(targetX - (otherPlayerX + otherPlayerWidth)) < ClonesOffset ||
                        Math.Abs((targetX + targetWidth) - otherPlayerX) < ClonesOffset)
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
        /// <param name="canvas"> the canvas that the players will be added onto</param>
        /// <param name="bulletManager"></param>
        /// <param name="uiTextManager"></param>
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
            this.players = new List<Player>();
            this.createAndPlacePlayer();
            this.initializeCollisionCheckTimer();
            this.InitializePowerUpTimer();
        }

        #endregion

        #region Methods

        private void createAndPlacePlayer()
        {
            Player newPlayer = ShipFactory.CreatePlayerShip(this.gameManager.gameType);
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
            this.playerLives--;
            this.uiTextManager.UpdatePlayerLives(this.playerLives);

            if (this.players.Count > 1)
            {
                Player mostRecentPlayer = this.players[this.players.Count - 1];
                var explosionX = mostRecentPlayer.X;
                var explosionY = mostRecentPlayer.Y;
                this.canvas.Children.Remove(mostRecentPlayer.Sprite);
                this.players.Remove(mostRecentPlayer);
                AudioManager.PlayPlayerBlowUp(this.gameManager.gameType);
                _ = ExplosionAnimationManager.Play(this.canvas, explosionX, explosionY, this.gameManager.gameType);
            }

            if (this.playerLives >= 0)
            {
                AudioManager.PlayPlayerBlowUp(this.gameManager.gameType);
            }

            if (this.playerLives == 0)
            {
                var explosionX = this.players[playerIndex].X;
                var explosionY = this.players[playerIndex].Y;
                this.canvas.Children.Remove(this.players[playerIndex].Sprite);
                this.uiTextManager.EndGame(false);
                _ = ExplosionAnimationManager.Play(this.canvas, explosionX, explosionY, this.gameManager.gameType);
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
            for (int i = 0; i < players.Count; i++)
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
                DateTime currentTime = DateTime.Now;
                if (currentTime - this.lastFireTime >= this.fireCooldown)
                {
                    this.bulletManager.PlayersFiring = this.players.Count;
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
            AudioManager.PlayActivePowerUp(this.gameManager.gameType);
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
                this.playerLives++;
                this.uiTextManager.UpdatePlayerLives(this.playerLives);
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

        public void CreateClonePlayer()
        {
            if (this.players.Count < MaxPlayerClones)
            {
                Player clonePlayer = ShipFactory.CreatePlayerShip(this.gameManager.gameType);
                clonePlayer.X = this.players[this.players.Count - 1].X + clonePlayer.Width + ClonesOffset;
                clonePlayer.Y = this.canvasHeight - clonePlayer.Height - PlayerOffsetFromBottom;
                this.players.Add(clonePlayer);
                this.canvas.Children.Add(clonePlayer.Sprite);
                this.updatePlayerPosition(clonePlayer);
            }
        }

        #endregion
    }
}
