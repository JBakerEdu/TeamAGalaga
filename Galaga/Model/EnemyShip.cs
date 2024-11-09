using System;
using Galaga.View.Sprites;

namespace Galaga.Model
{
    /// <summary>
    /// Represents an enemy ship with customizable properties.
    /// </summary>
    public class EnemyShip : GameObject
    {
        private readonly Random random = new Random();

        public int Score { get; private set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="EnemyShip"/> class.
        /// </summary>
        /// <param name="sprite">The sprite instance to be used for the enemy ship.</param>
        /// <param name="speed">The speed of the enemy ship.</param>
        public EnemyShip(BaseSprite sprite, int xSpeed, int ySpeed, int pointValue)
        {
            this.Sprite = sprite;
            this.Score = pointValue;
            SetSpeed(xSpeed, ySpeed);  // Assuming horizontal movement speed only
        }

        /// <summary>
        /// Renders the enemy ship at the specified (x,y) location.
        /// </summary>
        /// <param name="x">The x-coordinate on the canvas.</param>
        /// <param name="y">The y-coordinate on the canvas.</param>
        public void RenderAt(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// Moves the enemy ship in a specified direction based on its speed.
        /// </summary>
        /// <param name="deltaX">The change in X position, multiplied by speed.</param>
        public void Move(double deltaX)
        {
            this.X += deltaX * this.SpeedX;
        }
    }
}