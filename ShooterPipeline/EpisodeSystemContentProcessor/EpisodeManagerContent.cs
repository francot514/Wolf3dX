#region File Description
//-----------------------------------------------------------------------------
// EpisodeManagerContent.cs
//
// Wolfenstein3DX
// Copyright (C) Nexxt Studios - 2009
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using System.IO;
#endregion

namespace Nexxt.Pipeline.EpisodeSystemContentProcessor
{
    /// <summary>
    /// Represents the Content of the Episode Manager going through the XNA
    /// pipeline.
    /// </summary>
    public class EpisodeManagerContent
    {
        /// <summary>
        /// Holds all the episodes inside this "game"
        /// </summary>
        public List<EpisodeContent> Episodes { get; set; }

        public EpisodeManagerContent()
        {
            Episodes = new List<EpisodeContent>();
        }
    }
}
