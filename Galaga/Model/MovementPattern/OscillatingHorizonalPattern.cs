using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galaga.Model.MovementPattern
{
    public class OscillatingHorizontalPattern : IMovementPattern
    {
        private double phase = 0; // Controls the oscillation phase

        public void ApplyMovement(
            IList<EnemyShip> ships,
            bool movingRight,
            double movementSpeed,
            double maxMoveDistance,
            IList<double> originalPositions,
            Action<EnemyShip> updatePosition,
            Action<EnemyShip> handleSecondSprite)
        {
            // Determine the direction of horizontal movement
            double horizontalDirection = movingRight ? 1 : -1;

            // Update each ship's position
            foreach (var ship in ships)
            {
                // Move horizontally across the screen
                ship.X += horizontalDirection * movementSpeed;

                // Add oscillating motion
                ship.X += Math.Sin(phase) * 5; // Oscillates by ±5 units

                // Update the ship's position on the canvas
                updatePosition(ship);
            }

            // Increment the phase for smooth oscillation
            phase += 0.1;
            if (phase > 2 * Math.PI) phase -= 2 * Math.PI; // Reset phase to prevent overflow
        }
    }
}


