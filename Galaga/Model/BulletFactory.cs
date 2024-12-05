using Galaga.View.Sprites;
using System;

namespace Galaga.Model
{
    public class BulletFactory
    {
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
