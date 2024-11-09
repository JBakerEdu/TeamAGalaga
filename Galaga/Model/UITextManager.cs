using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Galaga.Model
{
    public class UITextManager
    {
        private readonly Canvas canvas;
        private TextBlock scoreTextBlock;
        private TextBlock playerLivesTextBlock;
        private int score;
        public bool gameOver { get; private set; }
        private TextBlock gameOverTextBlock;

        public UITextManager(Canvas canvas, int playerLives)
        {
            this.canvas = canvas;
            this.gameOver = false;
            this.initializeScoreGame();
            this.initializePlayerLives(playerLives);
        }


        private void initializeScoreGame()
        {
            this.scoreTextBlock = new TextBlock
            {
                Text = "Score: 0",
                FontSize = 24,
                Margin = new Thickness(this.canvas.Width - this.canvas.Width / 4 , 10, 0, 0)
            };
            this.canvas.Children.Add(this.scoreTextBlock);
        }

        private void initializePlayerLives(int playerLives)
        {
            this.playerLivesTextBlock = new TextBlock
            {
                Text = $"Lives: {playerLives}",
                FontSize = 24,
                Margin = new Thickness(10, 10, 0, 0)
            };
            this.canvas.Children.Add(this.playerLivesTextBlock);
        }


        public void updateScore(EnemyShip enemy)
        {
            this.score += enemy.Score;
            this.scoreTextBlock.Text = $"Score: {this.score}";
        }

        public void updatePlayerLives(int playerLives)
        {
            if (playerLives <= 0)
            {
                playerLives = 0;
            }
            this.playerLivesTextBlock.Text = $"Lives: {playerLives}";
        }

        public void endGame(bool win)
        {
            if (!this.gameOver)
            {
                String gameOverText = win ? "You Win!!!" : "You Loose!!!";
                this.gameOver = true;
                this.gameOverTextBlock = new TextBlock
                {
                    Text = gameOverText,
                    FontSize = 24,
                    Margin = new Thickness(10, this.canvas.Height - 40, 0, 0)
                };
                this.canvas.Children.Add(this.gameOverTextBlock);
            }
        }
    }
}