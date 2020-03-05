using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

using TInput = GameContentExtension.TextContent;
using TOutput = GameContentExtension.TextContent;

namespace GameContentExtension
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to apply custom processing to tileset data 
    /// </summary>
    [ContentProcessor(DisplayName = "TXT Processor - Positions")]
    public class TextProcessor : ContentProcessor<TInput, TOutput>
    {
        /// <summary>
        /// Processes the raw .tsx XML and creates a TilesetContent
        /// for use in an XNA framework game
        /// </summary>
        /// <param name="input">The XML string</param>
        /// <param name="context">The pipeline context</param>
        /// <returns>A TilesetContent instance corresponding to the tsx input</returns>
        public override TOutput Process(TInput input, ContentProcessorContext context)
        {
            // No processing necessary
            return input;
        }
    }
}