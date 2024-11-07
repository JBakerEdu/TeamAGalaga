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

        private const double PlayerOffsetFromBottom = 30;
        private const double PlayerMovementSpeed = 3;
        private const double EnemyMovementSpeed = 0.2;
        private const double EnemyMaxMoveDistance = 10;
        private const int MaxBulletsAllowed = 1;
        private const double BulletSpeed = 5;
        private readonly Canvas canvas;
        private readonly double canvasHeight;
        private readonly double canvasWidth;
        private int tickCounter;
        private const int FireIntervalMin = 30;
        private readonly int[] shipsPerRow = { 2, 3, 4 };
        private readonly double[] rowHeights = { 200, 120, 40 };
        private const int FireIntervalMax = 100;
        private readonly Random random = new Random();
        private readonly List<Bullet> activePlayerBullets;
        private Player player;
        private List<EnemyShip> enemyShips;
        private List<double> originalEnemyPositions;
        private bool movingRight = true;
        private DispatcherTimer enemyMovementTimer;
        private DispatcherTimer bulletMovementTimer;
        private int score;
        private TextBlock scoreTextBlock;
        private TextBlock gameOverTextBlock;
        private readonly List<Bullet> activeEnemyBullets;
        private bool gameOver;

        /// <summary>
        /// calls objects moveLeft
        /// </summary>
        public void MovePlayerLeft() => this.player.MoveLeft();
        /// <summary>
        /// calls objects moveRight
        /// </summary>
        public void MovePlayerRight() => this.player.MoveRight(this.canvasWidth);

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
            this.canvasHeight = canvas.Height;
            this.canvasWidth = canvas.Width;
            this.activePlayerBullets = new List<Bullet>();
            this.activeEnemyBullets = new List<Bullet>();
            this.initializeGame();
        }

        #endregion

        #region Methods

        private void initializeGame()
        {
            this.createAndPlacePlayer();
            this.createAndPlaceEnemies();
            this.initializeScoreGame();
            this.initializeTimer();
            this.initializeBulletTimer();
        }

        private void initializeScoreGame()
        {
            this.scoreTextBlock = new TextBlock
            {
                Text = "Score: 0",
                FontSize = 24,
                Margin = new Thickness(10, 10, 0, 0)
            };
            this.canvas.Children.Add(this.scoreTextBlock);


        }

        private void initializeTimer()
        {
            this.enemyMovementTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(16)
            };
            this.enemyMovementTimer.Tick += this.OnEnemyTimerTick;
            this.enemyMovementTimer.Start();
        }

        private void initializeBulletTimer()
        {
            this.bulletMovementTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(16)
            };
            this.bulletMovementTimer.Tick += this.OnBulletTimerTick;
            this.bulletMovementTimer.Start();
        }



        private void OnEnemyTimerTick(object sender, object e)
        {
            this.UpdateEnemies();
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

        private void OnBulletTimerTick(object sender, object e)
        {
            this.updateBullets();
            this.updateEnemyBullets();
        }


        private void createAndPlacePlayer()
        {
            this.player = new Player();
            this.canvas.Children.Add(this.player.Sprite);
            this.placePlayerNearBottom();
        }

        private void placePlayerNearBottom()
        {
            this.player.X = this.canvasWidth / 2 - this.player.Width / 2.0;
            this.player.Y = this.canvasHeight - this.player.Height - PlayerOffsetFromBottom;
            this.updatePlayerPosition();
        }

        private void updatePlayerPosition()
        {
            Canvas.SetLeft(this.player.Sprite, this.player.X);
            Canvas.SetTop(this.player.Sprite, this.player.Y);
        }

        private void createAndPlaceEnemies()
        {
            this.enemyShips = new List<EnemyShip>();
            this.originalEnemyPositions = new List<double>();

            for (var row = 0; row < this.shipsPerRow.Length; row++)
            {
                var shipCount = this.shipsPerRow[row];
                var rowY = this.rowHeights[row];
                var shipSpacing = (this.canvasWidth - shipCount * 50) / (shipCount + 1);

                for (var i = 0; i < shipCount; i++)
                {
                    EnemyShip enemySprite;

                    var enemyLevel = row + 1;
                    switch (enemyLevel)
                    {
                        case 1:
                            enemySprite = new EnemyShip(new EnemyShipLevel1Sprite(), 1);
                            break;
                        case 2:
                            enemySprite = new EnemyShip(new EnemyShipLevel2Sprite(), 1);
                            break;
                        case 3:
                            enemySprite = new EnemyShip(new EnemyShipLevel3Sprite(), 1);
                            break;
                        default:
                            enemySprite = new EnemyShip(new EnemyShipLevel1Sprite(), 1);
                            break;
                    }

                    var shipX = shipSpacing + i * (50 + shipSpacing);
                    var enemyShip = new EnemyShip(enemySprite, 2);
                    enemyShip.RenderAt(shipX, rowY);
                    this.canvas.Children.Add(enemyShip);
                    this.enemyShips.Add(enemyShip);
                    this.originalEnemyPositions.Add(shipX);
                }
            }
        }

        /// <summary>
        /// this updates how the enemy move around
        /// </summary>
        public void UpdateEnemies()
        {
            if (this.enemyShips.Count == 0 || this.enemyShips.All(e => e == null))
            {
                this.endGame(true);
                return;
            }

            var firstEnemy = this.enemyShips.FirstOrDefault(e => e != null);
            if (firstEnemy == null)
            {
                this.endGame(true);
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

        private void endGame(bool win)
        {
            this.gameOver = true;
            this.gameOverTextBlock = new TextBlock
            {
                Text = win ? "Game Over... You Win!!!" : "Game Over... You Lose!!!",
                FontSize = 24,
                Margin = new Thickness(10, this.canvasHeight - 40, 0, 0)
            };
            this.canvas.Children.Add(this.gameOverTextBlock);

        }

        private void updateEnemyPosition(EnemyShip enemy)
        {
            Canvas.SetLeft(enemy.Sprite, enemy.X);
            Canvas.SetTop(enemy.Sprite, enemy.Y);
        }

        /// <summary>
        /// this is how the player fires their fireball, the space bar calls here
        /// </summary>
        public void FireBullet()
        {
            if (this.activePlayerBullets.Count < MaxBulletsAllowed && !this.gameOver)
            {
                var bullet = new Bullet(new BulletSprite());
                bullet.RenderAt(this.player.X + this.player.Width / 2 - bullet.Width / 2, this.canvasHeight - PlayerOffsetFromBottom - bullet.Height);
                this.canvas.Children.Add(bullet);
                this.activePlayerBullets.Add(bullet);
            }
        }
        /// <summary>
        /// this is how the enemy fires their fireball
        /// </summary>
        /// <param name="enemy">which enemy is firing</param>
        public void FireEnemyBullet(EnemyShip enemy)
        {
            if (enemy != null && enemy.Sprite.Content is EnemyShipLevel3Sprite && !this.gameOver)
            {
                var bullet = new Bullet(new BulletSprite());
                bullet.RenderAt(enemy.X + enemy.Width / 2 - bullet.Width / 2, enemy.Y + enemy.Height);
                this.canvas.Children.Add(bullet);
                this.activeEnemyBullets.Add(bullet);
            }
        }

        private void updateBullets()
        {
            for (var i = this.activePlayerBullets.Count - 1; i >= 0; i--)
            {
                var bullet = this.activePlayerBullets[i];
                bullet.MoveUp();
                
                if (bullet.IsOffScreen(this.canvasHeight))
                {
                    this.removeBullet(i);
                }
                else
                {
                    this.checkBulletCollisions(i);
                }
            }
        }

        private void updateEnemyBullets()
        {
            for (var i = this.activeEnemyBullets.Count - 1; i >= 0; i--)
            {
                var bullet = this.activeEnemyBullets[i];
                bullet.MoveDown();
                if (bullet.IsOffScreen(this.canvasHeight))
                {
                    this.removeEnemyBullet(i);
                }
                else
                {
                    this.checkEnemyBulletCollisions(i);
                }
            }
        }



        private void removeBullet(int index)
        {
            var bullet = this.activePlayerBullets[index];
            if (bullet != null)
            {
                this.canvas.Children.Remove(bullet);
                this.activePlayerBullets.RemoveAt(index);
            }
        }

        private void removeEnemyBullet(int index)
        {
            var bullet = this.activeEnemyBullets[index];
            this.canvas.Children.Remove(bullet);
            this.activeEnemyBullets.RemoveAt(index);
        }

        private void checkBulletCollisions(int index)
        {
            var playerBullet = this.activePlayerBullets[index];
            if (playerBullet != null)
            {
                for (var i = this.enemyShips.Count - 1; i >= 0; i--)
                {
                    var enemy = this.enemyShips[i];
                    if (this.isCollision(playerBullet, enemy))
                    {
                        this.destroyEnemy(enemy);
                        this.removeBullet(index);
                        this.updateScore(enemy);
                        break;
                    }
                }
            }
        }

        private void checkEnemyBulletCollisions(int index)
        {
            var enemyBullet = this.activeEnemyBullets[index];
            if (enemyBullet != null && this.isCollision(enemyBullet))
            {
                this.handlePlayerHit();
                this.removeEnemyBullet(index);
            }
        }

        private void handlePlayerHit()
        {
            this.endGame(false);
            this.canvas.Children.Remove(this.player.Sprite);
        }

        private bool isCollision(Bullet bullet, EnemyShip enemy)
        {
            var bulletLeft = bullet.X;
            var bulletRight = bullet.X + bullet.Width;
            var bulletTop = bullet.Y;
            var bulletBottom = bullet.Y + bullet.Height;

            var enemyLeft = enemy.X;
            var enemyWidth = enemy.Width;
            var enemyHeight = enemy.Height;

            var enemyRight = enemy.X + enemyWidth;
            var enemyTop = enemy.Y;
            var enemyBottom = enemy.Y + enemyHeight;
            if (enemyWidth <= 0 || enemyHeight <= 0)
            {
                return false;
            }
            if (bulletBottom >= enemyTop && bulletTop <= enemyBottom)
            {
                return bulletRight > enemyLeft && bulletLeft < enemyRight;
            }

            return false;
        }

        private bool isCollision(Bullet bullet)
        {
            var bulletLeft = bullet.X;
            var bulletRight = bullet.X + bullet.Width;
            var bulletTop = bullet.Y;
            var bulletBottom = bullet.Y + bullet.Height;

            var playerLeft = this.player.X;
            var playerRight = this.player.X + this.player.Width;
            var playerTop = this.player.Y;
            var playerBottom = this.player.Y + this.player.Height;
            
            if (bulletBottom >= playerTop && bulletTop <= playerBottom)
            {
                return bulletRight > playerLeft && bulletLeft < playerRight;
            }

            return false;
        }

        private void destroyEnemy(EnemyShip enemy)
        {
            int index = this.enemyShips.IndexOf(enemy);
            if (index >= 0)
            {
                this.canvas.Children.Remove(enemy);
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
            if (this.enemyShips.Count == 0)
            {
                this.endGame(true);
            }
        }


        private void updateScore(EnemyShip enemy)
        {
            var pointsToAdd = 0;

            switch (enemy.Sprite.Content)
            {
                case EnemyShipLevel1Sprite _:
                    pointsToAdd = 100;
                    break;
                case EnemyShipLevel2Sprite _:
                    pointsToAdd = 200;
                    break;
                case EnemyShipLevel3Sprite _:
                    pointsToAdd = 300;
                    break;
            }

            this.score += pointsToAdd;
            this.updateScoreDisplay();
        }


        private void updateScoreDisplay()
        {
            this.scoreTextBlock.Text = $"Score: {this.score}";
        }

        #endregion
    }
}
