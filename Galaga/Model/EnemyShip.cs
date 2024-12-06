using System;
using Galaga.View.Sprites;
using Galaga.View.Sprites.HolidayGame;
using Galaga.View.Sprites.OriginalGame;

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
        public EnemyShip(BaseSprite sprite, int xSpeed, int ySpeed, int level, bool isShooter, GameType gameType)
        {
            Sprite = sprite;
            this.Level = level;
            this.Score = level * PointMultiplier;
            this.IsShooter = isShooter;
            SetSpeed(xSpeed, ySpeed);

            if (level == 3)
            {
                this.level3SecondSprite(gameType);
            }
            else if (level == 4)
            {
                this.level4SecondSprite(gameType);
            }
        }

        private void level3SecondSprite(GameType gameType)
        {
            BaseSprite shipSprite = gameType switch
            {
                GameType.HolidayGame => new HolidayEnemyShipLevel3SecondSprite(),
                GameType.OriginalGame => new EnemyShipLevel3SecondSprite(),
                _ => throw new ArgumentException("Unsupported game type")
            };
            this.Sprite2 = shipSprite;
            this.HasSecondSprite = true;
        }

        private void level4SecondSprite(GameType gameType)
        {
            BaseSprite shipSprite = gameType switch
            {
                GameType.HolidayGame => new HolidayEnemyShipLevel4SecondSprite(),
                GameType.OriginalGame => new EnemyShipLevel4SecondSprite(),
                _ => throw new ArgumentException("Unsupported game type")
            };
            this.Sprite2 = shipSprite;
            this.HasSecondSprite = true;
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
                this.renderSecondSprite(x, y);
            }
        }

        /// <summary>
        /// Renders the unique part of the second sprite.
        /// </summary>
        /// <param name="x">The x-coordinate on the canvas.</param>
        /// <param name="y">The y-coordinate on the canvas.</param>
        private void renderSecondSprite(double x, double y)
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