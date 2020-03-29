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
        ParticleSystem heartParticleSystem;
        ParticleSystem fruitParticleSystem;
        Vector2 offset;
        Texture2D heartTexture;

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
            heartTexture = Content.Load<Texture2D>("heart");
            Sprite heartSprite = new Sprite(new Rectangle(0, 0, 640, 640), heartTexture);
            heart = new Heart(heartSprite, rand, textPositions.Positions);

            // Load particles
            monsterParticleLoad();
            heartParticleLoad();
            fruitParticleLoad();

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
            heartParticleSystem.Update(gameTime);
            heartParticleSystem.Emitter = new Vector2(heart.Position.X + offset.X, heart.Position.Y);
            fruitParticleSystem.Update(gameTime);
            fruitParticleSystem.Emitter = new Vector2(fruit.Position.X + offset.X + 16, fruit.Position.Y);

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
            heartParticleSystem.Draw();
            fruitParticleSystem.Draw();

            base.Draw(gameTime);
        }

        // Load monster particles
        public void monsterParticleLoad()
        {
            // use this.Content to load your game content here
            particleTexture = Content.Load<Texture2D>("Particle");
            particleSystem = new ParticleSystem(this.GraphicsDevice, 1000, particleTexture);
            particleSystem.SpawnPerFrame = 4;
            // Set the SpawnParticle method
            particleSystem.SpawnParticle = (ref Particle particle) =>
            {
                particle.Position = new Vector2(monster.Bounds.X + offset.X + monster.Bounds.Width / 2, monster.Bounds.Y + monster.Bounds.Height / 2);
                particle.Velocity = new Vector2(
                    MathHelper.Lerp(-100, 100, (float)rand.NextDouble()), // X between -50 and 50
                    MathHelper.Lerp(0, 100, (float)rand.NextDouble()) // Y between 0 and 100
                    );
                particle.Acceleration = 0.1f * new Vector2(0, (float)-rand.NextDouble());
                particle.Color = Color.White;
                particle.Scale = 1f;
                particle.Life = 1.0f;
            };
            // Set the UpdateParticle method
            particleSystem.UpdateParticle = (float deltaT, ref Particle particle) =>
            {
                particle.Velocity += deltaT * particle.Acceleration;
                particle.Position += deltaT * particle.Velocity;
                particle.Scale -= deltaT;
                particle.Life -= deltaT;
            };
        }

        // Load heart particles
        public void heartParticleLoad()
        {
            // use this.Content to load your game content here
            var heartParticleTexture = Content.Load<Texture2D>("heart_texture");
            heartParticleSystem = new ParticleSystem(this.GraphicsDevice, 1000, heartParticleTexture);
            heartParticleSystem.SpawnPerFrame = 4;
            // Set the SpawnParticle method
            heartParticleSystem.SpawnParticle = (ref Particle heartParticle) =>
            {
                heartParticle.Position = new Vector2(heart.Position.X + offset.X + 25, heart.Position.Y + 9);
                heartParticle.Velocity = new Vector2(
                    MathHelper.Lerp(-100, 100, (float)rand.NextDouble()), // X between -50 and 50
                    MathHelper.Lerp(-100, 100, (float)rand.NextDouble()) // Y between 0 and 100
                    );
                heartParticle.Acceleration = 0.1f * new Vector2(0, (float)-rand.NextDouble());
                heartParticle.Color = Color.White;
                heartParticle.Scale = 1.0f;
                heartParticle.Life = 1.0f;
            };
            // Set the UpdateParticle method
            heartParticleSystem.UpdateParticle = (float heartDeltaT, ref Particle heartParticle) =>
            {
                heartParticle.Velocity += heartDeltaT * heartParticle.Acceleration;
                heartParticle.Position += heartDeltaT * heartParticle.Velocity;
                heartParticle.Scale -= heartDeltaT;
                heartParticle.Life -= heartDeltaT;
            };
        }

        // Load fruit particles
        public void fruitParticleLoad()
        {
            // use this.Content to load your game content here
            var fruitParticleTexture = Content.Load<Texture2D>("fruit_texture");
            fruitParticleSystem = new ParticleSystem(this.GraphicsDevice, 1000, fruitParticleTexture);
            fruitParticleSystem.SpawnPerFrame = 4;
            // Set the SpawnParticle method
            fruitParticleSystem.SpawnParticle = (ref Particle fruitParticle) =>
            {
                fruitParticle.Position = new Vector2(fruit.Position.X + offset.X + 25, fruit.Position.Y);
                fruitParticle.Velocity = new Vector2(
                    MathHelper.Lerp(-100, 100, (float)rand.NextDouble()), // X between -50 and 50
                    MathHelper.Lerp(-200, 0, (float)rand.NextDouble()) // Y between 0 and 100
                    );
                fruitParticle.Acceleration = 0.1f * new Vector2(0, (float)-rand.NextDouble());
                fruitParticle.Color = Color.Purple;
                fruitParticle.Scale = 1.0f;
                fruitParticle.Life = 1.0f;
            };
            // Set the UpdateParticle method
            fruitParticleSystem.UpdateParticle = (float fruitDeltaT, ref Particle fruitParticle) =>
            {
                fruitParticle.Velocity += fruitDeltaT * fruitParticle.Acceleration;
                fruitParticle.Position += fruitDeltaT * fruitParticle.Velocity;
                fruitParticle.Scale -= fruitDeltaT;
                fruitParticle.Life -= fruitDeltaT;
            };
        }
    }
}
