using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace break_out.Entity_Creating
{
    static class TextureCreator
    {
        private static readonly Random rnd = new Random();

        public static Texture2D CreateCircle(int radius, GraphicsDevice device, Color color)
        {
            Texture2D texture = new Texture2D(device, radius, radius);
            Color[] colorData = new Color[radius * radius];

            float diam = radius / 2f;
            float diamsq = diam * diam;

            for (int x = 0; x < radius; x++)
            {
                for (int y = 0; y < radius; y++)
                {
                    int index = x * radius + y;
                    Vector2 pos = new Vector2(x - diam, y - diam);
                    if (pos.LengthSquared() <= diamsq)
                    {
                        colorData[index] = color;
                    }
                    else
                    {
                        colorData[index] = Color.Transparent;
                    }
                }
            }

            texture.SetData(colorData);
            return texture;
        }

        public static Texture2D CreateRectangle(GraphicsDevice device, int Width, int Height)
        {
            Color RandomColor = Color.FromNonPremultiplied(rnd.Next(1, 255), rnd.Next(1, 255), rnd.Next(1, 255), 255);

            Color[] data = new Color[Width * Height];
            for (int j = 0; j < data.Length; j++)
                data[j] = RandomColor;

            var texture = new Texture2D(device, Width, Height);
            texture.SetData(data);

            return texture;
        }
    }
}
