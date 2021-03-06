﻿#region File Description
//-----------------------------------------------------------------------------
// EpisodeContent.cs
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
    /// Provides access to the building context to get the Data
    /// this is just in compile time
    /// </summary>
    public class EpisodeContent
    {
        public List<LevelContent> Levels{get; set;}
        public string Name { get; set; }
        public bool InitialEpisode { get; set; }
        public int Order { get; set; }

        public EpisodeContent()
        {
            Levels = new List<LevelContent>();
        }
    }
}
