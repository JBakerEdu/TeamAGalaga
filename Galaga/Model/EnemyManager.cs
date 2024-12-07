using System.Collections.Generic;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using System.Linq;
using Galaga.View;
using Galaga.Model.MovementPattern;

namespace Galaga.Model
{
    /// <summary>
    /// This is the manager of all enemy ships
    /// </summary>
    public class EnemyManager
    {
        #region Constants
        private const double MovementSpeed = 2;
        private const double MaxMoveDistance = 50;
        private const double AttackSpeed = 2.5;
        private const int MinAttackInterval = 5000;
        private const int MaxAttackInterval = 15000;
        private const int Offset = 150;
        private const int AttackChanceThreshold = 750;
        private const int BulletFireChance = 10;

        #endregion

        #region Fields
        private readonly Canvas canvas;
        private readonly Random random = new Random();
        private readonly UiTextManager uiTextManager;
        private readonly BulletManager bulletManager;
        private readonly PlayerManager playerManager;
        private readonly GameManager gameManager;
        private DispatcherTimer enemyMovementTimer;
        private DispatcherTimer attackTimer;
        private IList<EnemyShip> ships;
        private IList<double> originalShipPositions;
        private readonly IList<EnemyShip> attackingShips = new List<EnemyShip>();
        private int[] shipsPerRow;
        private int fireIntervalMin;
        private int fireIntervalMax;
        private int tickCounter;
        private readonly double[] rowHeights = { 320, 260, 180, 100 };
        private readonly Dictionary<int, bool> rowMovingRight = new Dictionary<int, bool>();
        private readonly Dictionary<int, IMovementPattern[]> levelMovementPatterns = new Dictionary<int, IMovementPattern[]>
        {
            { 1, new IMovementPattern[] { new StraightPattern(), new StraightPattern(), new StraightPattern(), new StraightPattern() } },
            { 2, new IMovementPattern[] { new StraightPattern(), new StraightPattern(), new ZigZagPattern(), new ZigZagPattern() } },
            { 3, new IMovementPattern[] { new StraightPattern(), new OscillatingHorizontalPattern(), new ZigZagPattern(), new DiagonalPattern() } }
        };

        /// <summary>
        /// Handles game-ending events. This action notifies subscribers whether the game has ended in a win (true) or a loss (false).
        /// </summary>
        public static Action<bool> OnGameEnd;
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EnemyManager"/> class, which manages enemy ships' behavior,
        /// including their movement, firing patterns, and interactions with the player.
        /// </summary>
        /// <param name="canvas">The canvas where all game objects are rendered.</param>
        /// <param name="bulletManager">Manages the creation and control of bullets fired by enemies and the player.</param>
        /// <param name="uiTextManager">Manages UI elements, such as score and game-over messages.</param>
        /// <param name="playerManager">Handles player-specific logic, such as position, movement, and collision detection.</param>
        /// <param name="gameManager">Controls the overall flow and logic of the game, including starting and ending the game.</param>
        public EnemyManager(Canvas canvas, BulletManager bulletManager, UiTextManager uiTextManager, PlayerManager playerManager, GameManager gameManager)
        {
            this.canvas = canvas;
            this.uiTextManager = uiTextManager;
            this.bulletManager = bulletManager;
            this.playerManager = playerManager;
            this.gameManager = gameManager;
            this.initializeTimers();
        }

        #endregion

        #region Initialization Methods

        private void initializeTimers()
        {
            this.initializeEnemyMovementTimer();
            this.initializeAttackTimer();
        }

        private void initializeEnemyMovementTimer()
        {
            this.enemyMovementTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
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

        /// <summary>
        /// Updates the level settings for the enemies, including the number of ships per row 
        /// and the interval range for enemy firing.
        /// </summary>
        /// <param name="shipsInRow">An array specifying the number of enemy ships in each row.</param>
        /// <param name="minFireInterval">The minimum interval (in milliseconds) between enemy ship attacks.</param>
        /// <param name="maxFireIntervalMax">The maximum interval (in milliseconds) between enemy ship attacks.</param>
        public void UpdateLevelSettings(int[] shipsInRow, int minFireInterval, int maxFireIntervalMax)
        {
            this.shipsPerRow = shipsInRow;
            this.fireIntervalMin = minFireInterval;
            this.fireIntervalMax = maxFireIntervalMax;
        }

        /// <summary>
        /// Initializes the enemy ships on the canvas based on the current level settings.
        /// This method calculates the positions of the ships and sets their starting locations and movement directions.
        /// </summary>
        public void InitializeEnemies()
        {
            this.ships = new List<EnemyShip>();
            this.originalShipPositions = new List<double>();
            for (var row = 0; row < this.shipsPerRow.Length; row++)
            {
                var shipCount = this.shipsPerRow[row];
                var rowY = this.rowHeights[row];
                var shipSpacing = (this.canvas.Width - shipCount) / (shipCount + 1);
                for (var i = 0; i < shipCount; i++)
                {
                    this.placeInitialShips(row, i, shipSpacing, rowY);
                }
                this.rowMovingRight[row] = true;
            }
        }

        private void placeInitialShips(int row, int shipIndex, double shipSpacing, double rowY)
        {
            var enemyLevel = row + 1;
            var enemyShip = this.createNewShip(enemyLevel);
            var shipX = shipSpacing + shipIndex * shipSpacing - enemyShip.Width / 2;

            enemyShip.BaseYPosition = rowY;
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

        #endregion

        #region Timers

        private void OnEnemyTimerTick(object sender, object e)
        {
            this.updateEnemiesMovement();
            this.checkCollision();
            this.updateFiring();
        }

        private void OnAttackTimerTick(object sender, object e)
        {
            var level4Ships = this.ships.Where(ship => ship.Level == 4 && !this.attackingShips.Contains(ship)).ToList();
            if (level4Ships.Any())
            {
                var attackingShip = level4Ships[this.random.Next(level4Ships.Count)];
                this.attackingShips.Add(attackingShip);
                this.startAttackPattern(attackingShip, this.playerManager.Players[0]);
            }
            this.attackTimer.Interval = TimeSpan.FromMilliseconds(this.random.Next(MinAttackInterval, MaxAttackInterval));
        }

        #endregion

        #region Movement

        private void updateEnemiesMovement()
        {
            if (!this.ships.Any() || this.ships.All(e => e == null))
            {
                this.gameManager.NextLevel();
                return;
            }
            for (var row = 0; row < this.shipsPerRow.Length; row++)
            {
                var rowShips = this.ships.Where(ship => ship.Level == row + 1 && !this.attackingShips.Contains(ship)).ToList();
                if (!rowShips.Any())
                {
                    continue;
                }
                this.updateRowDirection(row, rowShips);
                this.applyRowMovementPattern(row, rowShips);
            }
        }

        private void updateRowDirection(int row, List<EnemyShip> rowShips)
        {
            var rowMinX = rowShips.Min(ship => ship.X);
            var rowMaxX = rowShips.Max(ship => ship.X + ship.Width);
            if (this.rowMovingRight[row] && rowMaxX >= this.canvas.Width - MaxMoveDistance)
            {
                this.rowMovingRight[row] = false;
            }
            else if (!this.rowMovingRight[row] && rowMinX <= MaxMoveDistance)
            {
                this.rowMovingRight[row] = true;
            }
        }

        private void applyRowMovementPattern(int row, List<EnemyShip> rowShips)
        {
            var level = this.gameManager.CurrentGameLevel();
            if (!this.levelMovementPatterns.ContainsKey(level))
            {
                return;
            }
            var rowPatterns = this.levelMovementPatterns[level];
            if (row < rowPatterns.Length)
            {
                var currentPattern = rowPatterns[row];

                var rowMinX = rowShips.Min(ship => ship.X);
                var rowMaxX = rowShips.Max(ship => ship.X + ship.Width);

                if (this.rowMovingRight[row] && rowMaxX >= this.canvas.Width - MaxMoveDistance)
                {
                    this.rowMovingRight[row] = false;
                }
                else if (!this.rowMovingRight[row] && rowMinX <= MaxMoveDistance)
                {
                    this.rowMovingRight[row] = true;
                }
                currentPattern.ApplyMovement(rowShips, this.rowMovingRight[row], MovementSpeed, MaxMoveDistance, this.originalShipPositions, this.updateEnemyPosition, HandleSecondSprite);
            }
        }

        private void updateEnemyPosition(EnemyShip enemy)
        {
            enemy.X = Math.Max(0, Math.Min(this.canvas.Width - enemy.Width, enemy.X));
            Canvas.SetLeft(enemy.Sprite, enemy.X);
            Canvas.SetTop(enemy.Sprite, enemy.Y);

            if (enemy.HasSecondSprite)
            {
                Canvas.SetLeft(enemy.Sprite2, enemy.X);
                Canvas.SetTop(enemy.Sprite2, enemy.Y);
            }
        }

        /// <summary>
        /// will change between the first and second sprite outfit assigned to the ships
        /// </summary>
        /// <param name="enemy"></param>
        public static void HandleSecondSprite(EnemyShip enemy)
        {
            if (enemy.HasSecondSprite)
            {
                if (enemy.Sprite2.Visibility == Visibility.Visible)
                {
                    enemy.Sprite.Visibility = Visibility.Visible;
                    enemy.Sprite2.Visibility = Visibility.Collapsed;
                }
                else
                {
                    enemy.Sprite2.Visibility = Visibility.Visible;
                    enemy.Sprite.Visibility = Visibility.Collapsed;
                }
                Canvas.SetLeft(enemy.Sprite2, enemy.X);
                Canvas.SetTop(enemy.Sprite2, enemy.Y);
            }
        }


        #endregion

        #region Attack

        private void startAttackPattern(EnemyShip attackingShip, Player player)
        {
            if (this.uiTextManager.GameOver || attackingShip == null || player == null)
            {
                return;
            }
            if (!this.attackingShips.Contains(attackingShip))
            {
                this.attackingShips.Add(attackingShip);
            }
            var attackTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(16) };
            var speedX = (player.X - attackingShip.X) / this.canvas.Height * AttackSpeed;
            var speedY = (player.Y - attackingShip.Y) / this.canvas.Height * AttackSpeed;
            attackTimer.Tick += (s, e) =>
            {
                attackingShip.X += speedX;
                attackingShip.Y += speedY;
                this.updateEnemyPosition(attackingShip);
                HandleSecondSprite(attackingShip);
                if (this.random.Next(0, AttackChanceThreshold) < BulletFireChance && this.ships.Contains(attackingShip) && attackingShip.Y <= this.canvas.Height - Offset)
                {
                    this.bulletManager.FireEnemyBullet(attackingShip.X + attackingShip.Width / 2, attackingShip.Y + attackingShip.Height, player.X, player.Y, true);
                }
                if (attackingShip.X < 0 || attackingShip.X > this.canvas.Width || attackingShip.Y > this.canvas.Height + Offset)
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
            }
            else
            {
                attackingShip.X = 0;
                attackingShip.Y = this.rowHeights[0];
            }
            var newX = attackingShip.X;
            bool isOverlapping;
            do
            {
                isOverlapping = false;
                foreach (var otherShip in this.ships)
                {
                    if (otherShip != attackingShip && otherShip.Level == attackingShip.Level && Math.Abs(otherShip.X - newX) < attackingShip.Width)
                    {
                        isOverlapping = true;
                        newX += attackingShip.Width;
                        break;
                    }
                }
            } while (isOverlapping);
            attackingShip.X = newX;
            Canvas.SetLeft(attackingShip.Sprite, attackingShip.X);
            Canvas.SetTop(attackingShip.Sprite, attackingShip.Y);
        }



        #endregion

        #region Firing

        private void updateFiring()
        {
            this.tickCounter++;
            if (this.ships.Any() && this.tickCounter >= this.fireIntervalMin)
            {
                var randomFireTick = this.random.Next(this.fireIntervalMin, this.fireIntervalMax + 1);
                if (this.tickCounter % randomFireTick == 0)
                {
                    var randomIndex = this.random.Next(this.ships.Count);
                    this.fireEnemyBullet(this.ships[randomIndex], this.playerManager.Players[0]);
                }
            }
        }

        private void fireEnemyBullet(EnemyShip enemy, Player player)
        {
            if (enemy != null && enemy.IsShooter && !this.uiTextManager.GameOver)
            {
                this.bulletManager.FireEnemyBullet(enemy.X + enemy.Width / 2, enemy.Y + enemy.Height, player.X + player.Width / 2, player.Y + player.Height / 2, enemy.Level == 4);
            }
        }

        #endregion

        #region Collision

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

        #endregion
    }
}