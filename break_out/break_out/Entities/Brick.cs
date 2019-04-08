using Microsoft.Xna.Framework.Graphics;

namespace break_out.Entities
{
    class Brick : Entity
    {
        public readonly int Width;
        public readonly int Height;

        public Brick(double X, double Y, int Width, int Height, Texture2D texture) : base(X, Y, texture)
        {
            this.Width = Width;
            this.Height = Height;
        }
    }
}