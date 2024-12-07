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
            double amplitude = 20;
            double frequency = 0.05;

            for (int i = 0; i < ships.Count; i++)
            {
                var ship = ships[i];
                double originalX = originalPositions[i];
                double baseY = ship.BaseYPosition;

                ship.X += movingRight ? movementSpeed : -movementSpeed;

                ship.Y = baseY + amplitude * Math.Sin(frequency * ship.X);

                updatePosition(ship);
                handleSecondSprite(ship);
            }
        }
    }
}