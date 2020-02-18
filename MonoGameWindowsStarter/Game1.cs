using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Audio;

namespace MonoGameWindowsStarter
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;
        SpriteSheet sheet;
        Player monster;
        Sprite fruitSprite;
        Fruit fruit;
        int score = 0;
        int lives = 3;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            // Set up size
            graphics.PreferredBackBufferWidth = 1042;
            graphics.PreferredBackBufferHeight = 768;
            graphics.ApplyChanges();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            // Load Sprite Sheet into object
            var t = Content.Load<Texture2D>("spritesheet");
            sheet = new SpriteSheet(t, 50, 50, 0, 0);

            // Create player and frames for animation
            var playerFrames = from index in Enumerable.Range(0, 4) select sheet[index];
            SoundEffect eat = Content.Load<SoundEffect>("eat");
            monster = new Player(playerFrames, eat);

            var fruitTexture = Content.Load<Texture2D>("fruit");
            fruitSprite = new Sprite(new Rectangle(0, 0, 137, 131), fruitTexture);
            SoundEffect fail = Content.Load<SoundEffect>("fail");
            fruit = new Fruit(fruitSprite, fail);

            // Load font
            font = Content.Load<SpriteFont>("font");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            // Update Monster frames
            monster.Update(gameTime);

            // Update fruit frames
            fruit.Update(gameTime);

            if (monster.CollidedWithFruit(fruit))
            {
                fruit.spawnFruitToTop();
                score++;
                monster.playEatSoundEffect();
            }

            if (fruit.collidedWithBounds())
            {
                lives--;
                if (lives <= 0) Exit();
                fruit.spawnFruitToTop();
                fruit.playFailSoundEffect();
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            // Draw UI
            spriteBatch.DrawString(font, "Score: " + score, new Vector2(0, 0), Color.Black);
            spriteBatch.DrawString(font, "Lives: " + lives, new Vector2(0, 50), Color.Black);
            // Draw monster
            monster.Draw(spriteBatch);
            // Draw fruit
            fruit.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
