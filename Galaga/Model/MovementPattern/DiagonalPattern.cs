using System;
using System.Collections.Generic;

namespace Galaga.Model.MovementPattern
{
    /// <summary>
    /// Defines a diagonal movement pattern for enemy ships.
    /// Ships move diagonally by combining horizontal and sinusoidal vertical movement.
    /// </summary>
    public class DiagonalPattern : IMovementPattern
    {
        /// <summary>
        /// Applies the diagonal movement pattern to a list of enemy ships.
        /// </summary>
        /// <param name="ships">The list of enemy ships to apply the movement pattern to.</param>
        /// <param name="movingRight">A boolean indicating whether the ships are moving to the right.</param>
        /// <param name="speed">The base horizontal movement speed of the ships.</param>
        /// <param name="maxDistance">The maximum horizontal distance the ships can travel before reversing direction.</param>
        /// <param name="originalPositions">The original X positions of the ships, used to calculate the sinusoidal movement.</param>
        /// <param name="updatePosition">An action that updates the position of a ship on the screen.</param>
        /// <param name="handleSecondSprite">An action that handles updates to a ship's second sprite, if applicable.</param>
        public void ApplyMovement(
            IList<EnemyShip> ships,
            bool movingRight,
            double speed,
            double maxDistance,
            IList<double> originalPositions,
            Action<EnemyShip> updatePosition,
            Action<EnemyShip> handleSecondSprite)
        {
            for (int i = 0; i < ships.Count; i++)
            {
                var ship = ships[i];

                ship.X += movingRight ? speed : -speed;

                double originalY = ship.BaseYPosition;
                double maxVerticalDisplacement = maxDistance / 2;

                ship.Y = originalY + Math.Sin((ship.X - originalPositions[i]) / maxDistance * Math.PI) * maxVerticalDisplacement;

                updatePosition(ship);

                handleSecondSprite(ship);
            }
        }
    }
}
