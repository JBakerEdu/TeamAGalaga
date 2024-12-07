using System;
using System.Collections.Generic;

namespace Galaga.Model.MovementPattern
{
    /// <summary>
    /// Defines an oscillating horizontal movement pattern for enemy ships.
    /// Ships move horizontally while oscillating up and down in a sinusoidal pattern.
    /// </summary>
    public class OscillatingHorizontalPattern : IMovementPattern
    {
        /// <summary>
        /// Controls the oscillation phase for the movement pattern.
        /// </summary>
        private double phase;

        /// <summary>
        /// Applies the oscillating horizontal movement pattern to a list of enemy ships.
        /// </summary>
        /// <param name="ships">The list of enemy ships to apply the movement pattern to.</param>
        /// <param name="movingRight">A boolean indicating whether the ships are moving to the right.</param>
        /// <param name="movementSpeed">The base horizontal movement speed of the ships.</param>
        /// <param name="maxMoveDistance">The maximum distance the ships can move horizontally before reversing direction.</param>
        /// <param name="originalPositions">The original positions of the ships, used to calculate offsets.</param>
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
            double horizontalDirection = movingRight ? 1 : -1;

            foreach (var ship in ships)
            {
                ship.X += horizontalDirection * movementSpeed;
                ship.X += Math.Sin(this.phase) * 5;
                updatePosition(ship);
                handleSecondSprite(ship);
            }
            this.phase += 0.1;
            if (this.phase > 2 * Math.PI)
            {
                this.phase -= 2 * Math.PI;
            }
        }
    }
}
