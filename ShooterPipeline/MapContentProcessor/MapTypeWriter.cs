#region File Description
//-----------------------------------------------------------------------------
// MapTypeWriter.cs
//
// Wolfenstein3DX
// Copyright (C) Nexxt Studios - 2009
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Microsoft.Xna.Framework.Content.Pipeline;
#endregion

namespace Nexxt.Pipeline.MapContentProcessor
{
	[ContentTypeWriter]
	public class MapTypeWriter : ContentTypeWriter<MapContent>
	{
		protected override void Write(ContentWriter output, MapContent value)
		{

			//write out the number of textures
			output.Write(value.Textures.Count);

			//write out all the textures
			foreach (var t in value.Textures)
				output.WriteExternalReference(t);

			//write out the background texture
			output.WriteExternalReference(value.Background);

            //Write out the name for the ambient audio
            output.Write(value.AmbientAudio);

            // Write out the Spawning Point
            output.Write(value.SpawnPoint);

            //write the Sprites number
            output.Write(value.Sprites.Count);

            //write each GameObject using for loop (dont use foreach - performance issues)
            for (int i = 0; i < value.Sprites.Count; i++)
            {
                output.WriteObject(value.Sprites[i]);
            }

			//write out the number of map rows
			output.Write(value.Rows.Count);

			//write out each row
			foreach (var r in value.Rows)
				output.WriteObject(r);
		}

		public override string GetRuntimeReader(TargetPlatform targetPlatform)
		{
			return "Nexxt.Engine.TypeReaders.MapTypeReader, ShooterEngine";
		}
	}
}
