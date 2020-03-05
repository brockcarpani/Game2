using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

namespace GameContentExtension
{
    /// <summary>
    /// A class representing the content associated with a Tiled tileset.
    /// It is used as an intermediary stage in the content pipeline
    /// </summary>
    public class TextContent
    {
        public int PositionCount { get; set; }

        public List<Vector2> Positions;

    }
}
