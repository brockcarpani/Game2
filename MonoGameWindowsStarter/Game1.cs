using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Audio;
using System;
using GameLibrary;

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
        Fruit fruit2;
        int score = 0;
        int lives = 3;
        Random rand = new Random();
        List<Platform> platforms;
        AxisList world;
        Text textPositions;
        Heart heart;
        ParticleSystem particleSystem;
        Texture2D particleTexture;
        Vector2 offset;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            platforms = new List<Platform>();
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

#if VISUAL_DEBUG
            VisualDebugging.LoadContent(Content);
#endif

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
            fruit = new Fruit(fruitSprite, fail, rand);
            fruit2 = new Fruit(fruitSprite, fail, rand);

            // Load font
            font = Content.Load<SpriteFont>("font");

            // Create the platforms
            Texture2D pixel = Content.Load<Texture2D>("pixel");
            Sprite pix = new Sprite(new Rectangle(0, 0, 100, 25), pixel);
            platforms.Add(new Platform(new BoundingRectangle(80, 700, 200, 25), pix));
            platforms.Add(new Platform(new BoundingRectangle(-520, 760, 3500, 10), pix));
            platforms.Add(new Platform(new BoundingRectangle(500, 720, 100, 25), pix));
            platforms.Add(new Platform(new BoundingRectangle(800, 720, 100, 25), pix));
            platforms.Add(new Platform(new BoundingRectangle(1300, 710, 100, 25), pix));
            platforms.Add(new Platform(new BoundingRectangle(1700, 720, 100, 25), pix));
            platforms.Add(new Platform(new BoundingRectangle(2000, 740, 100, 25), pix));

            // Add the platforms to the axis list
            world = new AxisList();
            foreach (Platform platform in platforms)
            {
                world.AddGameObject(platform);
            }

            // Load from text custom content pipeline
            textPositions = Content.Load<Text>("Positions");

            // Use those positions to init heart
            var heartTexture = Content.Load<Texture2D>("heart");
            Sprite heartSprite = new Sprite(new Rectangle(0, 0, 640, 640), heartTexture);
            heart = new Heart(heartSprite, rand, textPositions.Positions);

            // TODO: use this.Content to load your game content here
            particleTexture = Content.Load<Texture2D>("Particle");
            particleSystem = new ParticleSystem(this.GraphicsDevice, 1000, particleTexture);
            particleSystem.Emitter = new Vector2(monster.Position.X, monster.Bounds.Y - monster.Bounds.Height);
            particleSystem.SpawnPerFrame = 4;

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
            fruit2.Update(gameTime);

            heart.Update(gameTime);

            // Check for platform collisions
            var platformQuery = world.QueryRange(monster.Bounds.X, monster.Bounds.X + monster.Bounds.Width);
            monster.CheckForPlatformCollision(platformQuery);

            if (monster.CollidedWithFruit(fruit))
            {
                fruit.spawnFruitToTop();
                score++;
                monster.playEatSoundEffect();
            }
            if (monster.CollidedWithFruit(fruit2))
            {
                fruit2.spawnFruitToTop();
                score++;
                monster.playEatSoundEffect();
            }

            if (monster.CollidedWithHeart(heart))
            {
                lives++;
                heart.spawnNewHeart();
            }

            if (fruit.collidedWithBounds())
            {
                lives--;
                if (lives <= 0) Exit();
                fruit.spawnFruitToTop();
                fruit.playFailSoundEffect();
            }
            if (fruit2.collidedWithBounds())
            {
                lives--;
                if (lives <= 0) Exit();
                fruit2.spawnFruitToTop();
                fruit2.playFailSoundEffect();
            }

            // TODO: Add your update logic here
            particleSystem.Update(gameTime);
            particleSystem.Emitter = new Vector2(monster.Bounds.X + offset.X + monster.Bounds.Width / 2, monster.Bounds.Y + monster.Bounds.Height / 2);

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
            // Calculate and apply the world/view transform
            offset = new Vector2(graphics.GraphicsDevice.Viewport.Width / 2, monster.Position.Y) - monster.Position;
            var t = Matrix.CreateTranslation(offset.X, offset.Y, 0);
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, t);

            // Draw UI
            spriteBatch.DrawString(font, "Score: " + score, new Vector2(monster.Position.X - 500, 0), Color.Black);
            spriteBatch.DrawString(font, "Lives: " + lives, new Vector2(monster.Position.X - 500 , 50), Color.Black);
            // Draw monster
            monster.Draw(spriteBatch);
            // Draw fruit
            fruit.Draw(spriteBatch);
            fruit2.Draw(spriteBatch);
            // Draw the platforms 
            platforms.ForEach(platform =>
            {
                platform.Draw(spriteBatch);
            });

            // Draw hearts
            heart.Draw(spriteBatch);

            spriteBatch.End();

            // TODO: Add your drawing code here
            particleSystem.Draw();

            base.Draw(gameTime);
        }
    }
}
