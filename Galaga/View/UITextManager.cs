using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Galaga.Model;

namespace Galaga.View
{
    public class UiTextManager
    {
        private readonly Canvas canvas;
        private readonly GameManager gameManager;
        private TextBlock scoreTextBlock;
        private TextBlock playerLivesTextBlock;
        private int score;
        private const int SmallFontSize = 15;
        private const int LargeFontSize = 25;
        private const int WaitTime = 3000;

        /// <summary>
        /// Marks when game is over
        /// </summary>
        public bool GameOver { get; private set; }
        private TextBlock gameOverTextBlock;
        private TextBlock powerUpTextBlock;
        private TextBlock levelTextBlock;

        /// <summary>
        /// This is the constructor for the UiTextManager and will set and initialize the correct data onto the canvas. 
        /// </summary>
        /// <param name="canvas">the canvas to add the text onto</param>
        /// <param name="playerLives">the life count for the player ship</param>
        /// <param name="gameManager">the game manager ot report back to</param>

        public UiTextManager(Canvas canvas, int playerLives, GameManager gameManager)
        {

            this.canvas = canvas;
            this.gameManager = gameManager;
            this.GameOver = false;
            this.initializeScoreGame();
            this.initializePlayerLives(playerLives);
            this.initializeLevel();

            EnemyManager.OnGameEnd += isWin =>
            {
                this.EndGame(isWin);
            };

        }

        /// <summary>
        /// Adds or updates the text field at the top of the canvas to display the active power-up.
        /// </summary>
        /// <param name="powerUpName">The name of the active power-up.</param>
        public void SetPowerUpText(string powerUpName)
        {
            var powerUpText = $"Active Power-Up: {powerUpName}";

            if (this.powerUpTextBlock != null)
            {
                this.powerUpTextBlock.Text = powerUpText;
            }
            else
            {
                this.powerUpTextBlock = new TextBlock
                {
                    Text = powerUpText,
                    FontSize = SmallFontSize,
                    Foreground = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.Yellow)
                };

                this.canvas.Children.Add(this.powerUpTextBlock);
                this.powerUpTextBlock.Measure(new Windows.Foundation.Size(double.PositiveInfinity, double.PositiveInfinity));
                var textSize = this.powerUpTextBlock.DesiredSize;

                Canvas.SetLeft(this.powerUpTextBlock, (this.canvas.Width - textSize.Width) / 2);
                Canvas.SetTop(this.powerUpTextBlock, 10);
            }
        }

        private void initializeLevel()
        {
            this.levelTextBlock = new TextBlock
            {
                Text = "Level: 0",
                FontSize = SmallFontSize,
                Foreground = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.White)
            };

            this.canvas.Children.Add(this.levelTextBlock);
            this.scoreTextBlock.Measure(new Windows.Foundation.Size(double.PositiveInfinity, double.PositiveInfinity));
            var textSize = this.levelTextBlock.DesiredSize;

            Canvas.SetLeft(this.levelTextBlock, (this.canvas.Width - textSize.Width) / 2);
            Canvas.SetTop(this.levelTextBlock, this.canvas.Height - textSize.Height - 10);
        }

        private void initializeScoreGame()
        {
            this.scoreTextBlock = new TextBlock
            {
                Text = "Score: 0",
                FontSize = SmallFontSize,
                Foreground = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.White)
            };

            this.canvas.Children.Add(this.scoreTextBlock);
            this.scoreTextBlock.Measure(new Windows.Foundation.Size(double.PositiveInfinity, double.PositiveInfinity));
            var textSize = this.scoreTextBlock.DesiredSize;

            Canvas.SetLeft(this.scoreTextBlock, this.canvas.Width - textSize.Width - 10);
            Canvas.SetTop(this.scoreTextBlock, 10);
        }

        private void initializePlayerLives(int playerLives)
        {
            this.playerLivesTextBlock = new TextBlock
            {
                Text = $"Lives: {playerLives}",
                FontSize = SmallFontSize,
                Foreground = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.White),
                Margin = new Thickness(10, 10, 0, 0)
            };
            this.canvas.Children.Add(this.playerLivesTextBlock);
        }

        /// <summary>
        /// Updates the level on screen by showing it in large text in the center, then small text at the bottom.
        /// </summary>
        /// <param name="level">The current level to display.</param>
        /// <param name="waitTime">The time to wait before switching to the smaller display.</param>
        public async void UpdateLevel(int level)
        {
            this.levelTextBlock.FontSize = LargeFontSize;
            this.levelTextBlock.Text = $"Level: {level}";

            this.levelTextBlock.Measure(new Windows.Foundation.Size(double.PositiveInfinity, double.PositiveInfinity));
            var firstTextSize = this.levelTextBlock.DesiredSize;
            Canvas.SetLeft(this.levelTextBlock, (this.canvas.Width - firstTextSize.Width) / 2);
            Canvas.SetTop(this.levelTextBlock, (this.canvas.Height - firstTextSize.Height) / 2);

            await Task.Delay(WaitTime);

            this.levelTextBlock.FontSize = SmallFontSize;

            this.levelTextBlock.Measure(new Windows.Foundation.Size(double.PositiveInfinity, double.PositiveInfinity));
            var secondTextSize = this.levelTextBlock.DesiredSize;
            Canvas.SetLeft(this.levelTextBlock, (this.canvas.Width - secondTextSize.Width) / 2);
            Canvas.SetTop(this.levelTextBlock, this.canvas.Height - secondTextSize.Height - 10);
        }

        /// <summary>
        /// Updates the score on screen with the enemy ship's point value added to the current score.
        /// </summary>
        /// <param name="enemy">The enemy ship to get the correct point value from.</param>
        public void UpdateScore(EnemyShip enemy)
        {
            this.score += enemy.Score;
            this.scoreTextBlock.Text = $"Score: {this.score}";
            this.scoreTextBlock.Measure(new Windows.Foundation.Size(double.PositiveInfinity, double.PositiveInfinity));
            var textSize = this.scoreTextBlock.DesiredSize;
            Canvas.SetLeft(this.scoreTextBlock, this.canvas.Width - textSize.Width - 10);
            Canvas.SetTop(this.scoreTextBlock, 10);
        }


        /// <summary>
        /// this updates the players lives left removing lives as the player gets hit
        /// </summary>
        /// <param name="playerLives">the number of lives left</param>

        public void UpdatePlayerLives(int playerLives)
        {
            playerLives = Math.Max(0, playerLives);
            this.playerLivesTextBlock.Text = $"Lives: {playerLives}";
        }

        /// <summary>
        /// Sets the end game output based on whether the player wins or loses,
        /// and displays the message centered on the screen.
        /// </summary>
        /// <param name="win">True if the player wins, false if they lose.</param>
        public async void EndGame(bool win)
        {
            if (!this.GameOver)
            {
                var gameOverText = win ? "You Win!!!" : "You Lose!!!";
                this.GameOver = true;
                this.gameManager.BonusShipSpawn(false);
                this.gameOverTextBlock = new TextBlock
                {
                    Text = gameOverText,
                    FontSize = LargeFontSize,
                    Foreground = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.White)
                };

                this.canvas.Children.Add(this.gameOverTextBlock);
                this.gameOverTextBlock.Measure(new Windows.Foundation.Size(double.PositiveInfinity, double.PositiveInfinity));
                var textSize = this.gameOverTextBlock.DesiredSize;
                Canvas.SetLeft(this.gameOverTextBlock, (this.canvas.Width - textSize.Width) / 2);
                Canvas.SetTop(this.gameOverTextBlock, (this.canvas.Height - textSize.Height) / 2);
                AudioManager.PlayGameOver(this.gameManager.gameType);
                await Task.Delay(WaitTime);
                (Window.Current.Content as Frame)?.Navigate(typeof(HighScorePage), this.score);
            }
        }
    }
}
