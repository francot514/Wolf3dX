#region File Description
//-----------------------------------------------------------------------------
// WallSlice.cs
//
// Wolfenstein3DX
// Copyright (C) Nexxt Studios - 2009
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework.Graphics; 
#endregion

namespace Nexxt.Engine.GameObjects
{
	/// <summary>
	/// This struct represents a texture mapped Wall 
	/// </summary>
    public struct WallSlice
	{
		public double Depth { get; internal set; }
		public int Height { get; internal set; }
		public int TextureX { get; internal set; }
		public Texture2D Texture { get; internal set; }
	}
}
