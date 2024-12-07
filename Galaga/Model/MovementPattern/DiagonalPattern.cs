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
