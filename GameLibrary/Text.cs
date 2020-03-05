using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameLibrary
{
    /// <summary>
    /// A class representing a tileset created in Tiled
    /// </summary>
    public class Text
    {
        List<Vector2> Positions;

        /// <summary>
        /// Constructs a new instance of Tileset
        /// </summary>
        /// <param name="positions">The tiles in this set</param>
        public Text(List<Vector2> positions)
        {
            this.Positions = positions;
        }

        /// <summary>
        /// The number of tiles in the set
        /// </summary>
        public int Count => Positions.Count;

    }
}
