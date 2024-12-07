using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galaga.Model
{
    public interface IMovementPattern
    {
        void ApplyMovement(IList<EnemyShip> ships, bool movingRight, double movementSpeed, double maxMoveDistance, IList<double> originalPositions, Action<EnemyShip> updatePosition, Action<EnemyShip> handleSecondSprite);
    }
}
