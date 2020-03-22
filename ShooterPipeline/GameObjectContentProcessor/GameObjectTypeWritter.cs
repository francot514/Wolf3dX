#region File Description
//-----------------------------------------------------------------------------
// GameObjectTypeWriter.cs
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
using Nexxt.Common.Enums;
using Nexxt.Common.Enums;
#endregion

namespace Nexxt.Pipeline.GameObjectContentProcessor

{
    /// <summary>
    /// Serializes a Game Object in a binary form to use within the framework
    /// </summary>
    [ContentTypeWriter]
    public class GameObjectTypeWriter : ContentTypeWriter<GameObjectContent>
    {
        /// <summary>
        /// Writes the contents of the GameObject
        /// </summary>
        /// <param name="output">The ContentWriter to write the object</param>
        /// <param name="value">The object to be written</param>
        protected override void Write(ContentWriter output, GameObjectContent value)
        {
            output.Write(value.Name);
            output.Write(value.IsActive);
            output.Write(value.IsCollectable);
            output.Write(value.IsMovable);
            if (value.Orientation == null || value.Orientation.Length <= 0 )
                output.Write(Orientation.None.ToString());
            else
                output.Write(value.Orientation.ToString());

            output.WriteExternalReference(value.Texture);
            output.Write(value.Position);
            output.Write(value.PickupSound);
            output.Write(value.StateBag.ToString());
            output.Write((int)value.Difficulties);
        }

        /// <summary>
        /// Gets the name of this type writer
        /// </summary>
        /// <param name="targetPlatform">The target platform of this compilation</param>
        /// <returns>A string representing the type writer</returns>
        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "Nexxt.Framework.TypeReaders.GameObjectTypeReader, ShooterEngine";
        }
    }
}
