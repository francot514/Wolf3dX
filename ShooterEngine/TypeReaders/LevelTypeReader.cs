#region File Description
//-----------------------------------------------------------------------------
// LevelTypeReader.cs
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
    public class LevelTypeReader : ContentTypeReader<Level>
    {
        protected override Level Read(ContentReader input, Level existingInstance)
        {
            Level level = new Level();
            //Reads the contents
            level.MapName = input.ReadString();
            level.MapResource = input.ReadString();
            level.Order = input.ReadInt32();
            level.IsSecret = input.ReadBoolean();
            level.BossLevel = input.ReadBoolean();
            return level;
        }
    }
}
