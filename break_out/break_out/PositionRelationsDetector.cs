using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace break_out
{
    [Flags]
    enum Relation
    {
        None = 0,
        Left = 1,
        Right = 2,
        Top = 4,
        Bottom = 8
    }

    static class PositionRelationsDetector
    {
        public static Relation GetRelation(Ball ball, Brick brick)
        {
            Relation relation = Relation.None;

            var brickMidX = brick.X + brick.Width / 2;
            var brickMidY = brick.Y + brick.Height / 2;

            if (ball.Y > brickMidY)
            {
                relation |= Relation.Bottom;
            }
            else if (ball.Y + ball.Radius * 2 < brickMidY)
            {
                relation |= Relation.Top;
            }
            else
            {
                if (ball.X > brickMidX)
                {
                    relation |= Relation.Right;
                }
                else if (ball.X + ball.Radius * 2 < brickMidY)
                {
                    relation |= Relation.Left;
                }
            }

            return relation;
        }
    }
}
