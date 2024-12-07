using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galaga.Model.MovementPattern
{
    public class DiagonalPattern : IMovementPattern
    {
        public void ApplyMovement(IList<EnemyShip> ships, bool movingRight, double speed, double maxDistance,
            IList<double> originalPositions, Action<EnemyShip> updatePosition, Action<EnemyShip> handleSecondSprite)
        {
            for (int i = 0; i < ships.Count; i++)
            {
                var ship = ships[i];

                // Move X based on the direction
                ship.X += movingRight ? speed : -speed;

                // Calculate the original Y position for the ship
                double originalY = ship.BaseYPosition;

                // Keep the ship oscillating around the original row height
                double maxVerticalDisplacement = maxDistance / 2; // Limit vertical movement
                ship.Y = originalY + Math.Sin((ship.X - originalPositions[i]) / maxDistance * Math.PI) * maxVerticalDisplacement;

                // Update the ship's position and handle sprite behavior
                updatePosition(ship);
                handleSecondSprite(ship);
            }
        }
    }

}
