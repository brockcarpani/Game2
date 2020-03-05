using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace MonoGameWindowsStarter
{
    public class Heart
    {
        // Sprite for heart
        public Sprite sprite;

        // Starting position of heart
        public Vector2 Position;

        // Random for generating random x to spawn fruit
        Random rand;

        List<Vector2> positions;

        /// <summary>
        /// Intitializes heart to sprite texture
        /// </summary>
        /// <param name="sprite">Sprite created in game init</param>
        public Heart(Sprite sprite, Random random, List<Vector2> positions)
        {
            this.sprite = sprite;
            this.rand = random;
            this.positions = positions;

            Position = positions[rand.Next(0, positions.Count)];
            //Position.Normalize();
        }

        /// <summary>
        /// Update function for heart
        /// </summary>
        /// <param name="gameTime">gameTime from game</param>
        public void Update(GameTime gameTime)
        {
            // Nothing here
        }

        /// <summary>
        /// Draw for fruit
        /// </summary>
        /// <param name="spriteBatch">spriteBatch from game</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            sprite.Draw(spriteBatch, new Rectangle((int)Position.X, (int)Position.Y, 50, 50), Color.White);
        }

        public void spawnNewHeart()
        {
            Position = positions[rand.Next(0, positions.Count)];
        }
    }
}
