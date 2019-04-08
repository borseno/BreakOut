using break_out.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace break_out.Entity_Creating
{
    class ShapeGenerator
    {
        public static Brick[] GenerateBricks(GraphicsDevice device, int Amount = 20, int Width = 90, int Height = 30)
        {          
            var bricks = new Brick[Amount];

            // init brick textures
            for (int i = 0; i < Amount; i++)
            {
                var texture = TextureCreator.CreateRectangle(device, Width, Height);

                bricks[i] = new Brick((i % 10) * Width, (i / 10) * Height, Width, Height, texture);
            }

            return bricks;
        }
        public static Ball GenerateBall(GraphicsDevice device, GraphicsDeviceManager graphics, int radius)
        {        
            var texture = TextureCreator.CreateCircle(radius * 2, device, Color.Black);

            var ball =
                new Ball(radius,
                    graphics.PreferredBackBufferWidth / 2 - radius,
                    graphics.PreferredBackBufferHeight / 2 - radius, texture);

            return ball;
        }
        public static Plate GeneratePlate(GraphicsDevice device, GraphicsDeviceManager graphics, int PlateWidth = 150, int PlateHeight = 6)
        {
            var texture = new Texture2D(device, PlateWidth, PlateHeight);

            Color[] PlateData = new Color[PlateWidth * PlateHeight];

            for (int i = 0; i < PlateData.Length; ++i)
                PlateData[i] = Color.White;

            texture.SetData(PlateData);

            var plate = new Plate(
                graphics.PreferredBackBufferWidth / 2 - (PlateWidth / 2),
                graphics.PreferredBackBufferHeight - PlateHeight,
                PlateWidth,
                PlateHeight,
                texture);

            return plate;
        }
    }
}
