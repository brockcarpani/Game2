using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

using TWrite = GameContentExtension.TextContent;

namespace GameContentExtension
{
    /// <summary>
    /// A ContentTypeWriter for the TiledSpriteSheetContent type
    /// </summary>
    [ContentTypeWriter]
    public class TextWriter : ContentTypeWriter<TWrite>
    {

        /// <summary>
        /// Write the binary (xnb) file corresponding to the supplied 
        /// TilesetContent that will be imported into our game
        /// as a Tileset
        /// </summary>
        /// <param name="output">The ContentWriter that writes the binary output</param>
        /// <param name="value">The TilesetContent we are writing</param>
        protected override void Write(ContentWriter output, TWrite value)
        {
            // Write all positions - X then Y
            foreach (Vector2 v in value.Positions)
            {
                output.Write(v.X);
                output.Write(v.Y);
            }

        }

        /// <summary>
        /// Gets the reader needed to read the binary content written by this writer
        /// </summary>
        /// <param name="targetPlatform"></param>
        /// <returns></returns>
        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "GameLibrary.TextReader, GameLibrary";
        }
    }
}
