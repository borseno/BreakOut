using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;

namespace break_out
{
    static class PositionRelationBallVelocityManager
    {
        public static void ReverseVelocityAccordingToPositionRelation(Ball ball, IEnumerable<Brick> bricks)
        {
            var positions = bricks.Select(t => PositionRelationsDetector.GetRelation(ball, t)).Distinct();

            foreach (var position in positions)
            {
                if (position.HasFlag(Relation.Left) || position.HasFlag(Relation.Right))
                {
                    ball.Vx *= -1;
                }

                if (position.HasFlag(Relation.Top) || position.HasFlag(Relation.Bottom))
                {
                    ball.Vy *= -1;
                }
            }
        }
    }
}
