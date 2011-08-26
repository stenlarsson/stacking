using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using System.Xml.Linq;
using System.IO;

namespace TetattContentPipelineExtension
{
    /// <summary>
    /// This processor works the same as FontTextureProcessor, except it
    /// reads what character is at a certain position from the XML file created
    /// by SpriteFont (http://www.nubik.com/SpriteFont/). The XML file should
    /// be named the same as the texture. E.g. if the texture is named font.png
    /// the XML file should be called font.xml.
    /// </summary>
    [ContentProcessor(DisplayName = "Sprite Font Texture with XML")]
    public class FontTextureAndXmlProcessor : FontTextureProcessor
    {
        IList<char> chars;

        public override SpriteFontContent Process(Texture2DContent input, ContentProcessorContext context)
        {
            // Read characters from the corresponding xml file
            string sourcefilename = input.Identity.SourceFilename;
            string filename = sourcefilename.Substring(0, sourcefilename.LastIndexOf('.')) + ".xml";
            using (var stream = new FileStream(filename, FileMode.Open))
            {
                chars = XDocument.Load(stream).Root.Elements("character").Select(
                    e => (char)int.Parse(e.Attribute("key").Value)
                ).ToList();
            }

            return base.Process(input, context);
        }

        protected override char GetCharacterForIndex(int index)
        {
            return chars[index];
        }
    }
}