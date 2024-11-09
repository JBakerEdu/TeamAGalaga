using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Galaga.View.Sprites;
using System.Linq;

namespace Galaga.Model
{
    public class GameManager
    {
        #region Data members

        private int playerLives = 3;
        private readonly Canvas canvas;
        private BulletManager bulletManager;
        private EnemyManager enemyManager;
        private PlayerManager playerManager;
        private UITextManager uiTextManager;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates the game manager class and init the game
        /// </summary>
        /// <param name="canvas">the canvas passes in</param>
        /// <exception cref="ArgumentNullException"></exception>
        public GameManager(Canvas canvas)
        {
            this.canvas = canvas ?? throw new ArgumentNullException(nameof(canvas));
            this.bulletManager = new BulletManager(this.canvas);
            this.uiTextManager = new UITextManager(this.canvas, this.playerLives);
            this.playerManager = new PlayerManager(this.canvas, this.playerLives, this.bulletManager, this.uiTextManager);
            this.enemyManager = new EnemyManager(this.canvas, this.bulletManager, this.uiTextManager);
            this.bulletManager.EnemyManager = this.enemyManager;
            this.bulletManager.PlayerManager = this.playerManager;
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
