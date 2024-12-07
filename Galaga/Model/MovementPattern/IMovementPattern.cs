using System;
using System.Collections.Generic;

namespace Galaga.Model.MovementPattern
{
    /// <summary>
    /// Defines a movement pattern that can be applied to a group of enemy ships in the game.
    /// </summary>
    public interface IMovementPattern
    {
        /// <summary>
        /// Applies the movement pattern to the provided list of enemy ships.
        /// </summary>
        /// <param name="ships">The list of enemy ships to which the movement pattern is applied.</param>
        /// <param name="movingRight">Indicates whether the ships are currently moving to the right.</param>
        /// <param name="movementSpeed">The speed at which the ships move.</param>
        /// <param name="maxMoveDistance">The maximum distance the ships can move horizontally before changing direction.</param>
        /// <param name="originalPositions">The original X-coordinates of the ships used to reset positions when necessary.</param>
        /// <param name="updatePosition">An action to update the position of a specific ship on the canvas.</param>
        /// <param name="handleSecondSprite">An action to handle updates for secondary sprites associated with the enemy ship (e.g., animations or effects).</param>
        void ApplyMovement(IList<EnemyShip> ships, bool movingRight, double movementSpeed, double maxMoveDistance, IList<double> originalPositions, Action<EnemyShip> updatePosition, Action<EnemyShip> handleSecondSprite);
    }
}
