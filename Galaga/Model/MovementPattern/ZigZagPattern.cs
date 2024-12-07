using System;
using System.Collections.Generic;

namespace Galaga.Model.MovementPattern
{
    public class ZigZagPattern : IMovementPattern
    {
        public void ApplyMovement(
            IList<EnemyShip> ships,
            bool movingRight,
            double movementSpeed,
            double maxMoveDistance,
            IList<double> originalPositions,
            Action<EnemyShip> updatePosition,
            Action<EnemyShip> handleSecondSprite)
        {
            double amplitude = 20; // Adjust as needed for vertical movement range
            double frequency = 0.05; // Adjust for the speed of vertical oscillation

            for (int i = 0; i < ships.Count; i++)
            {
                var ship = ships[i];
                double originalX = originalPositions[i];
                double baseY = ship.BaseYPosition; // The original Y position of the ship

                // Update horizontal position
                ship.X += movingRight ? movementSpeed : -movementSpeed;

                // Calculate vertical position using sine wave, centered around baseY
                ship.Y = baseY + amplitude * Math.Sin(frequency * ship.X);

                // Update the ship's position in the canvas
                updatePosition(ship);
                handleSecondSprite(ship);
            }
        }
    }
}