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
        private readonly PlayerManager playerManager;
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
            UiTextManager uiTextManager = new UiTextManager(canvas, this.playerLives);
            this.playerManager = new PlayerManager(this.playerLives, canvas, bulletManager, uiTextManager);
            EnemyManager enemyManager = new EnemyManager(canvas, bulletManager, uiTextManager);
            BonusShipManager bonusShipManager = new BonusShipManager(canvas, bulletManager);
        }
        #endregion

        #region Methods
        /// <summary>
        /// calls objects moveLeft
        /// </summary>
        public void MovePlayerLeft() => this.playerManager.MoveLeft();
        /// <summary>
        /// calls objects moveRight
        /// </summary>
        public void MovePlayerRight() => this.playerManager.MoveRight();
        /// <summary>
        /// calls objects fire
        /// </summary>
        public void FireBullet() => this.playerManager.FireBullet();
        #endregion
    }
}
