using Galaga.View.Sprites;

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
        public static EnemyShip CreateEnemyShipLevel1()
        {
            return new EnemyShip(new EnemyShipLevel1Sprite(), DefaultEnemySpeedX, DefaultEnemySpeedY, 1, !DefaultEnemyShooter);
        }

        /// <summary>
        /// Creates a level 2 enemy ship with default settings.
        /// </summary>
        public static EnemyShip CreateEnemyShipLevel2()
        {
            return new EnemyShip(new EnemyShipLevel2Sprite(), DefaultEnemySpeedX, DefaultEnemySpeedY, 2, !DefaultEnemyShooter);
        }

        /// <summary>
        /// Creates a level 3 enemy ship with default settings.
        /// </summary>
        public static EnemyShip CreateEnemyShipLevel3()
        {
            return new EnemyShip(new EnemyShipLevel3Sprite(), DefaultEnemySpeedX, DefaultEnemySpeedY, 3, DefaultEnemyShooter);
        }

        /// <summary>
        /// Creates a level 4 enemy ship with default settings.
        /// </summary>
        public static EnemyShip CreateEnemyShipLevel4()
        {
            return new EnemyShip(new EnemyShipLevel4Sprite(), DefaultEnemySpeedX, DefaultEnemySpeedY, 4, DefaultEnemyShooter);
        }

        #endregion

        #region Bonus Ship Creation
        /// <summary>
        /// Creates a new bonus ship with default settings.
        /// </summary>
        public static BonusShip CreateBonusShip()
        {
            return new BonusShip();
        }
        #endregion

        #region Player Ship Creation

        /// <summary>
        /// Creates the player ship.
        /// </summary>
        public static Player CreatePlayerShip()
        {
            var player = new Player();
            return player;
        }

        #endregion
    }
}
