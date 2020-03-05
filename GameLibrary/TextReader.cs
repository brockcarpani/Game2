using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using TRead = GameLibrary.Text;

namespace GameLibrary
{
    public class TextReader : ContentTypeReader<TRead>
    {
        protected override TRead Read(ContentReader input, TRead existingInstance)
        {
            int PositionCount = input.ReadInt32();

            List<Vector2> vectorList = new List<Vector2>();
            for (int i = 0; i < PositionCount * 2; i ++)
            {
                vectorList.Add(new Vector2(input.ReadInt32(), input.ReadInt32()));
            }

            // Construct and return the text
            return new Text(vectorList);
        }
    }
}
