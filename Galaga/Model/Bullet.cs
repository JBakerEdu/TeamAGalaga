using Windows.UI.Xaml.Controls;
using Galaga.View.Sprites;

namespace Galaga.Model
{
    public class Bullet : GameObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Bullet"/> class with the specified sprite.
        /// </summary>
        /// <param name="bulletSprite">The sprite to use for the bullet.</param>
        public Bullet(BaseSprite bulletSprite, int xSpeed, int ySpeed)
        {
            this.Sprite = bulletSprite;
            SetSpeed(xSpeed, ySpeed);
        }

        /// <summary>
        /// Renders the sprite to an X and Y value on the canvas.
        /// </summary>
        /// <param name="x">The x-coordinate on the canvas.</param>
        /// <param name="y">The y-coordinate on the canvas.</param>
        public void RenderAt(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// Checks if the bullet is off the screen.
        /// </summary>
        /// <param name="screenHeight">The height of the canvas to determine if out of range.</param>
        /// <returns>True if the bullet is off-screen, otherwise false.</returns>
        public bool IsOffScreen(double screenHeight)
        {
            return this.Y + this.Sprite.Height < 0 || this.Y > screenHeight;
        }
    }
}