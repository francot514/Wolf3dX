#region File Description
//-----------------------------------------------------------------------------
// GameObjectTypeReader.cs
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
using Nexxt.Common.Enums; 
#endregion

namespace Nexxt.Framework.TypeReaders
{
    public class GameObjectTypeReader :  ContentTypeReader<GameObject>
    {
        protected override GameObject Read(ContentReader input, GameObject existingInstance)
        {
            GameObject gameObject = new GameObject();
            gameObject.Name = input.ReadString();
            gameObject.IsActive = input.ReadBoolean();
            gameObject.IsCollectable = input.ReadBoolean();
            gameObject.IsMovable = input.ReadBoolean();
            gameObject.Orientation = (Orientation)Enum.Parse(typeof(Orientation), input.ReadString());
            gameObject.Texture = input.ReadExternalReference<Texture2D>();
            gameObject.Position = input.ReadVector2();
            gameObject.PickupSound = input.ReadString();

            // Reads the Object State Bag?
            gameObject.StateBag.FromString(input.ReadString());
            gameObject.Difficulties = (Difficulty)input.ReadInt32();
            return gameObject;
        }
    }
}
