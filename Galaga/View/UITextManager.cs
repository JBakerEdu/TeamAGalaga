using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Galaga.View;

namespace Galaga.Model
{
    /// <summary>
    /// This is the text Manager over the UI , so it is the manager over the text that appears on the canvas
    /// </summary>
    public class UiTextManager
    {
        private readonly Canvas canvas;
        private readonly GameManager gameManager;
        private TextBlock scoreTextBlock;
        private TextBlock playerLivesTextBlock;
        private int score;
        /// <summary>
        /// Marks when game is over
        /// </summary>
        public bool GameOver { get; private set; }
        private TextBlock gameOverTextBlock;
        private TextBlock powerUpTextBlock;

        /// <summary>
        /// This is the constructor for the UiTextManager and will set and initialize the correct data onto the canvas. 
        /// </summary>
        /// <param name="canvas">the canvas to add the text onto</param>
        /// <param name="playerLives">the life count for the player ship</param>
        public UiTextManager(Canvas canvas, int playerLives, GameManager gameManager)
        {
            this.canvas = canvas;
            this.gameManager = gameManager;
            this.GameOver = false;
            this.initializeScoreGame();
            this.initializePlayerLives(playerLives);
        }


        private void initializeScoreGame()
        {
            this.scoreTextBlock = new TextBlock
            {
                Text = "Score: 0",
                FontSize = 24,
                Foreground = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.White),
                Margin = new Thickness(this.canvas.Width - this.canvas.Width / 4, 10, 0, 0)
            };
            this.canvas.Children.Add(this.scoreTextBlock);
        }

        /// <summary>
        /// Adds or updates the text field at the top of the canvas to display the active power-up.
        /// </summary>
        /// <param name="powerUpName">The name of the active power-up.</param>
        public void SetPowerUpText(string powerUpName)
        {
            string powerUpText = $"Active Power-Up: {powerUpName}";

            if (this.powerUpTextBlock != null)
            {
                // Update the text if the TextBlock already exists
                this.powerUpTextBlock.Text = powerUpText;
            }
            else
            {
                // Create the TextBlock
                this.powerUpTextBlock = new TextBlock
                {
                    Text = powerUpText,
                    FontSize = 15,
                    Foreground = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.Yellow)
                };

                // Temporarily add to the canvas to measure size
                this.canvas.Children.Add(this.powerUpTextBlock);
                this.powerUpTextBlock.Measure(new Windows.Foundation.Size(double.PositiveInfinity, double.PositiveInfinity));
                var textSize = this.powerUpTextBlock.DesiredSize;

                // Adjust position based on measured size
                Canvas.SetLeft(this.powerUpTextBlock, (this.canvas.Width - textSize.Width) / 2); // Center horizontally
                Canvas.SetTop(this.powerUpTextBlock, 10); // Fixed distance from the top
            }
        }

        private void initializePlayerLives(int playerLives)
        {
            this.playerLivesTextBlock = new TextBlock
            {
                Text = $"Lives: {playerLives}",
                FontSize = 24,
                Foreground = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.White),
                Margin = new Thickness(10, 10, 0, 0)
            };
            this.canvas.Children.Add(this.playerLivesTextBlock);
        }

        /// <summary>
        /// this updates the score on screen with the enemy ships point value added to current score
        /// </summary>
        /// <param name="enemy"> the enemy ship so it can get the correct point value</param>
        public void UpdateScore(EnemyShip enemy)
        {
            this.score += enemy.Score;
            this.scoreTextBlock.Text = $"Score: {this.score}";
        }

        /// <summary>
        /// this updates the players lives left removing lives as the player gets hit
        /// </summary>
        /// <param name="playerLives">the number of lives left</param>
        public void UpdatePlayerLives(int playerLives)
        {
            if (playerLives <= 0)
            {
                playerLives = 0;
            }
            this.playerLivesTextBlock.Text = $"Lives: {playerLives}";
        }

        /// <summary>
        /// will set the end game out put based on the boolean value of if the player wins or not
        /// </summary>
        /// <param name="win">if the player wins or looses</param>
        public void EndGame(bool win)
        {
            if (!this.GameOver)
            {
                String gameOverText = win ? "You Win!!!" : "You Loose!!!";
                this.GameOver = true;
                this.gameManager.BonusShipSpawn(false);
                this.gameOverTextBlock = new TextBlock
                {
                    Text = gameOverText,
                    FontSize = 24,
                    Foreground = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.White),
                    Margin = new Thickness(10, this.canvas.Height - 40, 0, 0)
                };
                this.canvas.Children.Add(this.gameOverTextBlock);
                AudioManager.PlayGameOver();
            }
        }
    }
}