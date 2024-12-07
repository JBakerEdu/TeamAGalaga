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
            double horizontalDirection = movingRight ? 1 : -1;

            foreach (var ship in ships)
            {
                ship.X += horizontalDirection * movementSpeed;

                ship.X += Math.Sin(phase) * 5;

                updatePosition(ship);
            }

            phase += 0.1;
            if (phase > 2 * Math.PI) phase -= 2 * Math.PI;
        }
    }
}


