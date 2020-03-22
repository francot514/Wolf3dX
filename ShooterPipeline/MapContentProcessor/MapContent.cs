#region File Description
//-----------------------------------------------------------------------------
// MapContent.cs
//
// Wolfenstein3DX
// Copyright (C) Nexxt Studios - 2009
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Nexxt.Pipeline.GameObjectContentProcessor; 
#endregion

namespace Nexxt.Pipeline.MapContentProcessor
{
	public class MapContent
	{
		//all the textures used
		public List<ExternalReference<TextureContent>> Textures =
			new List<ExternalReference<TextureContent>>();

		//the map's background
		public ExternalReference<TextureContent> Background;

        public List<GameObjectContent> Sprites = new List<GameObjectContent>();

		//the map's layout stored as a list of rows.
		public List<int[]> Rows = new List<int[]>();

        //the map's Spawn Point for the player
        public Vector2 SpawnPoint;

        //the map's Ambient audio as a string with the name of the resource
        public string AmbientAudio = "";
	}
}
