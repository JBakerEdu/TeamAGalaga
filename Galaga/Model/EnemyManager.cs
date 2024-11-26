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

        private const double MovementSpeed = 0.2;
        private const double MaxMoveDistance = 10;
        private readonly Canvas canvas;
        private readonly double canvasWidth;
        private int tickCounter;
        private const int FireIntervalMin = 5;
        private readonly int[] shipsPerRow = { 3, 4, 4, 5 };
        private readonly double[] rowHeights = { 260, 200, 120, 40 };
        private const int FireIntervalMax = 25;
        private readonly Random random = new Random();
        private IList<EnemyShip> ships;
        private IList<double> originalShipPositions;
        private bool movingRight = true;
        private DispatcherTimer enemyMovementTimer;
        private readonly BulletManager bulletManager;
        private readonly UiTextManager uiTextManager;

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
            this.InitializeEnemies();
            this.uiTextManager = uiTextManager;
            this.bulletManager = bulletManager;
            this.InitializeTimer();
        }

        #endregion

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
                    this.FireEnemyBullet(enemy);
                }
            }
        }

        private void InitializeEnemies()
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
                this.uiTextManager.EndGame(true);
                return;
            }

            var firstEnemy = this.ships.FirstOrDefault(e => e != null);
            if (firstEnemy == null)
            {
                this.uiTextManager.EndGame(true);
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
        /// this is how the enemy fires their fireball
        /// </summary>
        /// <param name="enemy">which enemy is firing</param>
        private void FireEnemyBullet(EnemyShip enemy)
        {
            if (enemy != null && enemy.IsShooter && !this.uiTextManager.GameOver)
            {
                double renderX = enemy.X + enemy.Width / 2;
                double renderY = enemy.Y + enemy.Height;
                this.bulletManager.FireEnemyBullet(renderX, renderY);
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
                    this.uiTextManager.UpdateScore(enemy);
                }
            }
        }





















        ///// <summary>
        ///// Check if a bullet passed in has collided with any enemy ships
        ///// </summary>
        ///// <param name="playerBullet"> the bullet being checked if it hits an enemy ship</param>
        ///// <returns> bool if the bullet has hit a target that way the bullet manager knows to also remove the bullet</returns>
        //public bool CheckCollision(Bullet playerBullet)
        //{
        //    if (playerBullet == null)
        //    {
        //        return false;
        //    }

        //    for (var i = this.ships.Count - 1; i >= 0; i--)
        //    {
        //        var enemy = this.ships[i];

        //        if (this.IsCollision(playerBullet, enemy))
        //        {
        //            this.DestroyEnemy(enemy);
        //            this.uiTextManager.UpdateScore(enemy);
        //            return true;
        //        }
        //    }

        //    return false;
        //}

        //private bool IsCollision(Bullet bullet, GameObject ship)
        //{
        //    var bulletLeft = bullet.X;
        //    var bulletRight = bullet.X + bullet.Width;
        //    var bulletTop = bullet.Y;
        //    var bulletBottom = bullet.Y + bullet.Height;

        //    var enemyLeft = ship.X;
        //    var enemyWidth = ship.Width;
        //    var enemyHeight = ship.Height;

        //    var enemyRight = ship.X + enemyWidth;
        //    var enemyTop = ship.Y;
        //    var enemyBottom = ship.Y + enemyHeight;
        //    if (bulletBottom >= enemyTop && bulletTop <= enemyBottom)
        //    {
        //        return bulletRight > enemyLeft && bulletLeft < enemyRight;
        //    }
        //    return false;
        //}
    }
}