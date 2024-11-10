using System;
using System.Diagnostics;
using Windows.Devices.Printers;
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
        public int Level;
        public bool hasSecondSprite = false;

        public int Score { get; private set; }
        public bool isShooter { get; private set; }


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
            this.Sprite = sprite;
            this.Level = level;
            this.Score = level * PointMultiplier;
            this.isShooter = isShooter;
            SetSpeed(xSpeed, ySpeed);
            if (this.Level == 3)
            {
                this.Sprite2 = new EnemyShipLevel3SecondSprite();
                this.hasSecondSprite = true;
            }

            if (this.Level == 4)
            {
                this.Sprite2 = new EnemyShipLevel4SecondSprite();
                this.hasSecondSprite = true;
            }
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