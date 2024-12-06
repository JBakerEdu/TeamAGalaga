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
        public readonly PlayerManager playerManager;
        private readonly BonusShipManager bonusShipManager;
        private readonly LevelManager levelManager;
        private readonly int playerLives = 3;
        public GameType gameType { get; private set; }
        #endregion

        #region Constructors

        /// <summary>
        /// Creates the game manager class and init the game
        /// </summary>
        /// <param name="canvas">the canvas passes in</param>
        /// <exception cref="ArgumentNullException"></exception>
        public GameManager(Canvas canvas, bool isHolidayMode)
        {
            canvas = canvas ?? throw new ArgumentNullException(nameof(canvas));
            this.gameType = isHolidayMode ? GameType.HolidayGame : GameType.OriginalGame;
            var bulletManager = new BulletManager(canvas, this);
            var uiTextManager = new UiTextManager(canvas, this.playerLives, this);
            this.playerManager = new PlayerManager(this.playerLives, canvas, bulletManager, this, uiTextManager);
            var enemyManager = new EnemyManager(canvas, bulletManager, uiTextManager, this.playerManager, this);
            this.bonusShipManager = new BonusShipManager(canvas, bulletManager, this);
            this.levelManager = new LevelManager(enemyManager, uiTextManager);
            this.levelManager.StartGame();
        }
        #endregion

        #region Methods
        /// <summary>
        /// calls players moveLeft
        /// </summary>
        public void MovePlayerLeft() => this.playerManager.MoveLeft();
        /// <summary>
        /// calls players moveRight
        /// </summary>
        public void MovePlayerRight() => this.playerManager.MoveRight();
        /// <summary>
        /// calls players fire
        /// </summary>
        public void FireBullet() => this.playerManager.FireBullet();
        /// <summary>
        /// calls player to add a life
        /// </summary>
        public void AddLifeToPlayer() => this.playerManager.addLife();
        /// <summary>
        /// calls to clone the players ship
        /// </summary>
        public void ClonePlayerShip() => this.playerManager.createClonePlayer();
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
            this.playerManager.ApplyPowerUp(powerUp);
        }
        /// <summary>
        /// calls to move on to next level
        /// </summary>
        public void NextLevel() => this.levelManager.NextLevel();

        public int CurrentGameLevel()
        {
            return this.levelManager.currentLevel;
        }

        #endregion
    }
}
