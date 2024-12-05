using Galaga.View.Sprites;

namespace Galaga.Model
{
    /// <summary>
    /// Represents a bonus ships that can appear in game. 
    /// </summary>
    public class BonusShip : GameObject
    {
        #region Data members

        private const int SpeedXDirection = 5;
        private const int SpeedYDirection = 0;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the  <see cref="BonusShip"/> class.
        /// </summary>
        public BonusShip(BaseSprite sprite)
        {
            Sprite = sprite;
            SetSpeed(SpeedXDirection, SpeedYDirection);
        }

        #endregion
    }
}