using System.Collections.Generic;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Galaga.View.Sprites;
using System.Linq;
using Galaga.View;

namespace Galaga.Model
{
    /// <summary>
    /// This is the manager of all enemy ships
    /// </summary>
    public class EnemyManager
    {
        #region Data members

        private const double MovementSpeed = 0.2;
        private const double MaxMoveDistance = 10;
        private readonly Canvas canvas;
        private readonly double canvasWidth;
        private int tickCounter;
        private int FireIntervalMin = 5;
        private int[] shipsPerRow = { 3, 4, 4, 5 };
        private readonly double[] rowHeights = { 320, 260, 180, 100 };
        private int FireIntervalMax = 25;
        private readonly Random random = new Random();
        private IList<EnemyShip> ships;
        private IList<double> originalShipPositions;
        private bool movingRight = true;
        private DispatcherTimer enemyMovementTimer;
        private readonly BulletManager bulletManager;
        private readonly UiTextManager uiTextManager;
        private readonly PlayerManager playerManager;
        private readonly GameManager gameManager;

        public static Action<bool> OnGameEnd;


        #endregion

        #region Constructors

        /// <summary>
        /// Creates the game manager class and init the game
        /// </summary>
        /// <param name="canvas">the canvas to work on</param>
        /// <param name="bulletManager">the bullet manager so it can talk about if a bullet hits an enemy </param>
        /// <param name="uiTextManager"> the UiTextManger so it can call when games ends and to add a score of the enemies when hit</param>
        public EnemyManager(Canvas canvas, BulletManager bulletManager, UiTextManager uiTextManager, PlayerManager playerManager, GameManager gameManager)
        {
            this.canvas = canvas;
            this.canvasWidth = canvas.Width;
            this.uiTextManager = uiTextManager;
            this.bulletManager = bulletManager;
            this.playerManager = playerManager;
            this.gameManager = gameManager;
            this.InitializeTimer();
        }

        #endregion

        public void UpdateLevelSettings(int[] shipsPerRow, int fireIntervalMin, int fireIntervalMax)
        {
            this.shipsPerRow = shipsPerRow;
            this.FireIntervalMin = fireIntervalMin;
            this.FireIntervalMax = fireIntervalMax;
        }

        private void InitializeTimer()
        {
            this.enemyMovementTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };
            this.enemyMovementTimer.Tick += this.OnEnemyTimerTick;
            this.enemyMovementTimer.Start();
        }

        private void OnEnemyTimerTick(object sender, object e)
        {
            this.UpdateEnemiesMovement();
            this.CheckCollision();
            this.UpdateFiring();
        }

        private void UpdateFiring()
        {
            this.tickCounter++;
            if (this.ships.Count > 0 && this.tickCounter >= FireIntervalMin)
            {
                var randomFireTick = this.random.Next(FireIntervalMin, FireIntervalMax + 1);

                if (this.tickCounter % randomFireTick == 0)
                {
                    int randomIndex = this.random.Next(this.ships.Count);
                    var enemy = this.ships[randomIndex];
                    this.FireEnemyBullet(enemy, this.playerManager.player);
                }
            }
        }

        public void InitializeEnemies()
        {
            this.ships = new List<EnemyShip>();
            this.originalShipPositions = new List<double>();
            for (var row = 0; row < this.shipsPerRow.Length; row++)
            {
                var shipCount = this.shipsPerRow[row];
                var rowY = this.rowHeights[row];
                var shipSpacing = (this.canvasWidth - shipCount) / (shipCount + 1);
                for (var i = 0; i < shipCount; i++)
                {
                    this.PlaceInitialShips(row, i, shipSpacing, rowY);
                }
            }
        }

        private void PlaceInitialShips(int row, int shipIndex, double shipSpacing, double rowY)
        {
            var enemyLevel = row + 1;

            EnemyShip enemyShip = this.CreateNewShip(enemyLevel);

            var shipX = shipSpacing + shipIndex * shipSpacing - enemyShip.Width / 2;
            enemyShip.RenderAt(shipX, rowY);

            this.canvas.Children.Add(enemyShip.Sprite);

            if (enemyShip.HasSecondSprite)
            {
                this.canvas.Children.Add(enemyShip.Sprite2);
            }

            this.ships.Add(enemyShip);
            this.originalShipPositions.Add(shipX);
        }



        private EnemyShip CreateNewShip(int enemyLevel)
        {
            EnemyShip enemyShip;
            switch (enemyLevel)
            {
                case 1:
                    enemyShip = ShipFactory.CreateEnemyShipLevel1();
                    break;
                case 2:
                    enemyShip = ShipFactory.CreateEnemyShipLevel2();
                    break;
                case 3:
                    enemyShip = ShipFactory.CreateEnemyShipLevel3();
                    break;
                case 4:
                    enemyShip = ShipFactory.CreateEnemyShipLevel4();
                    break;
                default:
                    enemyShip = ShipFactory.CreateEnemyShipLevel1();
                    break;
            }
            return enemyShip;
        }

        /// <summary>
        /// this updates how the enemy move around
        /// </summary>z
        private void UpdateEnemiesMovement()
        {
            if (this.ships.Count == 0 || this.ships.All(e => e == null))
            {
                this.gameManager.NextLevel();
                return;
            }

            var firstEnemy = this.ships.FirstOrDefault(e => e != null);
            if (firstEnemy == null)
            {
                this.gameManager.NextLevel();
                return;
            }

            var moveAmount = this.movingRight ? MovementSpeed : -MovementSpeed;
            var currentDistanceFromStart = Math.Abs(firstEnemy.X - this.originalShipPositions[this.ships.IndexOf(firstEnemy)]);

            if (currentDistanceFromStart + Math.Abs(moveAmount) > MaxMoveDistance)
            {
                this.movingRight = !this.movingRight;
                moveAmount = this.movingRight ? MovementSpeed : -MovementSpeed;
            }

            foreach (var enemy in this.ships)
            {
                if (enemy == null)
                {
                    continue;
                }

                enemy.Move(moveAmount);
                this.UpdateEnemyPosition(enemy);

                if (enemy.HasSecondSprite)
                {
                    if (enemy.Sprite2.Visibility != Visibility.Visible)
                    {
                        enemy.Sprite2.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        enemy.Sprite2.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        private void UpdateEnemyPosition(EnemyShip enemy)
        {
            Canvas.SetLeft(enemy.Sprite, enemy.X);
            Canvas.SetTop(enemy.Sprite, enemy.Y);
            if (enemy.HasSecondSprite)
            {
                Canvas.SetLeft(enemy.Sprite2, enemy.X);
                Canvas.SetTop(enemy.Sprite2, enemy.Y);
            }
        }

        /// <summary>
        /// This is how the enemy fires their fireball.
        /// </summary>
        /// <param name="enemy">The enemy firing the bullet.</param>
        private void FireEnemyBullet(EnemyShip enemy, Player player)
        {
            if (enemy != null && enemy.IsShooter && !this.uiTextManager.GameOver)
            {
                double renderX = enemy.X + enemy.Width / 2;
                double renderY = enemy.Y + enemy.Height;
                bool aimedAtPlayer = enemy.Level == 4;
                this.bulletManager.FireEnemyBullet(renderX, renderY, player.X + player.Width / 2, player.Y + player.Height / 2, aimedAtPlayer);
            }
        }


        private void DestroyEnemy(EnemyShip enemy)
        {
            int index = this.ships.IndexOf(enemy);

            if (index >= 0)
            {
                this.canvas.Children.Remove(enemy.Sprite);

                if (enemy.HasSecondSprite)
                {
                    this.canvas.Children.Remove(enemy.Sprite2);
                }

                this.ships.RemoveAt(index);
                this.originalShipPositions.RemoveAt(index);
            }
            this.ships = this.ships.Where(ship => ship != null).ToList();
        }

        private void CheckCollision()
        {
            foreach (var enemy in this.ships.ToList())
            {
                if (enemy != null && this.bulletManager.CheckSpriteCollision(enemy, false))
                {
                    this.DestroyEnemy(enemy);
                    AudioManager.PlayEnemyBlowUp();
                    this.uiTextManager.UpdateScore(enemy);
                }
            }
        }
    }
}