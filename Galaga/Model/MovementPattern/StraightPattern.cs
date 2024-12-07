using System;
using System.Collections.Generic;

namespace Galaga.Model.MovementPattern
{
    /// <summary>
    /// Defines a straight-line horizontal movement pattern for enemy ships.
    /// Ships move horizontally in a straight line without vertical displacement.
    /// </summary>
    public class StraightPattern : IMovementPattern
    {
        /// <summary>
        /// Applies the straight-line horizontal movement pattern to a list of enemy ships.
        /// </summary>
        /// <param name="ships">The list of enemy ships to apply the movement pattern to.</param>
        /// <param name="movingRight">A boolean indicating whether the ships are moving to the right.</param>
        /// <param name="movementSpeed">The horizontal movement speed of the ships.</param>
        /// <param name="maxMoveDistance">The maximum distance the ships can travel horizontally before reversing direction.</param>
        /// <param name="originalPositions">The original X positions of the ships (not used in this pattern).</param>
        /// <param name="updatePosition">An action that updates the position of a ship on the screen.</param>
        /// <param name="handleSecondSprite">An action that handles updates to a ship's second sprite, if applicable (not used in this pattern).</param>
        public void ApplyMovement(
            IList<EnemyShip> ships,
            bool movingRight,
            double movementSpeed,
            double maxMoveDistance,
            IList<double> originalPositions,
            Action<EnemyShip> updatePosition,
            Action<EnemyShip> handleSecondSprite)
        {
            foreach (var ship in ships)
            {
                // Apply horizontal movement based on direction
                ship.X += movingRight ? movementSpeed : -movementSpeed;

                // Update the ship's position
                updatePosition(ship);
            }
        }
    }
}