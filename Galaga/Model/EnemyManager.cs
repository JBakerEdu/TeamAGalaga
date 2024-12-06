using System.Collections.Generic;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
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
        private int fireIntervalMin;
        private int[] shipsPerRow;
        private readonly double[] rowHeights = { 320, 260, 180, 100 };
        private int fireIntervalMax;
        private readonly Random random = new Random();
        private IList<EnemyShip> ships;
        private IList<double> originalShipPositions;
        private bool movingRight = true;
        private DispatcherTimer attackTimer;
        private readonly double attackSpeed = 2.5;
        private const int MinAttackInterval = 5000;
        private const int MaxAttackInterval = 15000;
        private readonly IList<EnemyShip> attackingShips = new List<EnemyShip>();
        private DispatcherTimer enemyMovementTimer;
        private readonly BulletManager bulletManager;
        private readonly UiTextManager uiTextManager;
        private readonly PlayerManager playerManager;
        private readonly GameManager gameManager;
        /// <summary>
        /// ends the game and shows the ending screens / text
        /// </summary>
        public static Action<bool> OnGameEnd;


        #endregion

        #region Constructors

        /// <summary>
        /// Creates the game manager class and init the game
        /// </summary>
        /// <param name="canvas">the canvas to work on</param>
        /// <param name="bulletManager">the bullet manager so it can talk about if a bullet hits an enemy </param>
        /// <param name="uiTextManager"> the UiTextManger so it can call when games ends and to add a score of the enemies when hit</param>
        /// <param name="playerManager">the player manager the team is up against</param>
        /// <param name="gameManager">the game manger to report back to</param>
        public EnemyManager(Canvas canvas, BulletManager bulletManager, UiTextManager uiTextManager, PlayerManager playerManager, GameManager gameManager)
        {
            this.canvas = canvas;
            this.canvasWidth = canvas.Width;
            this.uiTextManager = uiTextManager;
            this.bulletManager = bulletManager;
            this.playerManager = playerManager;
            this.gameManager = gameManager;
            this.initializeTimer();
            this.initializeAttackTimer();
        }

        #endregion

        /// <summary>
        /// Updates the data for new levels to change difficulty
        /// </summary>
        /// <param name="shipsPerLine">the list of int values telling how many ships would be in each row</param>
        /// <param name="minFireInterval">the lowest the fire interval can be</param>
        /// <param name="maxFireInterval">the highest the fire interval can be</param>
        public void UpdateLevelSettings(int[] shipsPerLine, int minFireInterval, int maxFireInterval)
        {
            this.shipsPerRow = shipsPerLine;
            this.fireIntervalMin = minFireInterval;
            this.fireIntervalMax = maxFireInterval;
        }

        private void initializeTimer()
        {
            this.enemyMovementTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };
            this.enemyMovementTimer.Tick += this.OnEnemyTimerTick;
            this.enemyMovementTimer.Start();
        }

        private void initializeAttackTimer()
        {
            this.attackTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(this.random.Next(MinAttackInterval, MaxAttackInterval))
            };
            this.attackTimer.Tick += this.OnAttackTimerTick;
            this.attackTimer.Start();
        }

        private void OnAttackTimerTick(object sender, object e)
        {
            var level4Ships = this.ships.Where(ship => ship.Level == 4 && !this.attackingShips.Contains(ship)).ToList();

            if (level4Ships.Count > 0)
            {
                var attackingShip = level4Ships[this.random.Next(level4Ships.Count)];
                this.attackingShips.Add(attackingShip);
                this.startAttackPattern(attackingShip, this.playerManager.Players[0]);
            }

            this.attackTimer.Interval = TimeSpan.FromMilliseconds(this.random.Next(MinAttackInterval, MaxAttackInterval));
        }

        private void startAttackPattern(EnemyShip attackingShip, Player player)
        {
            if (this.uiTextManager.GameOver || attackingShip == null)
            {
                return;
            }
            var playerShip = player;
            if (playerShip == null)
            {
                return;
            }
            var attackTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(16) };
            var targetX = playerShip.X;
            var targetY = playerShip.Y;
            var speedX = (targetX - attackingShip.X) / 500.0 * this.attackSpeed;
            var speedY = (targetY - attackingShip.Y) / 500.0 * this.attackSpeed;

            attackTimer.Tick += (s, e) =>
            {
                attackingShip.X += speedX;
                attackingShip.Y += speedY;

                Canvas.SetLeft(attackingShip.Sprite, attackingShip.X);
                Canvas.SetTop(attackingShip.Sprite, attackingShip.Y);

                if (this.random.Next(0, 1000) < 3 && this.ships.Contains(attackingShip))
                {
                    this.bulletManager.FireEnemyBullet(attackingShip.X + attackingShip.Width / 2, attackingShip.Y + attackingShip.Height, playerShip.X, playerShip.Y, true);
                }

                if (Math.Abs(attackingShip.X - targetX) < 5 && Math.Abs(attackingShip.Y - targetY) < 5)
                {
                    attackTimer.Stop();
                    this.attackingShips.Remove(attackingShip);
                    this.resetEnemyPosition(attackingShip);
                }

            };

            attackTimer.Start();
        }

        private void resetEnemyPosition(EnemyShip attackingShip)
        {
            if (attackingShip == null)
            {
                return;
            }
            var originalIndex = this.ships.IndexOf(attackingShip);

            if (originalIndex >= 0 && originalIndex < this.originalShipPositions.Count)
            {
                attackingShip.X = this.originalShipPositions[originalIndex];
                attackingShip.Y = this.rowHeights[attackingShip.Level - 1];

                Canvas.SetLeft(attackingShip.Sprite, attackingShip.X);
                Canvas.SetTop(attackingShip.Sprite, attackingShip.Y);
            }
            else
            {
                attackingShip.X = 0;
                attackingShip.Y = this.rowHeights[0];
            }
        }




        private void OnEnemyTimerTick(object sender, object e)
        {
            this.updateEnemiesMovement();
            this.checkCollision();
            this.updateFiring();
        }

        private void updateFiring()
        {
            this.tickCounter++;
            if (this.ships.Count > 0 && this.tickCounter >= this.fireIntervalMin)
            {
                var randomFireTick = this.random.Next(this.fireIntervalMin, this.fireIntervalMax + 1);

                if (this.tickCounter % randomFireTick == 0)
                {
                    var randomIndex = this.random.Next(this.ships.Count);
                    var enemy = this.ships[randomIndex];
                    this.fireEnemyBullet(enemy, this.playerManager.Players[0]);
                }
            }
        }

        /// <summary>
        /// Inits the 
        /// </summary>
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
                    this.placeInitialShips(row, i, shipSpacing, rowY);
                }
            }
        }

        private void placeInitialShips(int row, int shipIndex, double shipSpacing, double rowY)
        {
            var enemyLevel = row + 1;

            var enemyShip = this.createNewShip(enemyLevel);

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



        private EnemyShip createNewShip(int enemyLevel)
        {
            EnemyShip enemyShip;
            switch (enemyLevel)
            {
                case 1:
                    enemyShip = ShipFactory.CreateEnemyShipLevel1(this.gameManager.GameType);
                    break;
                case 2:
                    enemyShip = ShipFactory.CreateEnemyShipLevel2(this.gameManager.GameType);
                    break;
                case 3:
                    enemyShip = ShipFactory.CreateEnemyShipLevel3(this.gameManager.GameType);
                    break;
                case 4:
                    enemyShip = ShipFactory.CreateEnemyShipLevel4(this.gameManager.GameType);
                    break;
                default:
                    enemyShip = ShipFactory.CreateEnemyShipLevel1(this.gameManager.GameType);
                    break;
            }
            return enemyShip;
        }

        /// <summary>
        /// this updates how the enemy move around
        /// </summary>z
        private void updateEnemiesMovement()
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
                this.updateEnemyPosition(enemy);
                this.handleSecondSprite(enemy);
            }
        }

        private void handleSecondSprite(EnemyShip enemy)
        {
            if (enemy.HasSecondSprite)
            {
                if (enemy.Sprite2.Visibility != Visibility.Visible)
                {
                    enemy.Sprite2.Visibility = Visibility.Visible;
                    enemy.Sprite.Visibility = Visibility.Collapsed;
                }
                else
                {
                    enemy.Sprite.Visibility = Visibility.Visible;
                    enemy.Sprite2.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void updateEnemyPosition(EnemyShip enemy)
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
        /// <param name="player">the player ship firing</param>
        private void fireEnemyBullet(EnemyShip enemy, Player player)
        {
            if (enemy != null && enemy.IsShooter && !this.uiTextManager.GameOver)
            {
                var renderX = enemy.X + enemy.Width / 2;
                var renderY = enemy.Y + enemy.Height;
                var aimedAtPlayer = enemy.Level == 4;
                this.bulletManager.FireEnemyBullet(renderX, renderY, player.X + player.Width / 2, player.Y + player.Height / 2, aimedAtPlayer);
            }
        }


        private void destroyEnemy(EnemyShip enemy)
        {
            var index = this.ships.IndexOf(enemy);
            if (index >= 0)
            {
                var explosionX = enemy.X;
                var explosionY = enemy.Y;
                this.canvas.Children.Remove(enemy.Sprite);
                _ = ExplosionAnimationManager.Play(this.canvas, explosionX, explosionY, this.gameManager.GameType);
                if (enemy.HasSecondSprite)
                {
                    this.canvas.Children.Remove(enemy.Sprite2);
                }
                this.ships.RemoveAt(index);
                this.originalShipPositions.RemoveAt(index);
                if (this.attackingShips.Contains(enemy))
                {
                    this.attackingShips.Remove(enemy);
                }
            }
        }


        private void checkCollision()
        {
            foreach (var enemy in this.ships.ToList())
            {
                if (enemy != null && this.bulletManager.CheckSpriteCollision(enemy, false))
                {
                    this.destroyEnemy(enemy);
                    AudioManager.PlayEnemyBlowUp(this.gameManager.GameType);
                    this.uiTextManager.UpdateScore(enemy);
                }
            }
        }
    }
}