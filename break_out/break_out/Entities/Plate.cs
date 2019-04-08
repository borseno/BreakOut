using Microsoft.Xna.Framework.Graphics;

namespace break_out.Entities
{
    class Plate : Brick
    {
        public Plate(double X, double Y, int Width, int Height, Texture2D texture)
            : base(X, Y, Width, Height, texture)
        {
        }

        public void ChangeCoords(double x)
        {
            this.X = x - (Width / 2); // change not right edge point position to x, but the mid point to x
        }
    }
}