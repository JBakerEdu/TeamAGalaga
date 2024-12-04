using Galaga.View.Sprites;
using Windows.UI.Xaml.Controls;

namespace Galaga.Model
{
    /// <summary>
    /// This is a bullet object which will be fired from ships at others.
    /// </summary>
    public class Bullet : GameObject
    {
        // Speed components for the bullet
        private double xSpeed;
        private double ySpeed;

        /// <summary>
        /// Initializes a new instance of the <see cref="Bullet"/> class with the specified sprite.
        /// </summary>
        /// <param name="bulletSprite">The sprite to use for the bullet.</param>
        /// <param name="xSpeed">Speed along the x-axis.</param>
        /// <param name="ySpeed">Speed along the y-axis.</param>
        public Bullet(BaseSprite bulletSprite, double xSpeed, double ySpeed)
        {
            Sprite = bulletSprite;
            SetSpeed(xSpeed, ySpeed);
        }

        /// <summary>
        /// Sets the speed of the bullet.
        /// </summary>
        /// <param name="xSpeed">Speed along the x-axis.</param>
        /// <param name="ySpeed">Speed along the y-axis.</param>
        public void SetSpeed(double xSpeed, double ySpeed)
        {
            this.xSpeed = xSpeed;
            this.ySpeed = ySpeed;
        }

        /// <summary>
        /// Moves the bullet according to its current speed.
        /// </summary>
        public void Move()
        {
            X += xSpeed;
            Y += ySpeed;

            Canvas.SetLeft(Sprite, X);
            Canvas.SetTop(Sprite, Y);
        }

        /// <summary>
        /// Renders the sprite to an X and Y value on the canvas.
        /// </summary>
        /// <param name="x">The x-coordinate on the canvas.</param>
        /// <param name="y">The y-coordinate on the canvas.</param>
        public void RenderAt(double x, double y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Checks if the bullet is off the screen.
        /// </summary>
        /// <param name="screenHeight">The height of the canvas to determine if out of range.</param>
        /// <returns>True if the bullet is off-screen, otherwise false.</returns>
        public bool IsOffScreen(double screenHeight)
        {
            return Y + Sprite.Height < 0 || Y > screenHeight || X + Sprite.Width < 0 || X > screenHeight;
        }
    }
}
