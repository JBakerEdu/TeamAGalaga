using System.Collections.Generic;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Galaga.View.Sprites;
using System.Linq;

namespace Galaga.Model
{
    /// <summary>
    /// This is the manager of all enemy ships
    /// </summary>
    public class EnemyManager
    {
        #region Data members

        private const double EnemyMovementSpeed = 5;
        private const double EnemyMaxMoveDistance = 30;
        private readonly Canvas canvas;
        private readonly double canvasWidth;
        private int tickCounter;
        private const int FireIntervalMin = 1;
        private readonly int[] shipsPerRow = { 3, 4, 4, 5};
        private readonly double[] rowHeights = {260, 200, 120, 40 };
        private const int FireIntervalMax = 1;
        private readonly Random random = new Random();
        private List<EnemyShip> enemyShips;
        private List<double> originalEnemyPositions;
        private bool movingRight = true;
        private DispatcherTimer enemyMovementTimer;
        private readonly BulletManager bulletManager;
        private readonly UiTextManager uiTextManager;
        private const int ShipSpeedX = 1;
        private const int ShipSpeedY = 0;
        private const int ShootingMin = 3;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates the game manager class and init the game
        /// </summary>
        /// <param name="canvas">the canvas to work on</param>
        /// <param name="bulletManager">the bullet manager so it can talk about if a bullet hits an enemy </param>
        /// <param name="uiTextManager"> the UiTextManger so it can call when games ends and to add a score of the enemies when hit</param>
        public EnemyManager(Canvas canvas, BulletManager bulletManager, UiTextManager uiTextManager)
        {
            this.canvas = canvas;
            this.canvasWidth = canvas.Width;
            this.initializeEnemies();
            this.uiTextManager = uiTextManager;
            this.bulletManager = bulletManager;
            this.initializeTimer();
        }

        #endregion

        private void initializeTimer()
        {
            this.enemyMovementTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(1000)
            };
            this.enemyMovementTimer.Tick += this.OnEnemyTimerTick;
            this.enemyMovementTimer.Start();
        }

        private void OnEnemyTimerTick(object sender, object e)
        {
            this.updateEnemiesMovement();
            this.updateFiring();
        }

        private void updateFiring()
        {
            this.tickCounter++;
            if (this.enemyShips.Count > 0 && this.tickCounter >= FireIntervalMin)
            {
                var randomFireTick = this.random.Next(FireIntervalMin, FireIntervalMax + 1);

                if (this.tickCounter % randomFireTick == 0)
                {
                    int randomIndex = this.random.Next(this.enemyShips.Count);
                    var enemy = this.enemyShips[randomIndex];
                    this.fireEnemyBullet(enemy);
                }
            }
        }

        private void initializeEnemies()
        {
            this.enemyShips = new List<EnemyShip>();
            this.originalEnemyPositions = new List<double>();
            for (var row = 0; row < this.shipsPerRow.Length; row++)
            {
                var shipCount = this.shipsPerRow[row];
                var rowY = this.rowHeights[row];
                var shipSpacing = (this.canvasWidth - shipCount) / (shipCount + 1);
                for (var i = 0; i < shipCount; i++)
                {
                    this.placeShips(row, i, shipSpacing, rowY);
                }
            }
        }

        private void placeShips(int row, int shipIndex, double shipSpacing, double rowY)
        {
            var enemyLevel = row + 1;
            bool meetsShootingMin = enemyLevel >= ShootingMin;
            EnemyShip enemyShip = this.createNewShip(enemyLevel, meetsShootingMin);
            var shipX = shipSpacing + shipIndex * shipSpacing - enemyShip.Width / 2 ;
            enemyShip.RenderAt(shipX, rowY);
            this.canvas.Children.Add(enemyShip.Sprite);
            if (enemyShip.HasSecondSprite)
            {
                this.canvas.Children.Add(enemyShip.Sprite2);
            }
            this.enemyShips.Add(enemyShip);
            this.originalEnemyPositions.Add(shipX);
        }


        private EnemyShip createNewShip(int enemyLevel, bool meetsShootingMin)
        {
            EnemyShip enemyShip;
            switch (enemyLevel)
            {
                case 1:
                    enemyShip = new EnemyShip(new EnemyShipLevel1Sprite(), ShipSpeedX, ShipSpeedY, enemyLevel, meetsShootingMin);
                    break;
                case 2:
                    enemyShip = new EnemyShip(new EnemyShipLevel2Sprite(), ShipSpeedX, ShipSpeedY, enemyLevel, meetsShootingMin);
                    break;
                case 3:
                    enemyShip = new EnemyShip(new EnemyShipLevel3Sprite(), ShipSpeedX, ShipSpeedY, enemyLevel, meetsShootingMin);
                    break;
                case 4:
                    enemyShip = new EnemyShip(new EnemyShipLevel4Sprite(), ShipSpeedX, ShipSpeedY, enemyLevel, meetsShootingMin);
                    break;
                default:
                    enemyShip = new EnemyShip(new EnemyShipLevel1Sprite(), ShipSpeedX, ShipSpeedY, enemyLevel, meetsShootingMin);
                    break;
            }
            return enemyShip;
        }

        /// <summary>
        /// this updates how the enemy move around
        /// </summary>
        private void updateEnemiesMovement()
        {
            if (this.enemyShips.Count == 0 || this.enemyShips.All(e => e == null))
            {
                this.uiTextManager.EndGame(true);
                return;
            }
            var firstEnemy = this.enemyShips.FirstOrDefault(e => e != null);
            if (firstEnemy == null)
            {
                this.uiTextManager.EndGame(true);
                return;
            }

            // Determine movement direction
            var moveAmount = this.movingRight ? EnemyMovementSpeed : -EnemyMovementSpeed;
            var currentDistanceFromStart = Math.Abs(firstEnemy.X - this.originalEnemyPositions[this.enemyShips.IndexOf(firstEnemy)]);
            if (currentDistanceFromStart + Math.Abs(moveAmount) > EnemyMaxMoveDistance)
            {
                this.movingRight = !this.movingRight;
                moveAmount = this.movingRight ? EnemyMovementSpeed : -EnemyMovementSpeed;
            }

            // Move and switch sprites for each enemy
            foreach (var enemy in this.enemyShips)
            {
                if (enemy != null)
                {
                    enemy.Move(moveAmount);
                    this.updateEnemyPosition(enemy);

                    // Switch sprites on each movement
                    if (enemy.HasSecondSprite)
                    {
                        if (enemy.Sprite.Visibility == Visibility.Visible)
                        {
                            enemy.Sprite.Visibility = Visibility.Collapsed;
                            enemy.Sprite2.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            enemy.Sprite.Visibility = Visibility.Visible;
                            enemy.Sprite2.Visibility = Visibility.Collapsed;
                        }
                    }
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
        /// this is how the enemy fires their fireball
        /// </summary>
        /// <param name="enemy">which enemy is firing</param>
        private void fireEnemyBullet(EnemyShip enemy)
        {
            if (enemy != null && enemy.IsShooter && !this.uiTextManager.GameOver)
            {
                double renderX = enemy.X + enemy.Width / 2;
                double renderY = enemy.Y + enemy.Height;
                this.bulletManager.FireEnemyBullet(renderX, renderY);
            }
        }

        private void destroyEnemy(EnemyShip enemy)
        {
            int index = this.enemyShips.IndexOf(enemy);
            if (index >= 0)
            {
                this.canvas.Children.Remove(enemy.Sprite);
                if (enemy.HasSecondSprite)
                {
                    this.canvas.Children.Remove(enemy.Sprite2);
                }
                this.enemyShips.RemoveAt(index);
                this.originalEnemyPositions.RemoveAt(index);
            }
            var remainingEnemies = new List<EnemyShip>();
            foreach (var ship in this.enemyShips)
            {
                if (ship != null)
                {
                    remainingEnemies.Add(ship);
                }
            }
            this.enemyShips = remainingEnemies;
        }

        /// <summary>
        /// Check if a bullet passed in has collided with any enemy ships
        /// </summary>
        /// <param name="playerBullet"> the bullet being checked if it hits an enemy ship</param>
        /// <returns> bool if the bullet has hit a target that way the bullet manager knows to also remove the bullet</returns>
        public bool CheckCollision(Bullet playerBullet)
        {
            bool isHit = false;
            if (playerBullet != null)
            {
                for (var i = this.enemyShips.Count - 1; i >= 0; i--)
                {
                    var enemy = this.enemyShips[i];
                    isHit = this.isCollision(playerBullet, enemy);
                    if (this.isCollision(playerBullet, enemy))
                    {
                        this.destroyEnemy(enemy);
                        this.uiTextManager.UpdateScore(enemy);
                        break;
                    }
                }
            }
            return isHit;
        }

        private bool isCollision(Bullet bullet, GameObject ship)
        {
            var bulletLeft = bullet.X;
            var bulletRight = bullet.X + bullet.Width;
            var bulletTop = bullet.Y;
            var bulletBottom = bullet.Y + bullet.Height;

            var enemyLeft = ship.X;
            var enemyWidth = ship.Width;
            var enemyHeight = ship.Height;

            var enemyRight = ship.X + enemyWidth;
            var enemyTop = ship.Y;
            var enemyBottom = ship.Y + enemyHeight;
            if (bulletBottom >= enemyTop && bulletTop <= enemyBottom)
            {
                return bulletRight > enemyLeft && bulletLeft < enemyRight;
            }
            return false;
        }
    }
}