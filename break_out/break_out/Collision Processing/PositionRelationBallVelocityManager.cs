using break_out.Entities;
using Microsoft.Xna.Framework;

namespace break_out.Collision_Processing
{
    static class PositionRelationBallVelocityManager
    {
        public static void ChangeVelocityAccordingToCollisionPosition(Ball ball, Position position)
        {
            if (position == Position.Left || position == Position.Right)
                ball.Vx *= -1;
            else if (position == Position.Top || position == Position.Bottom)
                ball.Vy *= -1;
            else if (position == Position.LeftBottom ||
                     position == Position.RightBottom ||
                     position == Position.LeftTop ||
                     position == Position.RightTop)
            {
                ball.Vy *= -1;
                ball.Vx *= -1;
            }
        }
   
        /// Check if the ball arrives at any of the walls that aren't the bottom wall
        /// If arrives, reverse the corresponding velocity
        /// 
        public static void BallWallsContact(Ball ball, GraphicsDeviceManager graphics)
        {
            if ((ball.X + ball.Radius * 2 >= graphics.PreferredBackBufferWidth) || (ball.X <= 0))
                ball.Vx *= -1;

            if (ball.Y <= 0)
                ball.Vy *= -1;
        }
    }
}
