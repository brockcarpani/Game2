using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework;

using TInput = GameContentExtension.TextContent;

namespace GameContentExtension
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to import a file from disk into the specified type, TImport.
    ///
    /// This should be part of a Content Pipeline Extension Library project.
    ///
    /// TODO: change the ContentImporter attribute to specify the correct file
    /// extension, display name, and default processor for this importer.
    /// </summary>

    [ContentImporter(".txt", DisplayName = "TXT Importer - Positions", DefaultProcessor = "TextProcessor")]
    public class TextImporter : ContentImporter<TInput>
    {

        public override TInput Import(string filename, ContentImporterContext context)
        {
            System.IO.StreamReader file = new System.IO.StreamReader(filename);

            int Count = Int32.Parse(file.ReadLine());

            List<Vector2> vectorList = new List<Vector2>();
            int index = 0;
            while (index < Count)
            {
                String line = file.ReadLine();
                char[] spearator = { ' ' };
                String[] strlist = line.Split(spearator);
                Vector2 v = new Vector2(Int32.Parse(strlist[0]), Int32.Parse(strlist[1]));
                vectorList.Add(v);
                index++;
            }

            return new TextContent()
            {
                PositionCount = Count,
                Positions = vectorList
            };
        }

    }

}
