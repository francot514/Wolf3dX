#region File Description
//-----------------------------------------------------------------------------
// SpriteSheetContent.cs
//
// Wolfenstein3DX
// Copyright (C) Nexxt Studios - 2009
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
#endregion

namespace Nexxt.Pipeline.SpriteSheetPipeline
{
    /// <summary>
    /// Build-time type used to hold the output data from the SpriteSheetProcessor.
    /// This is saved into XNB format by the SpriteSheetWriter helper class, then
    /// at runtime, the SpriteSheetReader loads the data into a SpriteSheet object.
    /// </summary>
    public class SpriteSheetContent
    {
        // Single texture contains many separate sprite images.
        public Texture2DContent Texture = new Texture2DContent();

        // Remember where in the texture each sprite has been placed.
        public List<Rectangle> SpriteRectangles = new List<Rectangle>();

        // Store the original sprite filenames, so we can look up sprites by name.
        public Dictionary<string, int> SpriteNames = new Dictionary<string, int>();
    }
}
