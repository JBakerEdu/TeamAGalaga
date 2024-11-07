using Windows.UI.Xaml.Controls;
using Galaga.View.Sprites;

namespace Galaga.Model
{
    public class Bullet : BaseSprite
    {
        /// <summary>
        /// gets and sets the sprites X on canvas
        /// </summary>
        public double X
        {
            get => Canvas.GetLeft(this);
            set => Canvas.SetLeft(this, value);
        }

        /// <summary>
        /// gets and sets the sprites Y on canvas
        /// </summary>
        public double Y
        {
            get => Canvas.GetTop(this);
            set => Canvas.SetTop(this, value);
        }
        /// <summary>
        /// Gets and sets the sprite in use
        /// </summary>
        public BaseSprite Sprite { get; private set; }
        /// <summary>
        /// Inits the sprite 
        /// </summary>
        /// <param name="bulletSprite">in the new sprite using bullet</param>
        public Bullet(BaseSprite bulletSprite)
        {
            this.Sprite = bulletSprite;
            Width = 20;
            Height = 20;
            Content = this.Sprite;
        }

        /// <summary>
        /// Renders the sprite to an X and Y value
        /// </summary>
        /// <param name="x">the x on canvas</param>
        /// <param name="y">the y on canvas</param>
        public new void RenderAt(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// moves bullet up
        /// </summary>
        public void MoveUp()
        {
            this.Y -= 5;
        }

        /// <summary>
        /// Moves bullet down
        /// </summary>
        public void MoveDown()
        {
            this.Y += 5;
        }

        /// <summary>
        /// Checks if bullet goes out of canvas
        /// </summary>
        /// <param name="screenHeight">the height of the canvas to know if out of range</param>
        /// <returns>bool of if on canvas or not</returns>
        public bool IsOffScreen(double screenHeight)
        {
            return (this.Y + this.Sprite.Height < 0) || (this.Y + this.Sprite.Height > this.Y + screenHeight);
        }
    }
}
