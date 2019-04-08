using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace break_out.Entities
{
    abstract class Entity : IDisposable
    {
        private Texture2D _texture;

        public double X { get; protected set; }
        public double Y { get; protected set; }

        public Entity(double x, double y, Texture2D texture)
        {
            X = x;
            Y = y;
            _texture = texture;
        }

        public override string ToString()
        {
            return $"{this.GetType().Name} on coords : {{ X = {X}; Y = {Y} }}";
        }

        public void Draw(SpriteBatch batch)
        {
            batch.Draw(_texture, new Vector2((float)X, (float)Y),
                null, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 1);
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    X = 0;
                    Y = 0;
                }

                _texture.Dispose();
                _texture = null;

                disposedValue = true;
            }
        }
        ~Entity()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
