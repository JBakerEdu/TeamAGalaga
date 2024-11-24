using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Galaga.Model
{
    /// <summary>
    /// This is the text Manager over the UI , so it is the manager over the text that appears on the canvas
    /// </summary>
    public class UiTextManager
    {
        private readonly Canvas canvas;
        private TextBlock scoreTextBlock;
        private TextBlock playerLivesTextBlock;
        private int score;
        /// <summary>
        /// Marks when game is over
        /// </summary>
        public bool GameOver { get; private set; }
        private TextBlock gameOverTextBlock;

        /// <summary>
        /// This is the constructor for the UiTextManager and will set and initialize the correct data onto the canvas. 
        /// </summary>
        /// <param name="canvas">the canvas to add the text onto</param>
        /// <param name="playerLives">the life count for the player ship</param>
        public UiTextManager(Canvas canvas, int playerLives)
        {
            this.canvas = canvas;
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
        /// this updates the score on screen witht the enemy ships point value added to current score
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
                this.gameOverTextBlock = new TextBlock
                {
                    Text = gameOverText,
                    FontSize = 24,
                    Foreground = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.White),
                    Margin = new Thickness(10, this.canvas.Height - 40, 0, 0)
                };
                this.canvas.Children.Add(this.gameOverTextBlock);
            }
        }
    }
}