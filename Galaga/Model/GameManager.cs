using System;
using Windows.UI.Xaml.Controls;

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
        #endregion

        #region Constructors

        /// <summary>
        /// Creates the game manager class and init the game
        /// </summary>
        /// <param name="canvas">the canvas passes in</param>
        /// <exception cref="ArgumentNullException"></exception>
        public GameManager(Canvas canvas)
        {
            canvas = canvas ?? throw new ArgumentNullException(nameof(canvas));
            BulletManager bulletManager = new BulletManager(canvas);
            UiTextManager uiTextManager = new UiTextManager(canvas, this.playerLives, this);
            this.playerManager = new PlayerManager(this.playerLives, canvas, bulletManager, uiTextManager);
            EnemyManager enemyManager = new EnemyManager(canvas, bulletManager, uiTextManager, this.playerManager, this);
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
        /// calls the Bonus ship spawn to ensure that the bonus ship does not spawn at the wrong time
        /// </summary>
        /// <param name="spawn"></param>
        public void BonusShipSpawn(bool spawn)
        {
            this.bonusShipManager.BonusShipSpawn = spawn;
        }

        public void playerPowerUp(PowerUps powerUp)
        {
            this.playerManager.ApplyPowerUp(powerUp);
        }

        /// <summary>
        /// calls to move on to next level
        /// </summary>
        public void NextLevel() => this.levelManager.NextLevel();

        #endregion
    }
}
