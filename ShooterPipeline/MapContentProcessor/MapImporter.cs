#region File Description
//-----------------------------------------------------------------------------
// MapImporter.cs
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
using System.IO; 
#endregion

namespace Nexxt.Pipeline.MapContentProcessor
{
	[ContentImporter(".map", DisplayName = "Map Importer", DefaultProcessor = "MapProcessor")]
	public class MapImporter : ContentImporter<string>
	{
		public override string Import(string filename, ContentImporterContext context)
		{
			//all we do is read in the whole file as a string and pass it to the processor.

			string text = string.Empty;

			using (StreamReader reader = new StreamReader(filename))
				text = reader.ReadToEnd();

			return text;
		}
	}
}
