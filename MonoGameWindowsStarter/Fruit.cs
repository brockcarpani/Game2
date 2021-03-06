﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace MonoGameWindowsStarter
{
    public class Fruit
    {
        // Sprite for fruit
        public Sprite sprite;

        // Starting position of fruit
        public Vector2 Position;

        // Random for generating random x to spawn fruit
        Random rand;

        // Y velocity for fruit
        float yVelocity = 1.5F;

        SoundEffect failSoundEffect;

        /// <summary>
        /// Intitializes fruit to sprite texture
        /// </summary>
        /// <param name="sprite">Sprite created in game init</param>
        public Fruit(Sprite sprite, SoundEffect failSound, Random random)
        {
            this.sprite = sprite;
            this.failSoundEffect = failSound;
            this.rand = random;

            Position = new Vector2(
                (float)rand.Next(0, 2048 - 50), // width of window/game - width of fruit
                50
                );
            Position.Normalize();
        }

        /// <summary>
        /// Update function for fruit
        /// </summary>
        /// <param name="gameTime">gameTime from game</param>
        public void Update(GameTime gameTime)
        {
            Position.Y += yVelocity;
        }

        /// <summary>
        /// Draw for fruit
        /// </summary>
        /// <param name="spriteBatch">spriteBatch from game</param>
        public void Draw(SpriteBatch spriteBatch) 
        {
            sprite.Draw(spriteBatch, new Rectangle((int)Position.X, (int)Position.Y, 50, 50), Color.White);
        }

        public bool collidedWithBounds()
        {
            return (Position.Y >= 768 - 50); // 768 is size of window/game - size of fruit
        }

        public void spawnFruitToTop()
        {
            Position.Y = 50;
            Position.X = (float)rand.Next(0, 2048 - 50); // width of window/game - width of fruit
        }

        public void playFailSoundEffect()
        {
            failSoundEffect.Play();
        }
    }
}
