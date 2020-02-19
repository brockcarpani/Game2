using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.Diagnostics;

namespace MonoGameWindowsStarter
{
    /// <summary>
    /// An enumeration of possible player animation states
    /// </summary>
    enum PlayerAnimState
    {
        Idle,
        JumpingLeft,
        JumpingRight,
        WalkingLeft,
        WalkingRight,
        FallingLeft,
        FallingRight
    }

    /// <summary>
    /// An enumeration of possible player veritcal movement states
    /// </summary>
    enum VerticalMovementState
    {
        OnGround,
        Jumping,
        Falling
    }

    /// <summary>
    /// A class representing the player
    /// </summary>
    public class Player
    {
        // The speed of the walking animation
        const int FRAME_RATE = 300;

        // The duration of a player's jump, in milliseconds
        const int JUMP_TIME = 500;

        // The player sprite frames
        Sprite[] frames;

        // The currently rendered frame
        int currentFrame = 0;

        // The player's animation state
        PlayerAnimState animationState = PlayerAnimState.Idle;

        // The player's vertical movement state
        VerticalMovementState verticalState = VerticalMovementState.OnGround;

        // The player's speed
        int speed = 5;

        // If the player is jumping
        bool jumping = false;

        // If the player is falling 
        bool falling = false;

        // A timer for jumping
        TimeSpan jumpTimer;

        // A timer for animations
        TimeSpan animationTimer;

        // The currently applied SpriteEffects
        SpriteEffects spriteEffects = SpriteEffects.None;

        // The color of the sprite
        Color color = Color.White;

        // The origin of the sprite (centered on its feet)
        Vector2 origin = new Vector2(25, 50);

        /// <summary>
        /// Gets and sets the position of the player on-screen
        /// </summary>
        public Vector2 Position = new Vector2(500, 768);

        // Sound effect for eating
        SoundEffect eatSoundEffect;

        public BoundingRectangle Bounds => new BoundingRectangle(Position - origin, 50, 50);

        /// <summary>
        /// Constructs a new player
        /// </summary>
        /// <param name="frames">The sprite frames associated with the player</param>
        public Player(IEnumerable<Sprite> frames, SoundEffect eatSound)
        {
            this.frames = frames.ToArray();
            this.eatSoundEffect = eatSound;
            animationState = PlayerAnimState.WalkingLeft;
        }

        /// <summary>
        /// Updates the player, applying movement and physics
        /// </summary>
        /// <param name="gameTime">The GameTime object</param>
        public void Update(GameTime gameTime)
        {
            var keyboard = Keyboard.GetState();

            // Vertical movement
            switch (verticalState)
            {
                case VerticalMovementState.OnGround:
                    if (keyboard.IsKeyDown(Keys.Space))
                    {
                        verticalState = VerticalMovementState.Jumping;
                        jumpTimer = new TimeSpan(0);
                    }
                    break;
                case VerticalMovementState.Jumping:
                    jumpTimer += gameTime.ElapsedGameTime;
                    // Simple jumping with platformer physics
                    Position.Y -= (250 / (float)jumpTimer.TotalMilliseconds);
                    if (jumpTimer.TotalMilliseconds >= JUMP_TIME) verticalState = VerticalMovementState.Falling;
                    break;
                case VerticalMovementState.Falling:
                    Position.Y += speed;
                    // TODO: This needs to be replaced with collision logic
                    if (Position.Y >= 768)
                    {
                        verticalState = VerticalMovementState.OnGround;
                        Position.Y = 768;
                    }
                    break;
            }


            // Horizontal movement
            if (keyboard.IsKeyDown(Keys.Left))
            {
                animationState = PlayerAnimState.WalkingLeft;
                Position.X -= speed;
            }
            else if (keyboard.IsKeyDown(Keys.Right))
            {
                animationState = PlayerAnimState.WalkingRight;
                Position.X += speed;
            }
            else
            {
                animationState = PlayerAnimState.Idle;
            }


            // Apply animations
            switch (animationState)
            {
                case PlayerAnimState.Idle:
                    currentFrame = 0;
                    animationTimer = new TimeSpan(0);
                    break;

                case PlayerAnimState.JumpingLeft:
                    spriteEffects = SpriteEffects.FlipHorizontally;
                    break;

                case PlayerAnimState.JumpingRight:
                    spriteEffects = SpriteEffects.None;
                    break;

                case PlayerAnimState.WalkingLeft:
                    animationTimer += gameTime.ElapsedGameTime;
                    spriteEffects = SpriteEffects.FlipHorizontally;
                    // Walking frames are 9 & 10
                    if (animationTimer.TotalMilliseconds > FRAME_RATE * 2)
                    {
                        animationTimer = new TimeSpan(0);
                    }
                    currentFrame = (int)Math.Floor(animationTimer.TotalMilliseconds / FRAME_RATE);
                    break;

                case PlayerAnimState.WalkingRight:
                    animationTimer += gameTime.ElapsedGameTime;
                    spriteEffects = SpriteEffects.None;
                    // Walking frames are 9 & 10
                    if (animationTimer.TotalMilliseconds > FRAME_RATE * 2)
                    {
                        animationTimer = new TimeSpan(0);
                    }
                    currentFrame = (int)Math.Floor(animationTimer.TotalMilliseconds / FRAME_RATE);
                    break;

            }
        }

        /// <summary>
        /// Render the player sprite.  Should be invoked between 
        /// SpriteBatch.Begin() and SpriteBatch.End()
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch to use</param>
        public void Draw(SpriteBatch spriteBatch)
        {
#if VISUAL_DEBUG
            VisualDebugging.DrawRectangle(spriteBatch, Bounds, Color.Red);
#endif
            frames[currentFrame].Draw(spriteBatch, Position, color, 0, origin, 2, spriteEffects, 1);
        }

        public bool CollidedWithFruit(Fruit fruit)
        {
            return (Position.X < fruit.Position.X + fruit.sprite.Width
                    && Position.X + Bounds.Width > fruit.Position.X
                    && Position.Y < fruit.Position.Y + fruit.sprite.Height
                    && Position.Y + Bounds.Height > fruit.Position.Y);
        }

        public void CheckForPlatformCollision(IEnumerable<IBoundable> platforms)
        {
            Debug.WriteLine($"Checking collisions against {platforms.Count()} platforms");
            if (verticalState != VerticalMovementState.Jumping)
            {
                verticalState = VerticalMovementState.Falling;
                foreach (Platform platform in platforms)
                {
                    if (Bounds.CollidesWith(platform.Bounds))
                    {
                        Position.Y = platform.Bounds.Y - 1;
                        verticalState = VerticalMovementState.OnGround;
                    }
                }
            }
        }

        public void playEatSoundEffect()
        {
            eatSoundEffect.Play();
        }
    }
}