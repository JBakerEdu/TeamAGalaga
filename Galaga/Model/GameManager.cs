using System;
using Windows.UI.Xaml.Controls;
using Galaga.View;

namespace Galaga.Model
{
    /// <summary>
    /// This is the game manger which initializes the game and creates all the managers.
    /// </summary>
    public class GameManager
    {
        #region Data members
        private readonly BonusShipManager bonusShipManager;
        private readonly LevelManager levelManager;
        private readonly int playerLives = 3;
        /// <summary>
        /// the player manager being used
        /// </summary>
        public readonly PlayerManager PlayerManager;
        /// <summary>
        /// the game type set to be played
        /// </summary>
        public GameType GameType { get; private set; }
        #endregion

        #region Constructors

        /// <summary>
        /// Creates the game manager class and init the game
        /// </summary>
        /// <param name="canvas">the canvas passes in</param>
        /// <param name="isHolidayMode">this lets the game know if the user wants to play the holiday game type</param>
        /// <exception cref="ArgumentNullException"> if the canvas is not passed in correctly</exception>
        public GameManager(Canvas canvas, bool isHolidayMode)
        {
            canvas = canvas ?? throw new ArgumentNullException(nameof(canvas));
            this.GameType = isHolidayMode ? GameType.HolidayGame : GameType.OriginalGame;
            var bulletManager = new BulletManager(canvas, this);
            var uiTextManager = new UiTextManager(canvas, this.playerLives, this);
            this.PlayerManager = new PlayerManager(this.playerLives, canvas, bulletManager, this, uiTextManager);
            var enemyManager = new EnemyManager(canvas, bulletManager, uiTextManager, this.PlayerManager, this);
            this.bonusShipManager = new BonusShipManager(canvas, bulletManager, this);
            this.levelManager = new LevelManager(enemyManager, uiTextManager);
            this.levelManager.StartGame();
        }
        #endregion

        #region Methods
        /// <summary>
        /// calls Players moveLeft
        /// </summary>
        public void MovePlayerLeft() => this.PlayerManager.MoveLeft();
        /// <summary>
        /// calls Players moveRight
        /// </summary>
        public void MovePlayerRight() => this.PlayerManager.MoveRight();
        /// <summary>
        /// calls Players fire
        /// </summary>
        public void FireBullet() => this.PlayerManager.FireBullet();
        /// <summary>
        /// calls player to add a life
        /// </summary>
        public void AddLifeToPlayer() => this.PlayerManager.AddLife();
        /// <summary>
        /// calls to clone the Players ship
        /// </summary>
        public void ClonePlayerShip() => this.PlayerManager.CreateClonePlayer();
        /// <summary>
        /// calls the Bonus ship spawn to ensure that the bonus ship does not spawn at the wrong time
        /// </summary>
        /// <param name="spawn"></param>
        public void BonusShipSpawn(bool spawn)
        {
            this.bonusShipManager.BonusShipSpawn = spawn;
        }
        /// <summary>
        /// Gives a power up to the player to handle using the power up
        /// </summary>
        /// <param name="powerUp"> the type of power up to give the player</param>
        public void PlayerPowerUp(PowerUps powerUp)
        {
            this.PlayerManager.ApplyPowerUp(powerUp);
        }
        /// <summary>
        /// calls to move on to next level
        /// </summary>
        public void NextLevel() => this.levelManager.NextLevel();
        /// <summary>
        /// the current game level the game is on
        /// </summary>
        /// <returns> an int value of the level number </returns>
        public int CurrentGameLevel()
        {
            return this.levelManager.CurrentLevel;
        }

        #endregion
    }
}
