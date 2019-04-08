using Microsoft.Xna.Framework.Graphics;

namespace break_out.Entities
{
    class Ball : Entity
    {
        public int Radius { get; }
        public double Vx { get; set; } = 0;
        public double Vy { get; set; } = 2.25;

        public Ball(int radius, double x, double y, Texture2D texture) : base(x, y, texture)
        {
            Radius = radius;
        }

        public void Move()
        {
            this.X += Vx;
            this.Y += Vy;
        }
    }
}