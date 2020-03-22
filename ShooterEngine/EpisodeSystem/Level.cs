#region File Description
//-----------------------------------------------------------------------------
// Level.cs
//
// Wolfenstein3DX
// Copyright (C) Nexxt Studios - 2009
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
#endregion

namespace Nexxt.Framework.EpisodeSystem
{
    /// <summary>
    /// Defines the data needed to represent a Level inside the
    /// Object Structure of the game
    /// Holds all the information related to a Level of the game, 
    /// like the resource for this level its name, an its order
    /// inside the parent episode.
    /// </summary>
    public class Level
    {
        public string MapName { get; set; }

        public string MapResource { get; set; }

        public int Order { get; set; }

        public bool IsSecret { get; set; }

        public bool BossLevel { get; set; }
    }
}
