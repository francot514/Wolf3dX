#region File Description
//-----------------------------------------------------------------------------
// LevelTypeWriter.cs
//
// Wolfenstein3DX
// Copyright (C) Nexxt Studios - 2009
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Specialized;
#endregion


namespace Nexxt.Pipeline.EpisodeSystemContentProcessor
{
    /// <summary>
    /// Serializes a Level in a binary form to use within the framework
    /// </summary>
    [ContentTypeWriter]
    public class LevelTypeWritter : ContentTypeWriter<LevelContent>
    {
        protected override void Write(ContentWriter output, LevelContent value)
        {
            output.Write(value.MapName);
            output.Write(value.MapResource);
            output.Write(value.Order);
            output.Write(value.IsSecret);
            output.Write(value.BossLevel);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "Nexxt.Framework.TypeReaders.LevelTypeReader, ShooterEngine";
        }
    }
}
