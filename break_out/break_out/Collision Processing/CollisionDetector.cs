using System;
using break_out.Entities;

namespace break_out.Collision_Processing
{
    static class CollisionDetector
    {
        /// <summary>
        /// Checks if the circle intersects the brick.
        /// </summary>
        /// <param name="ball">ball to check for intersection with the brick</param>
        /// <param name="brick">brick to check for intersection with the ball</param>
        /// <returns>position of type Position that says where the intersection occured</returns>
        public static Position BrickToCircleIntersection(Ball ball, Brick brick)
        {
            Position positionHorizontal = Position.None;
            Position positionVertical = Position.None;

            double distanceY = 0;
            double distanceX = 0;

            double ballX = ball.X + ball.Radius;
            double ballY = ball.Y + ball.Radius;

            double closeX = ballX;
            double closeY = ballY;
            // Check left side of the Rect
            if (ballX < brick.X)
            {
                closeX = brick.X;
                positionHorizontal = Position.Left;
                distanceX = Math.Abs(ball.X - brick.X);
            }
            // Right side of the rect
            if (ballX > brick.X + brick.Width)
            {
                closeX = brick.X + brick.Width;
                positionHorizontal = Position.Right;
                distanceX = Math.Abs(ball.X - (brick.X + brick.Width));
            }

            // Check top side of the Rect
            if (ballY < brick.Y)
            {
                closeY = brick.Y;
                positionVertical = Position.Top;
                distanceY = Math.Abs(ballY - brick.Y);
            }
            // Right bottom of the rect
            if (ballY > brick.Y + brick.Height)
            {
                closeY = brick.Y + brick.Height;
                positionVertical = Position.Bottom;
                distanceY = Math.Abs(ballY - (brick.Y + brick.Height));
            }

            Position result = distanceX > distanceY ? positionHorizontal :
                distanceY > distanceX ? positionVertical : positionHorizontal | positionVertical;

            double distance = Math.Sqrt(
                (closeX - ballX) * (closeX - ballX) +
                (closeY - ballY) * (closeY - ballY)
                );

            if (distance <= ball.Radius)
                return result;
            return Position.None;
        }
    }
}
