using Galaga.Model;
using System;
using System.Collections.Generic;

namespace Galaga.Model.MovementPattern
{
    public class StraightPattern : IMovementPattern
    {
        public void ApplyMovement(IList<EnemyShip> ships, bool movingRight, double movementSpeed, double maxMoveDistance, IList<double> originalPositions, Action<EnemyShip> updatePosition, Action<EnemyShip> handleSecondSprite)
        {
            foreach (var ship in ships)
            {
                ship.X += movingRight ? movementSpeed : -movementSpeed;
                updatePosition(ship);
            }
        }
    }
}