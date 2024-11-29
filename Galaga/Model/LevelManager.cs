using System;

namespace Galaga.Model
{
    /// <summary>
    /// This manages the Levels and calling for another level to be started. 
    /// </summary>
    public class LevelManager
    {
        private readonly EnemyManager enemyManager;
        private readonly UiTextManager uiTextManager;

        private int currentLevel;
        private const int MaxLevel = 3;

        /// <summary>
        /// Constructor to initialize LevelManager with dependencies
        /// </summary>
        /// <param name="enemyManager">the manager of the enemy objects </param>
        /// <param name="uiTextManager">the manager of the text that will appear on the canvas</param>
        /// <exception cref="ArgumentNullException">Thrown if either enemyManager or UiTextManager are null</exception>
        public LevelManager(EnemyManager enemyManager, UiTextManager uiTextManager)
        {
            this.enemyManager = enemyManager ?? throw new ArgumentNullException(nameof(enemyManager));
            this.uiTextManager = uiTextManager ?? throw new ArgumentNullException(nameof(uiTextManager));
            this.currentLevel = 1;
        }

        /// <summary>
        /// Sets the level-specific parameters for the game.
        /// </summary>
        public void SetLevelParameters()
        {
            if (currentLevel > MaxLevel)
            {
                this.uiTextManager.EndGame(true);
                return;
            }

            int[] shipsPerRow = { 1 + this.currentLevel, 2 + this.currentLevel, 3 + this.currentLevel, 4 + this.currentLevel };
            int fireIntervalMin = Math.Max(5 - currentLevel, 1);
            int fireIntervalMax = Math.Max(25 - currentLevel * 2, 5);

            this.enemyManager.UpdateLevelSettings(shipsPerRow, fireIntervalMin, fireIntervalMax);
            this.uiTextManager.UpdateLevel(this.currentLevel);
        }

        /// <summary>
        /// Progresses to the next level.
        /// </summary>
        public void NextLevel()
        {
            if (currentLevel < MaxLevel)
            {
                currentLevel++;
                SetLevelParameters();
                this.enemyManager.InitializeEnemies();
            }
            else
            {
                this.uiTextManager.EndGame(true);
            }
        }

        /// <summary>
        /// Starts the game from the first level.
        /// </summary>
        public void StartGame()
        {
            currentLevel = 1;
            SetLevelParameters();
            this.enemyManager.InitializeEnemies();
        }
    }
}
