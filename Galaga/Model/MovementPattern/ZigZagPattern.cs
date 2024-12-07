using System;
using System.Collections.Generic;

namespace Galaga.Model.MovementPattern
{
    /// <summary>
    /// Defines a zigzag movement pattern for enemy ships.
    /// Ships move horizontally while oscillating vertically in a sinusoidal pattern.
    /// </summary>
    public class ZigZagPattern : IMovementPattern
    {
        /// <summary>
        /// Applies the zigzag movement pattern to a list of enemy ships.
        /// </summary>
        /// <param name="ships">The list of enemy ships to apply the movement pattern to.</param>
        /// <param name="movingRight">A boolean indicating whether the ships are moving to the right.</param>
        /// <param name="movementSpeed">The base horizontal movement speed of the ships.</param>
        /// <param name="maxMoveDistance">The maximum horizontal distance the ships can travel before reversing direction.</param>
        /// <param name="originalPositions">The original X positions of the ships, used to calculate oscillations.</param>
        /// <param name="updatePosition">An action that updates the position of a ship on the screen.</param>
        /// <param name="handleSecondSprite">An action that handles updates to a ship's second sprite, if applicable.</param>
        public void ApplyMovement(
            IList<EnemyShip> ships,
            bool movingRight,
            double movementSpeed,
            double maxMoveDistance,
            IList<double> originalPositions,
            Action<EnemyShip> updatePosition,
            Action<EnemyShip> handleSecondSprite)
        {
            double amplitude = 20;
            var frequency = 0.05;

            foreach (var ship in ships)
            {
                var baseY = ship.BaseYPosition;
                ship.X += movingRight ? movementSpeed : -movementSpeed;
                ship.Y = baseY + amplitude * Math.Sin(frequency * ship.X);
                updatePosition(ship);
                handleSecondSprite(ship);
            }
        }
    }
}
