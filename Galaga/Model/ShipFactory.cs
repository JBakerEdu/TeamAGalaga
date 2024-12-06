using Galaga.View.Sprites;
using System;
using Galaga.View.Sprites.HolidayGame;
using Galaga.View.Sprites.OriginalGame;

namespace Galaga.Model
{
    /// <summary>
    /// Factory class to create different types of ships (player and enemies)
    /// </summary>
    public static class ShipFactory
    {
        #region Constants

        private const int DefaultEnemySpeedX = 10;
        private const int DefaultEnemySpeedY = 0;
        private const bool DefaultEnemyShooter = true;

        #endregion

        #region Enemy Ship Creation

        /// <summary>
        /// Creates a level 1 enemy ship with default settings.
        /// </summary>
        public static EnemyShip CreateEnemyShipLevel1(GameType gameType)
        {
            BaseSprite shipSprite = gameType switch
            {
                GameType.HolidayGame => new HolidayEnemyShipLevel1Sprite(),
                GameType.OriginalGame => new EnemyShipLevel1Sprite(),
                _ => throw new ArgumentException("Unsupported game type")
            };
            return new EnemyShip(shipSprite, DefaultEnemySpeedX, DefaultEnemySpeedY, 1, !DefaultEnemyShooter, gameType);
        }

        /// <summary>
        /// Creates a level 2 enemy ship with default settings.
        /// </summary>
        public static EnemyShip CreateEnemyShipLevel2(GameType gameType)
        {

            BaseSprite shipSprite = gameType switch
            {
                GameType.HolidayGame => new HolidayEnemyShipLevel2Sprite(),
                GameType.OriginalGame => new EnemyShipLevel2Sprite(),
                _ => throw new ArgumentException("Unsupported game type")
            };
            return new EnemyShip(shipSprite, DefaultEnemySpeedX, DefaultEnemySpeedY, 2, !DefaultEnemyShooter, gameType);
        }

        /// <summary>
        /// Creates a level 3 enemy ship with default settings.
        /// </summary>
        public static EnemyShip CreateEnemyShipLevel3(GameType gameType)
        {
            BaseSprite shipSprite = gameType switch
            {
                GameType.HolidayGame => new HolidayEnemyShipLevel3Sprite(),
                GameType.OriginalGame => new EnemyShipLevel3Sprite(),
                _ => throw new ArgumentException("Unsupported game type")
            };
            return new EnemyShip(shipSprite, DefaultEnemySpeedX, DefaultEnemySpeedY, 3, DefaultEnemyShooter, gameType);
        }

        /// <summary>
        /// Creates a level 4 enemy ship with default settings.
        /// </summary>
        public static EnemyShip CreateEnemyShipLevel4(GameType gameType)
        {
            BaseSprite shipSprite = gameType switch
            {
                GameType.HolidayGame => new HolidayEnemyShipLevel4Sprite(),
                GameType.OriginalGame => new EnemyShipLevel4Sprite(),
                _ => throw new ArgumentException("Unsupported game type")
            };
            return new EnemyShip(shipSprite, DefaultEnemySpeedX, DefaultEnemySpeedY, 4, DefaultEnemyShooter, gameType);
        }

        #endregion

        #region Bonus Ship Creation
        /// <summary>
        /// Creates a new bonus ship with default settings.
        /// </summary>
        public static BonusShip CreateBonusShip(GameType gameType)
        {
            BaseSprite shipSprite = gameType switch
            {
                GameType.HolidayGame => new HolidayBonusShipCarePackageSprite(),
                GameType.OriginalGame => new BonusShipCarePackageSprite(),
                _ => throw new ArgumentException("Unsupported game type")
            };
            return new BonusShip(shipSprite);
        }
        #endregion

        #region Player Ship Creation

        /// <summary>
        /// Creates the player ship.
        /// </summary>
        public static Player CreatePlayerShip(GameType gameType)
        {
            var shipSprite = gameType switch
            {
                GameType.HolidayGame => new Player(new HolidayPlayerSprite()),
                GameType.OriginalGame => new Player(new PlayerSprite()),
                _ => throw new ArgumentException("Unsupported game type")
            };
            return shipSprite;
        }

        #endregion
    }
}
