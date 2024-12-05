using Galaga.View.Sprites;
using System;

namespace Galaga.Model
{
    public class BulletFactory
    {
        // Static method to create a Bullet with a BulletSprite
        public static Bullet CreateBullet(double velocityX, double velocityY, GameType gameType)
        {
            BaseSprite bulletSprite = gameType switch
            {
                GameType.HolidayGame => new HolidayBulletSprite(), // Assuming you have a HolidayBulletSprite
                GameType.OriginalGame => new BulletSprite(), // Default BulletSprite for the original game
                _ => throw new ArgumentException("Unsupported game type") // Error if gameType is not supported
            };
            var bullet = new Bullet(bulletSprite, velocityX, velocityY);

            return bullet;
        }
    }
}
