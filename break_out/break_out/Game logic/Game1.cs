using System;
using System.Collections.Generic;
using break_out.Collision_Processing;
using break_out.Entities;
using break_out.Entity_Creating;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace break_out.Game_logic
{
    public class Game1 : Game
    {
        private const int radius = 15;

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
        
        /// <summary>
        /// Goes through the array of bricks and checks if they're all destroyed, the game is won if yes.
        /// </summary>
        /// <returns>A boolean value that indicates whether the game is won or not.</returns>
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

        /// <summary>
        /// Processes possible the-ball-to-a-brick collisions, and does appropriate velocity changes if they occur.
        /// </summary>
        private void ProcessPossibleBrickCollisions()
        {
            for (int i = 0; i < bricks.Length; i++)
            {
                if (bricks[i] != null)
                {
                    Position currentCollisionPosition = CollisionDetector.BrickToCircleIntersection(ball, bricks[i]);

                    if (currentCollisionPosition != Position.None)
                    {
                        PositionRelationBallVelocityManager
                            .ChangeVelocityAccordingToCollisionPosition(ball, currentCollisionPosition);

                        bricks[i].Dispose();
                        bricks[i] = null;
                    }
                }
            }
        }

        /// <summary>
        /// Processes possible the-ball-to-the-plate collision, and if it occurs, changes velocity of the ball.
        /// </summary>
        private void ProcessPossiblePlateCollision()
        {
            // Checks whether the ball hits the plate or not. 1)
            // If not, then if its Y coord is <= plate's one, exits the game. 2) 
            // If yes, it reverses Vy Velocity and Gets the X speed of the ball depending on what part 3) 
            //                                                                  of the plate has been hit

            if (ball.Y + (ball.Radius * 2) >= plate.Y)
            {
                if (CollisionDetector.BrickToCircleIntersection(ball, plate) != Position.None)
                {
                    double scale = ball.X + ball.Radius - (plate.X + plate.Width / 2);
                    ball.Vx = (scale / Math.Sqrt(Math.Abs(scale))) / 4.5;
                    ball.Vy *= -1;
                }
                else if (ball.Y >= graphics.PreferredBackBufferHeight)
                    EndGame();
            }
        }

        /// <summary>
        /// Processes possible the-ball-to-a-wall collision, and if it occurs, changes velocity of the ball.
        /// </summary>
        private void ProcessPossibleWallCollisions()
        {
            PositionRelationBallVelocityManager.BallWallsContact(ball, graphics);
        }

        /// <summary>
        /// Cleans up all resources that the current game has been holding before switching to another gamestate
        /// </summary>
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

        /// <summary>
        /// Initializes new game i.e creates game objects, resets game fields (fields in the class Game1).
        /// </summary>
        protected override void Initialize()
        {
            _won = false;
            _endGameCalled = false;

            bricks = ShapeGenerator.GenerateBricks(GraphicsDevice);

            ball = ShapeGenerator.GenerateBall(GraphicsDevice, graphics, radius);

            plate = ShapeGenerator.GeneratePlate(GraphicsDevice, graphics);

            _state = GameState.Menu;

            base.Initialize();
        }

        /// <summary>
        /// Loads spriteFont used to display text, initializes spriteBatch
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            spriteFont = Content.Load<SpriteFont>("File");

            base.LoadContent();
        }

        /// <summary>
        /// Unloads game content (spriteFonts, spriteBatch etc.)
        /// </summary>
        protected override void UnloadContent() 
        {
            Content.Unload();

            base.UnloadContent();
        }

        /// <summary>
        /// Checks if the player has hit the enter button, and if they did, starts the game.
        /// </summary>
        private void UpdateMenu()
        {
            foreach (var i in Keyboard.GetState().GetPressedKeys())
                if (i == Keys.Enter)
                    _state = GameState.Game;
        }

        /// <summary>
        /// Checks if the player has hit the enter button, and if they did, restarts the game.
        /// </summary>
        private void UpdateOver()
        {
            foreach (var i in Keyboard.GetState().GetPressedKeys())
                if (i == Keys.Enter)
                {
                    Initialize();
                    _state = GameState.Game;
                }
        }

        /// <summary>
        /// Gamestate game-logic loop.
        /// <para>1. Reads mouse input from the player and changes coords of the plate</para>
        /// <para>2. Moves the ball</para>
        /// <para>3. Processes all possible collisions</para>
        /// <para>4. Checks if the game is won, and ends the game if yes</para>
        /// </summary>
        private void UpdateGame()
        {
            plate.ChangeCoords(Mouse.GetState().X);

            ball.Move();

            // Check if ball arrives at any of the remaining bricks

            ProcessPossibleBrickCollisions();

            // Check if contacts Walls. Reverses vilocity if does.
            ProcessPossibleWallCollisions();

            // Check if ball arrives at the plate
            ProcessPossiblePlateCollision();

            if (_won = CheckIfWon())
                EndGame();
        }

        /// <summary>
        /// Draws text to menu.
        /// </summary>
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
       
        /// <summary>
        /// Draws text to menu with info about the previous game result
        /// </summary>
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

        /// <summary>
        /// Draws all entities that are present.
        /// </summary>
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

        /// <summary>
        /// Calls corresponding to the current gamestate Draw method. (DrawGame, DrawMenu or DrawOver) 
        /// </summary>
        /// <param name="gameTime"></param>
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

        /// <summary>
        /// Calls coresponding to the current gamestate Update method. (UpdateGame, UpdateMenu or UpdateOver)
        /// </summary>
        /// <param name="gameTime"></param>
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
