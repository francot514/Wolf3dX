#region File Description
//-----------------------------------------------------------------------------
// EpisodeManagerTypeReader.cs
//
// Wolfenstein3DX
// Copyright (C) Nexxt Studios - 2009
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Specialized;
using Nexxt.Framework.EpisodeSystem;
#endregion

namespace Nexxt.Framework.TypeReaders
{
    public class EpisodeManagerTypeReader : ContentTypeReader<EpisodeManager>
    {
        protected override EpisodeManager Read(ContentReader input, EpisodeManager existingInstance)
        {
            EpisodeManager episodeManager = new EpisodeManager();
            // Reads the contents
            int episodeCount = input.ReadInt32();
            for (int i = 0; i < episodeCount; i++)
            {
                episodeManager.Episodes.Add(input.ReadObject<Episode>());
            }
            return episodeManager;
        }
    }
}
