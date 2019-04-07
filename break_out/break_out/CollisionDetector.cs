using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace break_out
{
    static class CollisionDetector
    {
        public static bool Intersects(Ball c, Brick r)
        {
            float circleX = (float)c.X + c.Radius;
            float circleY = (float)c.Y + c.Radius;

            float cx = (float)Math.Abs(circleX - r.X - r.Width / 2);
            float xDist = r.Width / 2 + c.Radius;
            if (cx > xDist)
                return false;
            float cy = (float)Math.Abs(circleY - r.Y - r.Height / 2);
            float yDist = r.Height / 2 + c.Radius;
            if (cy > yDist)
                return false;
            if (cx <= r.Width / 2 || cy <= r.Height / 2)
                return true;
            float xCornerDist = cx - r.Width / 2;
            float yCornerDist = cy - r.Height / 2;
            float xCornerDistSq = xCornerDist * xCornerDist;
            float yCornerDistSq = yCornerDist * yCornerDist;
            float maxCornerDistSq = c.Radius * c.Radius;
            return xCornerDistSq + yCornerDistSq <= maxCornerDistSq;
        }
    }
}
