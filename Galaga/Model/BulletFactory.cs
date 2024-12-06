using Galaga.View.Sprites;
using System;
using Galaga.View.Sprites.HolidayGame;
using Galaga.View.Sprites.OriginalGame;

namespace Galaga.Model
{
    /// <summary>
    /// Provides a factory for creating bullet objects with different types of sprites and velocities 
    /// based on the specified game type.
    /// </summary>
    public class BulletFactory
    {
        /// <summary>
        /// Creates a new bullet instance with the specified velocity and sprite type based on the game type.
        /// </summary>
        /// <param name="velocityX">The horizontal velocity of the bullet.</param>
        /// <param name="velocityY">The vertical velocity of the bullet.</param>
        /// <param name="gameType">The type of game, used to determine the bullet's sprite.</param>
        /// <returns>A new instance of the <see cref="Bullet"/> class with the appropriate sprite and velocities.</returns>
        /// <exception cref="ArgumentException">Thrown if the specified <paramref name="gameType"/> is unsupported.</exception>
        public static Bullet CreateBullet(double velocityX, double velocityY, GameType gameType)
        {
            BaseSprite bulletSprite = gameType switch
            {
                GameType.HolidayGame => new HolidayBulletSprite(),
                GameType.OriginalGame => new BulletSprite(),
                _ => throw new ArgumentException("Unsupported game type")
            };

            return new Bullet(bulletSprite, velocityX, velocityY);
        }
    }
}
