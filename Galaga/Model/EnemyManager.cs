using System.Collections.Generic;
using System;
using System.Diagnostics;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Galaga.View.Sprites;
using System.Linq;
using System.Reflection;

namespace Galaga.Model
{
    public class EnemyManager
    {
        #region Data members

        private const double EnemyMovementSpeed = 0.2;
        private const double EnemyMaxMoveDistance = 10;
        private readonly Canvas canvas;
        private readonly double canvasHeight;
        private readonly double canvasWidth;
        private int tickCounter;
        private const int FireIntervalMin = 30;
        private readonly int[] shipsPerRow = { 3, 4, 4, 5};
        private readonly double[] rowHeights = {260, 200, 120, 40 };
        private const int FireIntervalMax = 100;
        private readonly Random random = new Random();
        private List<EnemyShip> enemyShips;
        private List<double> originalEnemyPositions;
        private bool movingRight = true;
        private DispatcherTimer enemyMovementTimer;
        private BulletManager bulletManager;
        private UITextManager uiTextManager;
        private const int ShipSpeedX = 1;
        private const int ShipSpeedY = 0;
        private const int ShootingMin = 3;


        private const int VisibleTicks = 60; // Number of ticks to be visible
        private const int InvisibleTicks = 60; // Number of ticks to be invisible
        private int visibilityTickCounter = 0; // Counter to track visibility ticks











































        private bool test = false;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates the game manager class and init the game
        /// </summary>
        /// <param name="canvas">the canvas passes in</param>
        /// <exception cref="ArgumentNullException"></exception>
        public EnemyManager(Canvas canvas, BulletManager bulletManager, UITextManager uiTextManager)
        {
            this.canvas = canvas;
            this.canvasHeight = canvas.Height;
            this.canvasWidth = canvas.Width;
            this.createAndPlaceEnemies();
            this.uiTextManager = uiTextManager;
            this.bulletManager = bulletManager;
            this.initializeTimer();
        }

        #endregion

        private void initializeTimer()
        {
            this.enemyMovementTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(16)
            };
            this.enemyMovementTimer.Tick += this.OnEnemyTimerTick;
            this.enemyMovementTimer.Start();
        }

        private void OnEnemyTimerTick(object sender, object e)
        {
            this.UpdateEnemies();
            this.visibilityTickCounter++;
            foreach (var enemy in this.enemyShips)
            {
                if (enemy != null && enemy.Level >= 3)
                {
                    if (visibilityTickCounter < VisibleTicks)
                    {
                        enemy.Sprite.Visibility = Visibility.Visible;
                        enemy.Sprite2.Visibility = Visibility.Collapsed;
                    }
                    else if (visibilityTickCounter < VisibleTicks + InvisibleTicks)
                    {
                        enemy.Sprite2.Visibility = Visibility.Visible;
                        enemy.Sprite.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        visibilityTickCounter = 0;
                    }
                }
            }

            this.tickCounter++;

            if (this.enemyShips.Count > 0 && this.tickCounter >= FireIntervalMin)
            {
                var randomFireTick = this.random.Next(FireIntervalMin, FireIntervalMax + 1);

                if (this.tickCounter % randomFireTick == 0)
                {
                    int randomIndex = this.random.Next(this.enemyShips.Count);
                    var enemy = this.enemyShips[randomIndex];
                    this.FireEnemyBullet(enemy);
                }
            }
        }

        private void createAndPlaceEnemies()
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
                    EnemyShip enemyShip;
                    var enemyLevel = row + 1;
                    bool  meetsShootingMin = enemyLevel >= ShootingMin;
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
                    var shipX = shipSpacing + i * (shipSpacing);
                    enemyShip.RenderAt(shipX, rowY);
                    this.canvas.Children.Add(enemyShip.Sprite);
                    if (enemyShip.hasSecondSprite)
                    {
                        this.canvas.Children.Add(enemyShip.Sprite2);
                    }
                    this.enemyShips.Add(enemyShip);
                    this.originalEnemyPositions.Add(shipX);
                }
            }
        }


        /// <summary>
        /// this updates how the enemy move around
        /// </summary>
        private void UpdateEnemies()
        {
            if (this.enemyShips.Count == 0 || this.enemyShips.All(e => e == null))
            {
                this.uiTextManager.endGame(true);
                return;
            }

            var firstEnemy = this.enemyShips.FirstOrDefault(e => e != null);
            if (firstEnemy == null)
            {
                this.uiTextManager.endGame(true);
                return;
            }

            var moveAmount = this.movingRight ? EnemyMovementSpeed : -EnemyMovementSpeed;
            var currentDistanceFromStart = Math.Abs(firstEnemy.X - this.originalEnemyPositions[this.enemyShips.IndexOf(firstEnemy)]);
            if (currentDistanceFromStart + Math.Abs(moveAmount) > EnemyMaxMoveDistance)
            {
                this.movingRight = !this.movingRight;
                moveAmount = this.movingRight ? EnemyMovementSpeed : -EnemyMovementSpeed;
            }

            foreach (var enemy in this.enemyShips)
            {
                if (enemy != null)
                {
                    enemy.Move(moveAmount);
                    this.updateEnemyPosition(enemy);
                }
            }
        }

        private void updateEnemyPosition(EnemyShip enemy)
        {
            Canvas.SetLeft(enemy.Sprite, enemy.X);
            Canvas.SetTop(enemy.Sprite, enemy.Y);
            if (enemy.hasSecondSprite)
            {
                Canvas.SetLeft(enemy.Sprite2, enemy.X);
                Canvas.SetTop(enemy.Sprite2, enemy.Y);
            }
        }

        /// <summary>
        /// this is how the enemy fires their fireball
        /// </summary>
        /// <param name="enemy">which enemy is firing</param>
        private void FireEnemyBullet(EnemyShip enemy)
        {
            if (enemy != null && enemy.isShooter && !this.uiTextManager.gameOver)
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
                if (enemy.hasSecondSprite)
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

        public bool checkCollision(Bullet playerBullet)
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
                        this.uiTextManager.updateScore(enemy);
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