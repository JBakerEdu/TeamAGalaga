using System.Collections.Generic;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Galaga.View.Sprites;
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
        private const double MaxMoveDistance = 10;
        private const double AttackSpeed = 2.5;
        private const int MinAttackInterval = 5000;
        private const int MaxAttackInterval = 15000;

        #endregion

        #region Fields

        private readonly Canvas canvas;
        private readonly double canvasWidth;
        private readonly Random random = new Random();
        private readonly Random randomAttack = new Random();
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
        private int FireIntervalMin;
        private int FireIntervalMax;
        private int tickCounter;
        private readonly double[] rowHeights = { 320, 260, 180, 100 };


        private readonly Dictionary<int, bool> rowMovingRight = new Dictionary<int, bool>();
        private readonly Dictionary<int, IMovementPattern[]> levelMovementPatterns = new Dictionary<int, IMovementPattern[]>
        {
            { 1, new IMovementPattern[] { new StraightPattern(), new StraightPattern(), new StraightPattern(), new StraightPattern() } },
            { 2, new IMovementPattern[] { new StraightPattern(), new StraightPattern(), new ZigZagPattern(), new ZigZagPattern() } },
            { 3, new IMovementPattern[] { new StraightPattern(), new OscillatingHorizontalPattern(), new ZigZagPattern(), new DiagonalPattern() } },
        };

        public static Action<bool> OnGameEnd;

        #endregion

        #region Constructors

        public EnemyManager(Canvas canvas, BulletManager bulletManager, UiTextManager uiTextManager, PlayerManager playerManager, GameManager gameManager)
        {
            this.canvas = canvas;
            this.canvasWidth = canvas.Width;
            this.uiTextManager = uiTextManager;
            this.bulletManager = bulletManager;
            this.playerManager = playerManager;
            this.gameManager = gameManager;
            InitializeTimers();
        }

        #endregion

        #region Initialization Methods

        private void InitializeTimers()
        {
            InitializeEnemyMovementTimer();
            InitializeAttackTimer();
        }

        private void InitializeEnemyMovementTimer()
        {
            enemyMovementTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
            enemyMovementTimer.Tick += OnEnemyTimerTick;
            enemyMovementTimer.Start();
        }

        private void InitializeAttackTimer()
        {
            attackTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(random.Next(MinAttackInterval, MaxAttackInterval))
            };
            attackTimer.Tick += OnAttackTimerTick;
            attackTimer.Start();
        }

        public void UpdateLevelSettings(int[] shipsPerRow, int fireIntervalMin, int fireIntervalMax)
        {
            this.shipsPerRow = shipsPerRow;
            this.FireIntervalMin = fireIntervalMin;
            this.FireIntervalMax = fireIntervalMax;
        }

        public void InitializeEnemies()
        {
            ships = new List<EnemyShip>();
            originalShipPositions = new List<double>();
            for (var row = 0; row < shipsPerRow.Length; row++)
            {
                var shipCount = shipsPerRow[row];
                var rowY = rowHeights[row];
                var shipSpacing = (canvasWidth - shipCount) / (shipCount + 1);
                for (var i = 0; i < shipCount; i++)
                {
                    PlaceInitialShips(row, i, shipSpacing, rowY);
                }
                rowMovingRight[row] = true;
            }
        }

        private void PlaceInitialShips(int row, int shipIndex, double shipSpacing, double rowY)
        {
            var enemyLevel = row + 1;
            var enemyShip = CreateNewShip(enemyLevel);
            var shipX = shipSpacing + shipIndex * shipSpacing - enemyShip.Width / 2;

            enemyShip.BaseYPosition = rowY;
            enemyShip.RenderAt(shipX, rowY);

            canvas.Children.Add(enemyShip.Sprite);
            if (enemyShip.HasSecondSprite)
            {
                canvas.Children.Add(enemyShip.Sprite2);
            }

            ships.Add(enemyShip);
            originalShipPositions.Add(shipX);
        }

        private EnemyShip CreateNewShip(int enemyLevel)
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
            UpdateEnemiesMovement();
            CheckCollision();
            UpdateFiring();
        }

        private void OnAttackTimerTick(object sender, object e)
        {
            var level4Ships = ships.Where(ship => ship.Level == 4 && !attackingShips.Contains(ship)).ToList();
            if (level4Ships.Any())
            {
                var attackingShip = level4Ships[random.Next(level4Ships.Count)];
                attackingShips.Add(attackingShip);
                StartAttackPattern(attackingShip, playerManager.Players[0]);
            }
            attackTimer.Interval = TimeSpan.FromMilliseconds(random.Next(MinAttackInterval, MaxAttackInterval));
        }

        #endregion

        #region Movement

        private void UpdateEnemiesMovement()
        {
            if (!ships.Any() || ships.All(e => e == null))
            {
                gameManager.NextLevel();
                return;
            }

            for (int row = 0; row < shipsPerRow.Length; row++)
            {
                var rowShips = ships.Where(ship => ship.Level == row + 1 && !attackingShips.Contains(ship)).ToList();
                if (!rowShips.Any()) continue;

                UpdateRowDirection(row, rowShips);
                ApplyRowMovementPattern(row, rowShips);
            }
        }

        private void UpdateRowDirection(int row, List<EnemyShip> rowShips)
        {
            var rowMinX = rowShips.Min(ship => ship.X);
            var rowMaxX = rowShips.Max(ship => ship.X + ship.Width);

            if (rowMovingRight[row] && rowMaxX >= canvasWidth - MaxMoveDistance)
                rowMovingRight[row] = false;
            else if (!rowMovingRight[row] && rowMinX <= MaxMoveDistance)
                rowMovingRight[row] = true;
        }

        private void ApplyRowMovementPattern(int row, List<EnemyShip> rowShips)
        {
            var level = gameManager.CurrentGameLevel();
            if (!levelMovementPatterns.ContainsKey(level)) return;

            var rowPatterns = levelMovementPatterns[level];
            if (row < rowPatterns.Length)
            {
                var currentPattern = rowPatterns[row];

                // Determine boundaries for the row
                var rowMinX = rowShips.Min(ship => ship.X);
                var rowMaxX = rowShips.Max(ship => ship.X + ship.Width);

                // Reverse direction at screen edges
                if (rowMovingRight[row] && rowMaxX >= canvasWidth - MaxMoveDistance)
                {
                    rowMovingRight[row] = false; // Move left
                }
                else if (!rowMovingRight[row] && rowMinX <= MaxMoveDistance)
                {
                    rowMovingRight[row] = true; // Move right
                }

                // Apply the movement pattern based on the current direction
                currentPattern.ApplyMovement(
                    rowShips,
                    rowMovingRight[row], // Use global direction state for this row
                    MovementSpeed,
                    MaxMoveDistance,
                    originalShipPositions,
                    UpdateEnemyPosition,
                    HandleSecondSprite);
            }
        }



        private void UpdateEnemyPosition(EnemyShip enemy)
        {
            enemy.X = Math.Max(0, Math.Min(canvasWidth - enemy.Width, enemy.X));
            Canvas.SetLeft(enemy.Sprite, enemy.X);
            Canvas.SetTop(enemy.Sprite, enemy.Y);

            if (enemy.HasSecondSprite)
            {
                Canvas.SetLeft(enemy.Sprite2, enemy.X);
                Canvas.SetTop(enemy.Sprite2, enemy.Y);
            }
        }


        private void HandleSecondSprite(EnemyShip enemy)
        {
            if (enemy.HasSecondSprite)
            {
                if (enemy.Sprite2.Visibility == Visibility.Visible)
                {
                    enemy.Sprite2.Visibility = Visibility.Collapsed;
                }
                else
                {
                    enemy.Sprite2.Visibility = Visibility.Visible;
                }

                // Ensure second sprite moves with the main sprite
                Canvas.SetLeft(enemy.Sprite2, enemy.X);
                Canvas.SetTop(enemy.Sprite2, enemy.Y);
            }
        }


        #endregion

        #region Attack

        private void StartAttackPattern(EnemyShip attackingShip, Player player)
        {
            if (uiTextManager.GameOver || attackingShip == null || player == null) return;

            if (!attackingShips.Contains(attackingShip))
            {
                attackingShips.Add(attackingShip);
            }

            var attackTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(16) };
            var speedX = (player.X - attackingShip.X) / 500.0 * AttackSpeed;
            var speedY = (player.Y - attackingShip.Y) / 500.0 * AttackSpeed;

            attackTimer.Tick += (s, e) =>
            {
                attackingShip.X += speedX;
                attackingShip.Y += speedY;

                UpdateEnemyPosition(attackingShip);

                // Randomly fire bullets during the attack
                if (random.Next(1000) < 3)
                {
                    bulletManager.FireEnemyBullet(
                        attackingShip.X + attackingShip.Width / 2,
                        attackingShip.Y + attackingShip.Height,
                        player.X,
                        player.Y,
                        true);
                }

                // End the attack when the ship reaches the player
                if (Math.Abs(attackingShip.X - player.X) < 5 && Math.Abs(attackingShip.Y - player.Y) < 5)
                {
                    attackTimer.Stop();
                    attackingShips.Remove(attackingShip);
                    ResetEnemyPosition(attackingShip);
                }
            };

            attackTimer.Start();
        }


        private void ResetEnemyPosition(EnemyShip attackingShip)
        {
            var index = ships.IndexOf(attackingShip);
            if (index >= 0 && index < originalShipPositions.Count)
            {
                attackingShip.X = originalShipPositions[index];
                attackingShip.Y = rowHeights[attackingShip.Level - 1];
                UpdateEnemyPosition(attackingShip);
            }
        }

        #endregion

        #region Firing

        private void UpdateFiring()
        {
            tickCounter++;
            if (ships.Any() && tickCounter >= FireIntervalMin)
            {
                var randomFireTick = random.Next(FireIntervalMin, FireIntervalMax + 1);
                if (tickCounter % randomFireTick == 0)
                {
                    var randomIndex = random.Next(ships.Count);
                    FireEnemyBullet(ships[randomIndex], playerManager.Players[0]);
                }
            }
        }

        private void FireEnemyBullet(EnemyShip enemy, Player player)
        {
            if (enemy != null && enemy.IsShooter && !uiTextManager.GameOver)
            {
                bulletManager.FireEnemyBullet(
                    enemy.X + enemy.Width / 2,
                    enemy.Y + enemy.Height,
                    player.X + player.Width / 2,
                    player.Y + player.Height / 2,
                    enemy.Level == 4);
            }
        }

        #endregion

        #region Collision

        private void CheckCollision()
        {
            foreach (var enemy in this.ships.ToList())
            {
                if (enemy != null && this.bulletManager.CheckSpriteCollision(enemy, false))
                {
                    this.DestroyEnemy(enemy);
                    AudioManager.PlayEnemyBlowUp(this.gameManager.GameType);
                    this.uiTextManager.UpdateScore(enemy);
                }
            }
        }

        private void DestroyEnemy(EnemyShip enemy)
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