#region File Description
//-----------------------------------------------------------------------------
// SpriteSheetWriter.cs
//
// Wolfenstein3DX
// Copyright (C) Nexxt Studios - 2009
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Microsoft.Xna.Framework.Content.Pipeline;
#endregion

namespace Nexxt.Pipeline.SpriteSheetPipeline
{
    /// <summary>
    /// Content pipeline support class for saving sprite sheet data into XNB format.
    /// </summary>
    [ContentTypeWriter]
    public class SpriteSheetWriter : ContentTypeWriter<SpriteSheetContent>
    {
        /// <summary>
        /// Saves sprite sheet data into an XNB file.
        /// </summary>
        protected override void Write(ContentWriter output, SpriteSheetContent value)
        {
            output.WriteObject(value.Texture);
            output.WriteObject(value.SpriteRectangles.ToArray());
            output.WriteObject(value.SpriteNames);
        }


        /// <summary>
        /// Tells the content pipeline what worker type
        /// will be used to load the sprite sheet data.
        /// </summary>
        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "Nexxt.Engine.TypeReaders.SpriteSheetTypeReader, " +
                   "ShooterEngine, Version=1.0.0.0, Culture=neutral";
        }


        /// <summary>
        /// Tells the content pipeline what CLR type the sprite sheet
        /// data will be loaded into at runtime.
        /// </summary>
        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return "Nexxt.Engine.Graphics.Resources.SpriteSheet, " +
                   "ShooterEngine, Version=1.0.0.0, Culture=neutral";
        }
    }
}
