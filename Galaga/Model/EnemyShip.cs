using System;
using Galaga.View.Sprites;

namespace Galaga.Model
{
    /// <summary>
    /// Represents an enemy ship with customizable properties.
    /// </summary>
    public class EnemyShip : GameObject
    {
        private const int PointMultiplier = 100;
        private readonly Random random = new Random();
        public BaseSprite Sprite2 { get; protected set; }

        /// <summary>
        ///This is the level of the enemy ship
        /// </summary>
        public int Level;
        /// <summary>
        /// this is a boolean if a sprite has two sprites
        /// </summary>
        public bool HasSecondSprite;
        /// <summary>
        /// this is the point value of the ship if destroyed
        /// </summary>
        public int Score { get; private set; }
        /// <summary>
        /// this is a boolean if a sprite can shoot/fire
        /// </summary>
        public bool IsShooter { get; private set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="EnemyShip"/> class.
        /// </summary>
        /// <param name="sprite">The sprite instance to be used for the enemy ship.</param>
        /// <param name="xSpeed">The speed of the enemy ship on x axis.</param>
        /// <param name="ySpeed">The speed of the enemy ship on y axis.</param>
        /// <param name="level">The level of the ship.</param>
        /// <param name="isShooter">The value used to see if enemy can shoot or not.</param>
        public EnemyShip(BaseSprite sprite, int xSpeed, int ySpeed, int level, bool isShooter)
        {
            Sprite = sprite;
            this.Level = level;
            this.Score = level * PointMultiplier;
            this.IsShooter = isShooter;
            SetSpeed(xSpeed, ySpeed);

            // Initialize secondary sprite only for levels 3 and 4
            if (level == 3)
            {
                this.Sprite2 = new EnemyShipLevel3SecondSprite();
                this.HasSecondSprite = true;
            }
            else if (level == 4)
            {
                this.Sprite2 = new EnemyShipLevel4SecondSprite();
                this.HasSecondSprite = true;
            }
        }


        /// <summary>
        /// Renders the enemy ship at the specified (x, y) location.
        /// Updates both the primary and secondary sprites, but avoids duplicating shared logic.
        /// </summary>
        /// <param name="x">The x-coordinate on the canvas.</param>
        /// <param name="y">The y-coordinate on the canvas.</param>
        public void RenderAt(double x, double y)
        {
            X = x;
            Y = y;

            // Render the primary sprite
            Sprite.RenderAt(x, y);

            // Render the secondary sprite only if it exists
            if (this.HasSecondSprite && this.Sprite2 != null)
            {
                this.RenderSecondSprite(x, y);
            }
        }

        /// <summary>
        /// Renders the unique part of the second sprite.
        /// </summary>
        /// <param name="x">The x-coordinate on the canvas.</param>
        /// <param name="y">The y-coordinate on the canvas.</param>
        private void RenderSecondSprite(double x, double y)
        {
            // Adjust position or appearance for the second sprite as needed
            this.Sprite2.RenderAt(x, y);
        }

        /// <summary>
        /// Moves the enemy ship in a specified direction based on its speed.
        /// Updates both the primary and secondary sprites.
        /// </summary>
        /// <param name="deltaX">The change in X position, multiplied by speed.</param>
        public void Move(double deltaX)
        {
            X += deltaX * SpeedX;

            // Move the primary sprite
            Sprite.RenderAt(X, Y);

            // Move the secondary sprite only if it exists
            if (this.HasSecondSprite && this.Sprite2 != null)
            {
                this.Sprite2.RenderAt(X, Y);
            }
        }

    }
}