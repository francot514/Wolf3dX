#region File Description
//-----------------------------------------------------------------------------
// EpisodeTypeWriter.cs
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
    /// Serializes a Episode in a binary form to use within the framework
    /// </summary>
    [ContentTypeWriter]
    public class EpisodeTypeWritter : ContentTypeWriter<EpisodeContent>
    {
        /// <summary>
        /// Writes the contents of the EpisodeContent
        /// </summary>
        /// <param name="output">The ContentWriter to write the object</param>
        /// <param name="value">The object to be written</param>
        protected override void Write(ContentWriter output, EpisodeContent value)
        {
            output.Write(value.Name);
            output.Write(value.InitialEpisode);
            output.Write(value.Order);
            output.Write(value.Levels.Count);
            for (int i = 0; i < value.Levels.Count; i++)
            {
                output.WriteObject<LevelContent>(value.Levels[i]);
            }
        }

        /// <summary>
        /// Gets the name of this type writer
        /// </summary>
        /// <param name="targetPlatform">The target platform of this compilation</param>
        /// <returns>A string representing the type writer</returns>
        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "Nexxt.Framework.TypeReaders.EpisodeTypeReader, ShooterEngine";
        }
    }
}
