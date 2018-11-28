using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace break_out
{
    abstract class Entity
    {
        public double X { get; protected set; }
        public double Y { get; protected set; }

        public Entity(double X, double Y)
        {
            this.X = X;
            this.Y = Y;
        }
        public override string ToString()
        {
            return $"{this.GetType().Name} on coords : {{ X = {X}; Y = {Y} }}";
        }

    }

    class Brick : Entity
    {
        public readonly int Width;
        public readonly int Height;
        public Color Color = Color.Black;

        public Brick(double X, double Y, int Width, int Height, Color Color) : base(X, Y)
        {
            this.Width = Width;
            this.Height = Height;
            this.Color = Color;
        }
    }

    class Ball : Entity
    {
        public readonly int Radius;
        public double Vx { get; set; } = 0;
        public double Vy { get; set; } = 2.25;

        public Ball(int Radius, double X, double Y) : base(X, Y)
        {
            this.Radius = Radius;
        }

        public void Move()
        {
            this.X += Vx;
            this.Y += Vy;
        }
    }

    class Plate : Brick
    {
        public Plate(double X, double Y, int Width, int Height, Color Color) : base(X, Y, Width, Height, Color)
        {
        }

        public void ChangeCoords(double x)
        {
            this.X = x - (Width / 2);
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

        bool HaveWon
        {
            get
            {
                for (int i = 0; i < bricks.Length; i++)
                {
                    if (bricks[i] != null)
                        return false;
                }
                return true;
            }
            set
            {

            }
        }

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D BallTexture;
        Texture2D PlateTexture;
        Texture2D[] BrickTextures;
        Texture2D Menu;

        //SpriteFont spriteFont;

        Brick[] bricks;
        Ball ball;
        Plate plate;

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

        private void GenerateBricks(int Amount = 20, int Width = 90, int Height = 30)
        {
            Random rnd = new Random();

            bricks = new Brick[Amount];
            BrickTextures = new Texture2D[Amount];

            for (int i = 0; i < Amount; i++)
            {
                Color RandomColor = Color.FromNonPremultiplied(rnd.Next(1, 255), rnd.Next(1, 255), rnd.Next(1, 255), 255);
                bricks[i] = new Brick((i % 10) * Width, (i / 10) * Height, Width, Height, RandomColor);

                Color[] data = new Color[bricks[i].Width * bricks[i].Height];
                for (int j = 0; j < data.Length; j++)
                    data[j] = bricks[i].Color;

                BrickTextures[i] = new Texture2D(GraphicsDevice, bricks[i].Width, bricks[i].Height);
                BrickTextures[i].SetData(data);
            }
        }
        private void GenerateBall()
        {
            Random rnd = new Random();

            int Radius = 15;

            ball =
                new Ball(Radius,
                graphics.PreferredBackBufferWidth / 2 - Radius,
                graphics.PreferredBackBufferHeight / 2 - Radius);
        }
        private void GeneratePlate(int PlateWidth = 150, int PlateHeight = 6)
        {
            Random rnd = new Random();

            plate = new Plate(graphics.PreferredBackBufferWidth / 2 - (PlateWidth / 2),
                graphics.PreferredBackBufferHeight - PlateHeight, PlateWidth, PlateHeight, Color.White);
            PlateTexture = new Texture2D(GraphicsDevice, plate.Width, plate.Height);

            Color[] PlateData = new Color[plate.Width * plate.Height];

            for (int i = 0; i < PlateData.Length; ++i)
                PlateData[i] = plate.Color;

            PlateTexture.SetData(PlateData);
        }

        private List<int> GetBricksCollidingTheBallIndexes()
        {
            List<int> IndexesList = new List<int>(2);

            for (int i = 0; i < bricks.Length; i++)
            {
                if (bricks[i] != null && BrickToCircleCollison(bricks[i]))
                    IndexesList.Add(i);
            }
            return IndexesList;
        }
        private void BallHitsPlate()
        {
            // Checks whether the ball hits the plate or not. 2)
            // If not, then if its Y coord is <= plate's one, exits the game. 1) 
            // If yes, it reverses Vy Velocity and Gets the X speed of the ball depending on what part 3) 
            //                                                                  of the plate has been hit

            if (ball.Y + (ball.Radius * 2) >= plate.Y)
            {
                if (ball.X + ball.Radius * 2 >= plate.X && ball.X <= plate.X + plate.Width) // hits
                {
                    double scale = ball.X + ball.Radius - (plate.X + plate.Width / 2);
                    ball.Vx = (scale / Math.Sqrt(Math.Abs(scale))) / 4.5;
                    ball.Vy *= -1;
                }
                else
                    _state = GameState.Over;
            }
        }

        private bool BrickToCircleCollison(Brick brick)
        {
            double closeX = ball.X;
            double closeY = ball.Y;

            // Check left side of the Rect
            if (ball.X < brick.X) closeX = brick.X;
            // Right side of the rect
            if (ball.X > brick.X + brick.Width) closeX = brick.X + brick.Width;

            // Check top side of the Rect
            if (ball.Y < brick.Y) closeY = brick.Y;
            // Right bottom of the rect
            if (ball.Y > brick.Y + brick.Height) closeY = brick.Y + brick.Height;

            double distance = Math.Sqrt(
                (closeX - ball.X) * (closeX - ball.X)
                + (closeY - ball.Y) * (closeY - ball.Y));

            if (distance <= ball.Radius)
                return true;
            return false;
        }

        private void BallContactsWalls() // Check if the ball arrives at any of the walls, reverse Y-Velocity if does 
        {
            if ((ball.X + ball.Radius * 2 >= graphics.PreferredBackBufferWidth) || (ball.X <= 0))
                ball.Vx *= -1;
            if ((ball.Y + ball.Radius * 2 >= graphics.PreferredBackBufferHeight) || (ball.Y <= 0))
                ball.Vy *= -1;
        }

        protected override void Initialize()
        {
            GenerateBricks();
            GenerateBall();
            GeneratePlate();

            _state = GameState.Menu;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            BallTexture = Content.Load<Texture2D>("Ball");
            Menu = Content.Load<Texture2D>("PressEnterToContinue");            
            
        }

        protected override void UnloadContent()
        {
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
            List<int> Indexes = GetBricksCollidingTheBallIndexes();
            if (Indexes.Count != 0) // delete bricks
            {
                for (int i = 0; i < Indexes.Count; i++)
                {
                    bricks[Indexes[i]] = null;
                    BrickTextures[Indexes[i]] = null;
                }
                ball.Vy *= -1;
            }

            // Check if contacts Walls. Reverses vilocity if does.
            BallContactsWalls();

            // Check if ball arrives at the plate
            BallHitsPlate();

            if (HaveWon)
                _state = GameState.Over;
        }

        private void DrawMenu()
        {
            graphics.GraphicsDevice.Clear(Color.Gray);

            spriteBatch.Begin();

            spriteBatch.Draw(Menu, new Vector2(0, 0), Color.White);

            spriteBatch.End();
        }
        private void DrawOver()
        {
            graphics.GraphicsDevice.Clear(Color.Gray);

            spriteBatch.Begin();

            spriteBatch.Draw(Menu, new Vector2(0, 0), Color.White);

            spriteBatch.End();
        }
        private void DrawGame()
        {
            GraphicsDevice.Clear(Color.DodgerBlue);
            spriteBatch.Begin();

            Texture2D[] texturesToDraw = new Texture2D[bricks.Length + 2];

            BrickTextures.CopyTo(texturesToDraw, 0);
            texturesToDraw[BrickTextures.Length] = BallTexture;
            texturesToDraw[texturesToDraw.Length - 1] = PlateTexture;

            Entity[] entitiesBeingDrawn = new Entity[bricks.Length + 2];

            bricks.CopyTo(entitiesBeingDrawn, 0);
            entitiesBeingDrawn[bricks.Length] = ball;
            entitiesBeingDrawn[entitiesBeingDrawn.Length - 1] = plate;
            
            for (int i = 0; i < entitiesBeingDrawn.Length; i++)
            {
                if (entitiesBeingDrawn[i] != null)
                    spriteBatch.Draw(texturesToDraw[i], new Vector2((float)entitiesBeingDrawn[i].X, (float)entitiesBeingDrawn[i].Y),
                        null, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 1);
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
