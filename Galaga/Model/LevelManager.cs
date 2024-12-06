using System;
using Galaga.View;

namespace Galaga.Model
{
    /// <summary>
    /// This manages the Levels and calling for another level to be started. 
    /// </summary>
    public class LevelManager
    {
        private readonly EnemyManager enemyManager;
        private readonly UiTextManager uiTextManager;

        public int currentLevel { get; private set; }
        private const int MaxLevel = 3;
        private const int MinPossibleFireRateMin = 4;
        private const int MinPossibleFireRateMax = 10;
        private const int FireRateMin = 12;
        private const int FireRateMax = 35;

        private const int FirstRowShips = 1;
        private const int SecondRowShips = 2;
        private const int ThirdRowShips = 2;
        private const int FourthRowShips = 3;


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
            if (this.currentLevel > MaxLevel)
            {
                this.uiTextManager.EndGame(true);
                return;
            }

            int[] shipsPerRow = { FirstRowShips + this.currentLevel, SecondRowShips + this.currentLevel, ThirdRowShips + this.currentLevel, FourthRowShips + this.currentLevel };
            var fireIntervalMin = Math.Max(FireRateMin - this.currentLevel, MinPossibleFireRateMin);
            var fireIntervalMax = Math.Max(FireRateMax - this.currentLevel * 2, MinPossibleFireRateMax);

            this.enemyManager.UpdateLevelSettings(shipsPerRow, fireIntervalMin, fireIntervalMax);
            this.uiTextManager.UpdateLevel(this.currentLevel);
        }

        /// <summary>
        /// Progresses to the next level.
        /// </summary>
        public void NextLevel()
        {
            if (this.currentLevel < MaxLevel)
            {
                this.currentLevel++;
                this.SetLevelParameters();
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
            this.currentLevel = 1;
            this.SetLevelParameters();
            this.enemyManager.InitializeEnemies();
        }
    }
}
