#region File Description
//-----------------------------------------------------------------------------
// EpisodeManagerTypeWriter.cs
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
    /// Serializes the XML Configuration for the Episode System
    /// </summary>
    [ContentTypeWriter]
    public class EpisodeManagerTypeWritter : ContentTypeWriter<EpisodeManagerContent>
    {
        /// <summary>
        /// Writes the contents of the the EpisodeManager
        /// </summary>
        /// <param name="output">The ContentWriter to write the object</param>
        /// <param name="value">The object to be written</param>
        protected override void Write(ContentWriter output, EpisodeManagerContent value)
        {
            output.Write(value.Episodes.Count);
            for (int i = 0; i < value.Episodes.Count; i++)
            {
                output.WriteObject<EpisodeContent>(value.Episodes[i]);
            }
        }

        /// <summary>
        /// Gets the name of this type writer
        /// </summary>
        /// <param name="targetPlatform">The target platform of this compilation</param>
        /// <returns>A string representing the type writer</returns>
        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "Nexxt.Framework.TypeReaders.EpisodeManagerTypeReader, ShooterEngine";
        }
    }
}
