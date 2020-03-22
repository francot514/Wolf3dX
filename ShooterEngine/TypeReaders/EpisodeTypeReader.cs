#region File Description
//-----------------------------------------------------------------------------
// EpisodeTypeReader.cs
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
    public class EpisodeTypeReader : ContentTypeReader<Episode>
    {
        protected override Episode Read(ContentReader input, Episode existingInstance)
        {
            Episode episode = new Episode();
            //Reads the contents
            episode.Name = input.ReadString();
            episode.InitialEpisode = input.ReadBoolean();
            episode.Order = input.ReadInt32();
            int levelCount = input.ReadInt32();
            for (int i = 0; i < levelCount; i++)
            {
                episode.Levels.Add(input.ReadObject<Level>());
            }
            return episode;
        }
    }
}
