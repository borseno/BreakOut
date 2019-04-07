using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace break_out
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


    public class Game1 : Game
    {
        enum GameState
        {
            Menu,
            Game,
            Over
        }
        GameState _state;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        SpriteFont spriteFont;

        Brick[] bricks;
        Ball ball;
        Plate plate;

        private bool _won;
        private bool _endGameCalled;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferHeight = 900,
                PreferredBackBufferWidth = 900
            };

            TargetElapsedTime = new TimeSpan(0, 0, 0, 0, 6);

            Window.Title = "Break Out";

            Content.RootDirectory = "Content";
        }

        private bool CheckIfWon()
        {
            if (bricks == null)
                return false;

            for (int i = 0; i < bricks.Length; i++)
            {
                if (bricks[i] != null)
                    return false;
            }
            return true;
        }

        private void GenerateBricks(int Amount = 20, int Width = 90, int Height = 30)
        {
            Random rnd = new Random();

            bricks = new Brick[Amount];

            // init brick textures
            for (int i = 0; i < Amount; i++)
            {
                Color RandomColor = Color.FromNonPremultiplied(rnd.Next(1, 255), rnd.Next(1, 255), rnd.Next(1, 255), 255);

                Color[] data = new Color[Width * Height];
                for (int j = 0; j < data.Length; j++)
                    data[j] = RandomColor;

                var texture = new Texture2D(GraphicsDevice, Width, Height);
                texture.SetData(data);

                bricks[i] = new Brick((i % 10) * Width, (i / 10) * Height, Width, Height, texture);
            }
        }
        private void GenerateBall()
        {
            const int radius = 15;

            var texture = ShapeCreator.CreateCircle(radius * 2, GraphicsDevice, Color.Black);

            ball =
                new Ball(radius,
                graphics.PreferredBackBufferWidth / 2 - radius,
                graphics.PreferredBackBufferHeight / 2 - radius, texture);
        }
        private void GeneratePlate(int PlateWidth = 150, int PlateHeight = 6)
        {
            var texture = new Texture2D(GraphicsDevice, PlateWidth, PlateHeight);

            Color[] PlateData = new Color[PlateWidth * PlateHeight];

            for (int i = 0; i < PlateData.Length; ++i)
                PlateData[i] = Color.White;

            texture.SetData(PlateData);

            plate = new Plate(
                graphics.PreferredBackBufferWidth / 2 - (PlateWidth / 2),
                graphics.PreferredBackBufferHeight - PlateHeight,
                PlateWidth,
                PlateHeight,
                texture);
        }

        private Dictionary<int, Brick> GetCollidingBricksIndexes()
        {
            var result = new Dictionary<int, Brick>(bricks.Length);

            for (int i = 0; i < bricks.Length; i++)
            {
                if (bricks[i] != null && BrickToCircleCollison(bricks[i]))
                    result.Add(i, bricks[i]);
            }

            return result;
        }

        private void BallHitsPlate()
        {
            // Checks whether the ball hits the plate or not. 1)
            // If not, then if its Y coord is <= plate's one, exits the game. 2) 
            // If yes, it reverses Vy Velocity and Gets the X speed of the ball depending on what part 3) 
            //                                                                  of the plate has been hit

            if (ball.Y + (ball.Radius * 2) >= plate.Y)
            {
                if (BrickToCircleCollison(plate))
                {
                    double scale = ball.X + ball.Radius - (plate.X + plate.Width / 2);
                    ball.Vx = (scale / Math.Sqrt(Math.Abs(scale))) / 4.5;
                    ball.Vy *= -1;
                }
                else if (ball.Y >= graphics.PreferredBackBufferHeight)
                    EndGame();
            }
        }
        private void EndGame()
        {
            if (_endGameCalled)
                return;

            for (int i = 0; i < bricks.Length; i++)
            {
                bricks[i]?.Dispose();
            }

            bricks = null;

            ball.Dispose();
            ball = null;

            plate.Dispose();
            plate = null;

            _state = GameState.Over;
            _endGameCalled = true;
        }

        private bool BrickToCircleCollison(Brick brick)
        {
            return CollisionDetector.Intersects(ball, brick);
        }

        private bool BallContactsWalls() // Check if the ball arrives at any of the walls, reverse Velocity if does 
        {
            bool result = false;

            if ((ball.X + ball.Radius * 2 >= graphics.PreferredBackBufferWidth) || (ball.X <= 0))
            {
                ball.Vx *= -1;
                result = true;
            }

            if (ball.Y <= 0)
            {
                ball.Vy *= -1;
                result = true;
            }

            return result;
        }

        protected override void Initialize()
        {
            _won = false;
            _endGameCalled = false;

            GenerateBricks();

            GenerateBall();

            GeneratePlate();

            _state = GameState.Menu;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            spriteFont = Content.Load<SpriteFont>("File");

            base.LoadContent();
        }
        protected override void UnloadContent() // !!!
        {
            Content.Unload();

            base.UnloadContent();
        }

        private void UpdateMenu()
        {
            foreach (var i in Keyboard.GetState().GetPressedKeys())
                if (i == Keys.Enter)
                    _state = GameState.Game;
        }
        private void UpdateOver()
        {
            foreach (var i in Keyboard.GetState().GetPressedKeys())
                if (i == Keys.Enter)
                {
                    Initialize();
                    _state = GameState.Game;
                }
        }
        private void UpdateGame()
        {
            plate.ChangeCoords(Mouse.GetState().X);

            ball.Move();

            // Check if ball arrives at any of the remaining bricks
            var bricksAndIndexes = GetCollidingBricksIndexes();
            if (bricksAndIndexes.Count > 0) // delete bricks
            {
                PositionRelationBallVelocityManager.ReverseVelocityAccordingToPositionRelation(ball, bricksAndIndexes.Values);

                foreach (var i in bricksAndIndexes)
                {
                    bricks[i.Key].Dispose();
                    bricks[i.Key] = null;
                }
            }

            // Check if contacts Walls. Reverses vilocity if does.
            BallContactsWalls();

            // Check if ball arrives at the plate
            BallHitsPlate();

            if (_won = CheckIfWon())
                EndGame();
        }

        private void DrawMenu()
        {
            graphics.GraphicsDevice.Clear(Color.Gray);

            spriteBatch.Begin();

            spriteBatch.DrawString(
                spriteFont,
                "Press enter to start the game",
                new Vector2(graphics.PreferredBackBufferWidth / 2 - 100, graphics.PreferredBackBufferHeight / 2),
                Color.Black
                );

            spriteBatch.End();
        }
        private void DrawOver()
        {
            graphics.GraphicsDevice.Clear(Color.Gray);

            spriteBatch.Begin();

            spriteBatch.DrawString(
                spriteFont,
                $"You've {(_won ? "won" : "lost")}. Press enter to start the game",
                new Vector2(graphics.PreferredBackBufferWidth / 2 - 100, graphics.PreferredBackBufferHeight / 2),
                Color.Black
            );

            spriteBatch.End();
        }
        private void DrawGame()
        {
            GraphicsDevice.Clear(Color.DodgerBlue);

            spriteBatch.Begin();

            var entities = new List<Entity>(bricks) { ball, plate };

            foreach (var i in entities)
            {
                i?.Draw(spriteBatch);
            }

            spriteBatch.End();
        }

        protected override void Draw(GameTime gameTime)
        {
            switch (_state)
            {
                case GameState.Menu:
                    DrawMenu();
                    break;
                case GameState.Over:
                    DrawOver();
                    break;
                case GameState.Game:
                    DrawGame();
                    break;
            }
            base.Draw(gameTime);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            switch (_state)
            {
                case GameState.Game:
                    UpdateGame();
                    break;
                case GameState.Menu:
                    UpdateMenu();
                    break;
                case GameState.Over:
                    UpdateOver();
                    break;
            }
            base.Update(gameTime);
        }

    }
}
